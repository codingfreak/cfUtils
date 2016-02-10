namespace codingfreaks.cfUtils.Logic.Azure
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.WindowsAzure.Storage.Table;

    /// <summary>
    /// Contains helper and extension methods for the <see cref="CloudTable"/> type.
    /// </summary>
    public class TableHelper<TTableItem>
        where TTableItem : TableEntity, new()
    {
        #region member vars

        private bool _isMonitoringRunning;

        private AutoResetEvent _monitoringFinished;
        private readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();

        #endregion

        #region events

        /// <summary>
        /// Occurs when <see cref="GetEntriesAsync(CloudTable,TimeSpan)"/> receives new entries.
        /// </summary>
        public event EventHandler<TableEntityListEventArgs<TTableItem>> EntriesReceived;

        /// <summary>
        /// Occurs when <see cref="MonitorTableAsync"/> receives new entries.
        /// </summary>
        public event EventHandler<TableEntityListEventArgs<TTableItem>> MonitoringReceivedNewEntries;

        /// <summary>
        /// Occurs when a storage query starts.
        /// </summary>
        public event EventHandler QueryStarted;

        /// <summary>
        /// Occurs after a storage query finishes.
        /// </summary>
        public event EventHandler QueryFinished;

        #endregion

        #region methods

        /// <summary>
        /// Starts a process that retrieves all entries inside a given <paramref name="timeSlot"/>.
        /// </summary>
        /// <remarks>
        /// After 1000 elements are loaded this method will fire <see cref="EntriesReceived"/>.
        /// </remarks>
        /// <param name="table">The Azure WADLogs table to query against.</param>
        /// <param name="timeSlot">The amount of time to go into the past.</param>
        /// <returns>All items from the WADLogs table inside the <paramref name="timeSlot"/>.</returns>
        /// <typeparam name="TTableItem">The type of the items in the table.</typeparam>
        public async Task<IEnumerable<TTableItem>> GetEntriesAsync(CloudTable table, TimeSpan timeSlot)
        {
            var partitionKeyMin = "0" + DateTime.UtcNow.Subtract(timeSlot).Ticks;
            return await GetEntriesAsync(table, partitionKeyMin);
        }

        /// <summary>
        /// Starts a process that retrieves all entries with a partition key greater or equal than <paramref name="minTimestamp"/>
        /// </summary>
        /// <remarks>
        /// After 1000 elements are loaded this method will fire <see cref="EntriesReceived"/>.
        /// </remarks>
        /// <param name="table">The Azure WADLogs table to query against.</param>
        /// <param name="minTimestamp">The smallest WADLogs partition key to put into result.</param>
        /// <returns>All items from the WADLogs table inside the defined time.</returns>
        public async Task<IEnumerable<TTableItem>> GetEntriesAsync(CloudTable table, string minTimestamp)
        {
            var term = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.GreaterThanOrEqual, minTimestamp);
            var query = new TableQuery<TTableItem>().Where(term);
            TableContinuationToken continuationToken = null;
            var result = new List<TTableItem>();
            do
            {
                var stopWatch = new Stopwatch();
                QueryStarted?.Invoke(null, EventArgs.Empty);
                stopWatch.Start();
                try
                {
                    var tableQueryResult = await table.ExecuteQuerySegmentedAsync(query, continuationToken);
                    stopWatch.Stop();
                    LastQueryTime = stopWatch.Elapsed;
                    QueryFinished?.Invoke(null, EventArgs.Empty);
                    continuationToken = tableQueryResult.ContinuationToken;
                    var entries = tableQueryResult.Results.ToList();                    
                    EntriesReceived?.Invoke(null, new TableEntityListEventArgs<TTableItem>(entries));
                    result.AddRange(entries);
                }
                catch (Exception ex)
                {                    
                    Trace.TraceError(ex.Message);
                }
                
            }
            while (continuationToken != null);
            return result;
        }

        /// <summary>
        /// Watches the given <paramref name="table"/> every <paramref name="intervalSeconds"/> s for added entries.
        /// </summary>
        /// <remarks>
        /// If new entries are available the <see cref="MonitoringReceivedNewEntries"/> event is raised.
        /// </remarks>
        /// <param name="table">The Azure WADLogs table to query against.</param>
        /// <param name="cancellationToken">An external cancellation token to stop monitoring.</param>
        /// <param name="intervalSeconds">The interval in seconds the monitoring should try to retrieve new elements.</param>
        /// <param name="timeSpanSeconds">The amount of seconds to look in the past with the first request.</param>        
        public async Task MonitorTableAsync(CloudTable table, CancellationToken cancellationToken, int intervalSeconds = 5, double timeSpanSeconds = 3600)
        {
            if (_isMonitoringRunning)
            {
                throw new InvalidOperationException("Only one monitoring per instance allowed!");
            }
            _monitoringFinished = new AutoResetEvent(false);
            _isMonitoringRunning = true;
            var nextTicksToRetrieve = "0" + DateTime.Now.Subtract(TimeSpan.FromSeconds(timeSpanSeconds)).Ticks;
            EntriesReceived += (s, e) => MonitoringReceivedNewEntries?.Invoke(this, new TableEntityListEventArgs<TTableItem>(e.Entries.ToList()));
            while (true)
            {
                var entries = string.IsNullOrEmpty(nextTicksToRetrieve) ? GetEntriesAsync(table, TimeSpan.FromSeconds(timeSpanSeconds)).Result.ToList() : GetEntriesAsync(table, nextTicksToRetrieve).Result.ToList();
                var nextTicks = "0";
                if (entries.Any())
                {
                    nextTicks = "0" + (entries.Max(e => long.Parse(e.PartitionKey)) + 1);
                }                
                if (entries.Any() && nextTicksToRetrieve != nextTicks)
                {
                    nextTicksToRetrieve = nextTicks;
                    //MonitoringReceivedNewEntries?.Invoke(null, new TableEntityListEventArgs<TTableItem>(entries));
                }
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(intervalSeconds), cancellationToken);
                }
                catch (TaskCanceledException)
                {
                    Trace.TraceInformation("Monitoring cancelled.");
                }
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
            }
            _isMonitoringRunning = false;
            _monitoringFinished.Set();
        }

        /// <summary>
        /// Starts <see cref="MonitorTableAsync"/> and leaves the method immediately.
        /// </summary>
        /// <remarks>
        /// Use <see cref="StopMonitoringTable"/> to cancel the monitoring.
        /// </remarks>
        /// <param name="table">The Azure WADLogs table to query against.</param>        
        /// <param name="intervalSeconds">The interval in seconds the monitoring should try to retrieve new elements.</param>
        /// <param name="timeSpanSeconds">The amount of seconds to look in the past with the first request.</param>  
        public void StartMonitoringTable(CloudTable table, int intervalSeconds = 5, double timeSpanSeconds = 3600)
        {
            var token = _tokenSource.Token;
            Task.Factory.StartNew(() => MonitorTableAsync(table, token, intervalSeconds, timeSpanSeconds), token);
        }

        /// <summary>
        /// Stops a monitoring started by <see cref="StartMonitoringTable"/>.
        /// </summary>
        public void StopMonitoringTable()
        {
            if (!_isMonitoringRunning)
            {
                throw new InvalidOperationException("No monitoring running!");
            }
            _tokenSource.Cancel();
            _monitoringFinished.WaitOne();
        }

        #endregion

        #region properties

        /// <summary>
        /// The amount of time the last Azure query took.
        /// </summary>
        public TimeSpan LastQueryTime { get; private set; }

        #endregion
    }
}
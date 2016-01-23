namespace s2.s2Utils.Logic.Azure
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
    public static class WadLogTableHelper
    {
        #region events

        /// <summary>
        /// Occurs when <see cref="GetEntriesAsync(CloudTable,string)"/> receives new entries.
        /// </summary>
        public static event EventHandler<WadLogEntityListEventArgs> EntriesReceived;

        /// <summary>
        /// Occurs when <see cref="MonitorTableAsync"/> receives new entries.
        /// </summary>
        public static event EventHandler<WadLogEntityListEventArgs> MonitoringReceivedNewEntries;

        /// <summary>
        /// Occurs when a storage query starts.
        /// </summary>
        public static event EventHandler QueryStarted;

        /// <summary>
        /// Occurs after a storage query finishes.
        /// </summary>
        public static event EventHandler QueryFinished;

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
        public static async Task<IEnumerable<WadLogEntity>> GetEntriesAsync(this CloudTable table, TimeSpan timeSlot)
        {
            var partitionKeyMin = "0" + DateTime.UtcNow.Subtract(timeSlot).Ticks;
            return await table.GetEntriesAsync(partitionKeyMin);
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
        public static async Task<IEnumerable<WadLogEntity>> GetEntriesAsync(this CloudTable table, string minTimestamp)
        {
            var term = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.GreaterThanOrEqual, minTimestamp);
            var query = new TableQuery<WadLogEntity>().Where(term);
            TableContinuationToken continuationToken = null;
            var result = new List<WadLogEntity>();
            do
            {
                var stopWatch = new Stopwatch();
                QueryStarted?.Invoke(null, EventArgs.Empty);
                stopWatch.Start();
                var tableQueryResult = await table.ExecuteQuerySegmentedAsync(query, continuationToken);
                stopWatch.Stop();
                LastQueryTime = stopWatch.Elapsed;
                QueryFinished?.Invoke(null, EventArgs.Empty);
                continuationToken = tableQueryResult.ContinuationToken;
                var entries = tableQueryResult.Results.ToList();
                EntriesReceived?.Invoke(null, new WadLogEntityListEventArgs(entries));
                result.AddRange(entries);
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
        public static async Task MonitorTableAsync(this CloudTable table, CancellationToken cancellationToken, int intervalSeconds = 5, int timeSpanSeconds = 3600)
        {
            var lastTicks = "";
            while (true)
            {
                var entries = string.IsNullOrEmpty(lastTicks) ? table.GetEntriesAsync(TimeSpan.FromSeconds(timeSpanSeconds)).Result.ToList() : table.GetEntriesAsync(lastTicks).Result.ToList();
                var maxTicks = entries.Max(e => e.PartitionKey);
                if (entries.Any() && lastTicks != maxTicks)
                {
                    lastTicks = maxTicks;
                    MonitoringReceivedNewEntries?.Invoke(null, new WadLogEntityListEventArgs(entries));
                }
                await Task.Delay(TimeSpan.FromSeconds(intervalSeconds), cancellationToken);
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
            }
        }

        #endregion

        #region properties

        /// <summary>
        /// The amount of time the last Azure query took.
        /// </summary>
        public static TimeSpan LastQueryTime { get; private set; }

        #endregion
    }
}
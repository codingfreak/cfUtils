namespace codingfreaks.cfUtils.Logic.Csv
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;

    using Core.Extensions;
    using Core.Utilities;

    public class Importer<T>
    {
        #region member vars

        /// <summary>
        /// The amount of data rows counted by <see cref="CheckFileStructureAsync"/> if called ever.
        /// </summary>
        private long? _dataRows;

        /// <summary>
        /// The field names derived from the header line if present and configured.
        /// </summary>
        private string[] _fieldNames;

        /// <summary>
        /// Storage for in-order mapping of data lines read.
        /// </summary>
        private ConcurrentQueue<(long offset, string[] itemData)> _incomingData;

        /// <summary>
        /// The amount of lines skipped for any reason (empty, regex, options).
        /// </summary>
        private long _skippedLines;

        /// <summary>
        /// <c>true</c> when the first data-row is read.
        /// </summary>
        private bool _waitingForData;

        #endregion

        #region constructors and destructors

        /// <summary>
        /// Default constructor taking <paramref name="options" /> in.
        /// </summary>
        /// <param name="options">The importer options to use.</param>
        /// <exception cref="NullReferenceException">Is thrown if <paramref name="options" /> is <c>null</c>.</exception>
        public Importer(ImporterOptions options)
        {
            CheckUtil.ThrowIfNull(() => options);
            Options = options;
        }

        #endregion

        #region methods

        /// <summary>
        /// Checks whether the data structure of the provided <paramref name="fileUri" /> is consistent and matching the
        /// <see cref="Options" />.
        /// </summary>
        /// <remarks>
        /// <para>It does the same thing like <see cref="ImportAsync" /> but will not map the data.</para>
        /// <para>As a result of calling this method <see cref="ImportAsync" /> will report relative progress in % automatically.</para>
        /// </remarks>
        /// <param name="fileUri">The absolute path to the file.</param>
        /// <param name="progress">An optional progress to report progress back to the caller.</param>
        /// <param name="cancellationToken">An optional token to cancel the operation by the caller.</param>
        /// <returns><c>true</c> if the structure is ok otherwise <c>false</c>.</returns>
        public async Task<bool> CheckFileStructureAsync(string fileUri, IProgress<OperationProgress> progress = null, CancellationToken cancellationToken = default)
        {
            _dataRows = 0;
            var result = true;
            try
            {
                try
                {
                    await ReadFileAsync(
                        items =>
                        {
                            if (_fieldNames.Length != items.Length)
                            {
                                Log($"Line {_dataRows + 1} has an invalid amount of columns.");
                                result = false;
                            }
                            _dataRows++;
                        },
                        fileUri,
                        progress,
                        cancellationToken);
                }
                catch (Exception ex)
                {
                    ThrowException(new InvalidOperationException("Error during import.", ex));
                }
                return result;
            }
            catch (Exception ex)
            {
                ThrowException(new InvalidOperationException("Could not check file structure. See inner exception for details.", ex));
            }
            // This point is never reached but the compiler cannot understand it because it does not understand ThrowException.
            return false;
        }

        /// <summary>
        /// Controls the import of a single file.
        /// </summary>
        /// <param name="fileUri">The absolute path to the file.</param>
        /// <param name="progress">An optional progress to report progress back to the caller.</param>
        /// <param name="cancellationToken">An optional token to cancel the operation by the caller.</param>
        /// <returns>An instance of <see cref="ImportResult{TResult}" /> containing the results and meta-data.</returns>
        public async Task<ImportResult<T>> ImportAsync(string fileUri, IProgress<OperationProgress> progress = null, CancellationToken cancellationToken = default)
        {
            if (IsBusy)
            {
                throw new InvalidOperationException("Another operation is running on this instance.");
            }
            IsBusy = true;
            CheckUtil.ThrowIfNullOrEmpty(() => fileUri);
            if (!Options.Valid)
            {
                ThrowException(new ApplicationException("Provided options are not valid!"));
            }
            if (!File.Exists(fileUri))
            {
                ThrowException(new FileNotFoundException("Provided file not found.", fileUri));
            }
            var started = DateTimeOffset.Now;
            if (Options.CheckFileBeforeImport)
            {
                // caller wants us to check the file structure before we run the actual import process
                Log("File check started.");
                try
                {
                    var checkResult = await CheckFileStructureAsync(fileUri, null, cancellationToken);
                    if (!checkResult)
                    {
                        Log("File check failed.");
                        IsBusy = false;
                        return new ImportResult<T>(false, null, started, DateTimeOffset.Now);
                    }
                    Log("File check succeeded.");
                }
                catch (Exception ex)
                {
                    ThrowException(new InvalidOperationException("Error during operation. See inner exception for details.", ex));
                }
            }
            // initialize the mapping process and data
            _incomingData = new ConcurrentQueue<(long offset, string[] itemData)>();
            _waitingForData = true;
            var watcherTask = StartQueueWatcher(cancellationToken);
            // perform the file parsing
            try
            {
                var offset = 0;
                await ReadFileAsync(
                    items =>
                    {
                        _incomingData.Enqueue((offset, items));
                        offset++;
                        _waitingForData = false;
                    },
                    fileUri,
                    progress,
                    cancellationToken);
            }
            catch (Exception ex)
            {
                ThrowException(new InvalidOperationException("Error during import.", ex));
            }
            await watcherTask;
            var results = Results.OrderBy(r => r.Key).Select(r => r.Value).AsEnumerable();
            IsBusy = false;
            return new ImportResult<T>(true, results, started, DateTimeOffset.Now, _skippedLines++);
        }

        /// <summary>
        /// Logs a given <paramref name="text" /> to every output available and configured.
        /// </summary>
        /// <param name="text">The text to write to the outputs.</param>
        private void Log(string text)
        {
            Options.Logger?.Invoke(text);
            TraceLog(text);
        }

        /// <summary>
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private T MapDataToItem(string[] data)
        {            
            throw new NotImplementedException();
        }

        /// <summary>
        /// Traverses the file using the <see cref="Options" /> and triggers the <paramref name="mappingAction" /> for every data
        /// row found.
        /// </summary>
        /// <param name="mappingAction">The action to perform for every data row containing the the data row values.</param>
        /// <param name="fileUri">The absolute path to the file.</param>
        /// <param name="progress">An optional progress to report progress back to the caller.</param>
        /// <param name="cancellationToken">An optional token to cancel the operation by the caller.</param>
        private async Task ReadFileAsync(Action<string[]> mappingAction, string fileUri, IProgress<OperationProgress> progress = null, CancellationToken cancellationToken = default)
        {
            CheckUtil.ThrowIfNull(() => mappingAction);
            var encoding = Encoding.Default;
            if (Options.AutoDetectEncoding)
            {
                // caller wants us to determine the encoding using the BOM automatically
                try
                {
                    encoding = FileUtils.GetEncoding(fileUri);
                }
                catch (Exception ex)
                {
                    ThrowException(new InvalidOperationException("Error during encoding-auto-resolve. See inner exception for details.", ex));
                }
            }
            else
            {
                // caller passes the encoding in manually
                encoding = Options.Encoding;
            }
            var currentLine = 0;
            var currentDataRow = 0;
            _skippedLines = 0;
            var headersPassed = false;
            Results = new ConcurrentDictionary<long, T>();
            _fieldNames = null;
            Regex regex = null;
            if (!Options.IgnoreLinesRegex.IsNullOrEmpty())
            {
                regex = new Regex(Options.IgnoreLinesRegex);
            }
            try
            {
                using (var stream = File.OpenRead(fileUri))
                {
                    using (var reader = new StreamReader(stream, encoding))
                    {
                        while (!reader.EndOfStream)
                        {
                            if (cancellationToken.IsCancellationRequested)
                            {
                                Log("Cancellation requested.");
                                break;
                            }
                            currentLine++;
                            var line = await reader.ReadLineAsync();
                            if (line.IsNullOrEmpty())
                            {
                                // handle empty lines                                
                                if (Options.ThrowOnEmptyLines)
                                {
                                    ThrowException(new InvalidOperationException($"Line number {currentLine} is empty."));
                                }
                                // we can't do anything useful with an empty string so we go to the next line
                                Log("Skipping empty line.");
                                _skippedLines++;
                                continue;
                            }
                            if (regex != null)
                            {
                                // check every line for the passed regex
                                if (regex.IsMatch(line))
                                {
                                    // skip this line in any case because it matches the pattern
                                    Log("Skipping line due to regex hit.");
                                    _skippedLines++;
                                    continue;
                                }
                            }
                            if (!headersPassed)
                            {
                                // we are not in the data block yet because we are skipping lines or waiting for the header
                                if (Options.SkipLines > _skippedLines)
                                {
                                    // skip this line because the caller wanted to do this for a certain amount of times
                                    _skippedLines++;
                                    continue;
                                }
                                if (Options.FirstReadedLineContainsHeader)
                                {
                                    // this should be line containing the field names
                                    _fieldNames = line.Split(Options.Delimiter).ToArray();
                                    if (!_fieldNames.Any())
                                    {
                                        ThrowException(new InvalidOperationException("Could not read field names from file. Check delimiter option or file."));
                                    }
                                    Log("Headers handled.");
                                    headersPassed = true;
                                    continue;
                                }
                            }
                            // at this point the line should contain data
                            var items = line.Split(Options.Delimiter);
                            if (currentDataRow == 0 && (!_fieldNames?.Any() ?? false))
                            {
                                // set the field names array to an array of empty strings with the amount of fields
                                // coming from the first data row
                                _fieldNames = new List<string>(items.Length).ToArray();
                            }
                            // the caller is responsible to do something with the data
                            mappingAction.Invoke(items);
                            // count up the row offset
                            currentDataRow++;
                            // report the progress if the caller has passed one
                            progress?.Report(new OperationProgress(currentDataRow, _dataRows));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ThrowException(new InvalidOperationException("Error during read operation. See inner exception for details.", ex));
            }
        }

        private Task StartQueueWatcher(CancellationToken cancellationToken = default)
        {
            return Task.Run(
                async () =>
                {
                    while (!cancellationToken.IsCancellationRequested && (!_incomingData.IsEmpty || _waitingForData))
                    {
                        await Task.Delay(10, cancellationToken);
                        if (_incomingData.TryDequeue(out var data))
                        {
                            // TODO handle MaxDOP
                            var item = MapDataToItem(data.itemData);
                            if (!Results.TryAdd(data.offset, item))
                            {
                                // TODO 
                                Log("Could not add result data.");
                            }
                        }
                    }
                },
                cancellationToken);
        }

        /// <summary>
        /// Throws the provided <paramref name="exception" />
        /// </summary>
        /// <param name="exception">The exception to take care about.</param>
        /// <param name="setUnbusy"><c>true</c> if the import operation should considered stopped after this exception.</param>
        private void ThrowException(Exception exception, bool setUnbusy = true)
        {
            if (exception == null)
            {
                return;
            }
            Log(exception.Message);
            if (setUnbusy)
            {
                IsBusy = false;
            }
            throw exception;
        }

        /// <summary>
        /// Writes a given <paramref name="text" /> to the trace if TRACE symbol is configured.
        /// </summary>
        /// <param name="text">The text to write to the log.</param>
        [Conditional("TRACE")]
        private static void TraceLog(string text)
        {
            TraceUtil.WriteTraceDebug(text);
        }

        #endregion

        #region properties

        /// <summary>
        /// Indicates if this instance is importing currently.
        /// </summary>
        public bool IsBusy { get; private set; }

        /// <summary>
        /// The options to use in this importer instance.
        /// </summary>
        public ImporterOptions Options { get; }

        /// <summary>
        /// Is used to collect items during a running import operation.
        /// </summary>
        private ConcurrentDictionary<long, T> Results { get; set; }

        #endregion
    }
}
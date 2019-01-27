namespace codingfreaks.cfUtils.Logic.Csv
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;

    using Core.Extensions;
    using Core.Utilities;

    /// <summary>
    /// Handles CSV-imports for one file.
    /// </summary>
    /// <typeparam name="T">The type which represents a single row of the CSV file after it's mapped.</typeparam>
    public class Importer<T>
        where T : new()
    {
        #region member vars

        /// <summary>
        /// The amount of data rows counted by <see cref="CheckFileStructureAsync" /> if called ever.
        /// </summary>
        private long? _dataRows;

        /// <summary>
        /// The field names derived from the header line if present and configured.
        /// </summary>
        private string[] _fieldNames;

        /// <summary>
        /// The amount of rows handled completely in the current job.
        /// </summary>
        private long _handledRows;

        /// <summary>
        /// Storage for in-order mapping of data lines read.
        /// </summary>
        private ConcurrentQueue<(long offset, string[] itemData)> _incomingData;

        /// <summary>
        /// Holds data for the <see cref="PropertyInfos"/>.
        /// </summary>
        private Dictionary<string, (PropertyInfo propertyInfo, PropertyAttribute propertyAttribute, TypeConverter converter)> _propertyInfos;

        /// <summary>
        /// Is set to <c>true</c> when the process of reading the complete file is finished.
        /// </summary>
        /// <remarks>
        /// The reading only gets the data and put's it to the <see cref="_incomingData"/> queue.
        /// </remarks>
        private bool _readingFinished;

        /// <summary>
        /// Is used as a lock for accessing the <see cref="Results"/> property in MT scenarios.
        /// </summary>
        private readonly object _resultLock = new object();

        /// <summary>
        /// The amount of mapper jobs running currently.
        /// </summary>
        private int _runningMappers;

        /// <summary>
        /// The amount of lines skipped for any reason (empty, regex, options).
        /// </summary>
        private long _skippedLines;
  
        #endregion

        #region events

        /// <summary>
        /// Occurs when a single line is imported completely.
        /// </summary>
        public event EventHandler<ItemEventArgs<T>> ItemImported;

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
            PopulatePropertyInfos();
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
            var resultsList = new List<(long offset, string[] itemData)>();
            _readingFinished = false;
            // perform the file parsing
            try
            {
                var offset = 0;
                await ReadFileAsync(
                    items =>
                    {
                        resultsList.Add((offset, items));
                        offset++;                        
                    },
                    fileUri,                    
                    cancellationToken);
                
            }
            catch (Exception ex)
            {
                ThrowException(new InvalidOperationException("Error during import.", ex));
            }
            _readingFinished = true;
            _incomingData = new ConcurrentQueue<(long offset, string[] itemData)>(resultsList);
            resultsList.FreeFromMemory();
            Log($"File content read completely after {DateTimeOffset.Now.Subtract(started)}.");
            await StartQueueWatcher(progress, cancellationToken);
            var results = Results.SelectMany(l => l).OrderBy(r => r.offset).Select(r => r.item).AsEnumerable();
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
        /// Is called once for each row in the CSV file to map the CSV-fields to an instance of type <typeparamref name="T"/>.
        /// </summary>
        /// <remarks>
        /// This is the import part of type-safe mapping CSV-text-data to .NET properties.
        /// </remarks>
        /// <param name="data">The data of the CSV line.</param>
        /// <param name="currentOffset">The current line number.</param>
        /// <returns>The mapped instance including all properties.</returns>
        private T MapDataToItem(string[] data, long currentOffset)
        {
            var result = new T();
            foreach (var mapping in PropertyInfos)
            {
                var context = new BaseTypeDescriptorContext(result, mapping.Value.propertyInfo.Name);
                var textValue = string.Empty;
                var fieldName = mapping.Value.propertyAttribute.FieldName;
                if (mapping.Value.propertyAttribute == null)
                {
                    // no attribute on this property was found
                    var offset = _fieldNames.GetIndexOf(fn => fn.Equals(mapping.Value.propertyInfo.Name, StringComparison.OrdinalIgnoreCase));
                    if (offset >= 0)
                    {
                        textValue = data[offset];
                    }
                }
                else
                {
                    // attribute was found on the property
                    if (!fieldName.IsNullOrEmpty())
                    {
                        // the attribute defines a field name which we will use now to obtain the offset of the data-entry
                        var offset = _fieldNames.GetIndexOf(fieldName);
                        if (offset < 0)
                        {
                            // this is invalid and means the caller has misspelled the field name when defining the property
                            ThrowException(new InvalidOperationException($"Field name {fieldName} not found in import data."));
                        }
                        // found the offset -> get the data
                        textValue = data[offset];
                    }
                    else if (fieldName.IsNullOrEmpty() && mapping.Value.propertyAttribute.Offset.HasValue)
                    {
                        // retrieve value from offset defined in the property attribute
                        textValue = data[mapping.Value.propertyAttribute.Offset.Value];
                    }
                }
                // we've got some string value and try to map it the property using the associated converter
                try
                {
                    var converted = mapping.Value.converter.ConvertFromString(context, Options.Culture, textValue);
                    mapping.Value.propertyInfo.SetValue(result, converted);
                }
                catch (Exception ex)
                {
                    ThrowException(new InvalidOperationException($"Could not set value {textValue} to property {mapping.Value.propertyInfo.Name} on line {currentOffset}.", ex));
                }
            }
            return result;
        }

        /// <summary>
        /// Taskes a bunch of <paramref name="items" /> containing the offset of each item and the parsed values as a string array
        /// and handles them in one thread.
        /// </summary>
        /// <param name="items">The items to handle in one thread.</param>
        /// <param name="progress">An optional progress to report progress back to the caller.</param>
        /// <param name="cancellationToken">An optional token to cancel the operation by the caller.</param>
        private void MapItems(IEnumerable<(long offset, string[] data)> items, IProgress<OperationProgress> progress = null, CancellationToken cancellationToken = default)
        {
            Task.Run(
                () =>
                {
                    Stopwatch watch = null;
                    if (Options.OutputMappingPerformance)
                    {
                        watch = new Stopwatch();
                        watch.Start();
                    }
                    var results = new List<(long offset, T item)>();
                    foreach (var item in items)
                    {
                        var result = MapDataToItem(item.data, item.offset);
                        results.Add((item.offset, result));
                        // data was added correctly
                        // report the progress if the caller has passed 
                        Interlocked.Increment(ref _handledRows);
                        progress?.Report(new OperationProgress(_handledRows, _dataRows));
                        ItemImported?.Invoke(this, new ItemEventArgs<T>(result));
                    }
                    // add local results to the overall result
                    Results.Add(results);
                    if (watch != null)
                    {
                        watch.Stop();
                        Log($"Handled {results.Count} items in {watch.Elapsed}.");
                    }
                    results.FreeFromMemory();
                    Interlocked.Decrement(ref _runningMappers);
                },
                cancellationToken);
        }

        /// <summary>
        /// Uses reflection to gain all informations on the type <typeparamref name="T" /> and stores it in memory.
        /// </summary>
        private void PopulatePropertyInfos()
        {
            _propertyInfos = new Dictionary<string, (PropertyInfo propertyInfo, PropertyAttribute propertyAttribute, TypeConverter converter)>();
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.CanRead && p.CanWrite);
            foreach (var property in properties)
            {
                var propertyAtt = property.GetCustomAttribute<PropertyAttribute>(true);
                var converter = TypeDescriptor.GetConverter(property.PropertyType);
                if (propertyAtt != null)
                {
                    _propertyInfos.Add(propertyAtt.FieldName, (property, propertyAtt, converter));
                }
                else
                {
                    var ignoreAtt = property.GetCustomAttribute<IgnoreAttribute>(true);
                    if (ignoreAtt == null || !ignoreAtt.IgnoreOnImport)
                    {
                        _propertyInfos.Add(property.Name, (property, null, converter));
                    }
                }
            }
        }

        /// <summary>
        /// Traverses the file using the <see cref="Options" /> and triggers the <paramref name="mappingAction" /> for every data
        /// row found.
        /// </summary>
        /// <param name="mappingAction">The action to perform for every data row containing the the data row values.</param>
        /// <param name="fileUri">The absolute path to the file.</param>        
        /// <param name="cancellationToken">An optional token to cancel the operation by the caller.</param>
        private async Task ReadFileAsync(Action<string[]> mappingAction, string fileUri, CancellationToken cancellationToken = default)
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
            Results = new ConcurrentBag<IEnumerable<(long, T)>>();
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
                                if (Options.FirstReadLineContainsHeader)
                                {
                                    // this should be line containing the field names
                                    _fieldNames = line.Split(Options.Delimiter).ToArray();
                                    if (!_fieldNames.Any())
                                    {
                                        ThrowException(new InvalidOperationException("Could not read field names from file. Check delimiter option or file."));
                                    }
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
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ThrowException(new InvalidOperationException("Error during read operation. See inner exception for details.", ex));
            }
        }

        /// <summary>
        /// Starts and retrieves a task which will watch the <see cref="_incomingData" /> queue and call mapping of the data.
        /// </summary>
        /// <remarks>
        /// Ensure to check the <see cref="Options"/> in order to understand this method. Especially <see cref="ImporterOptions.MaxDegreeOfParallelism"/> is
        /// important to understand the behavior of this method.
        /// </remarks>
        /// <param name="progress">An optional progress to report progress back to the caller.</param>
        /// <param name="cancellationToken">An optional token to cancel the operation by the caller.</param>
        private Task StartQueueWatcher(IProgress<OperationProgress> progress = null, CancellationToken cancellationToken = default)
        {
            _runningMappers = 0;
            _handledRows = 0;
            // calculated how many lines should be handled by 1 worker process
            var itemsPerWorker = _dataRows.HasValue ? Options.ItemsPerWorker : 0;
            // start watching the queue on a new thread
            return Task.Run(
                async () =>
                {
                    var nextItems = new List<(long offset, string[] data)>();
                    while (!(cancellationToken.IsCancellationRequested || _incomingData.IsEmpty && _readingFinished))
                    {
                        if (_incomingData.TryDequeue(out var data))
                        {
                            // We've got one item out of the queue. We add it to the workload for the next
                            // task ...
                            nextItems.Add(data);
                            // ... and check if the calculated work-amount for each worker is reached.
                            if (nextItems.Count >= itemsPerWorker)
                            {
                                // start a new worker because it is enough work collected
                                MapItems(nextItems.AsEnumerable(), progress, cancellationToken);
                                nextItems = new List<(long offset, string[] data)>();
                                Interlocked.Increment(ref _runningMappers);
                            }
                        }
                        // wait now because we reached the limit of concurrent workers
                        while (_runningMappers >= Options.MaxDegreeOfParallelism)
                        {
                            await Task.Delay(1000, cancellationToken);
                        }
                    }
                    // wait until remaining workers are finished
                    Log($"All jobs queued. Waiting for them to finish.");
                    while (_runningMappers > 0)
                    {
                        await Task.Delay(1000, cancellationToken);
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
        /// Holds information about properties of the type <typeparamref name="T"/> so that expensive reflection-based
        /// processes are performed only one (ctor).
        /// </summary>
        /// <remarks>
        /// Contains the reflected property information, the associated <see cref="PropertyAttribute"/> if any is present and
        /// a <see cref="TypeConverter"/> for the reflected type.
        /// </remarks>
        private Dictionary<string, (PropertyInfo propertyInfo, PropertyAttribute propertyAttribute, TypeConverter converter)> PropertyInfos
        {
            get
            {
                if (_propertyInfos == null)
                {
                    PopulatePropertyInfos();
                }
                return _propertyInfos;
            }
        }

        /// <summary>
        /// Is used to collect items during a running import operation.
        /// </summary>
        private ConcurrentBag<IEnumerable<(long offset, T item)>> Results { get; set; }

        #endregion
    }
}
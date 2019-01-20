namespace codingfreaks.cfUtils.Logic.Csv
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;

    using Core.Extensions;
    using Core.Utilities;

    public class Importer
    {
        #region events

        public event EventHandler<EventArgs> EncodingDetected;

        public event EventHandler<EventArgs> ImportStarted;

        public event EventHandler<EventArgs> ImportFinished;

        public event EventHandler<EventArgs> ErrorOccured;

        public event EventHandler<EventArgs> ProgressChanged;

        public event EventHandler<EventArgs> FileCheckFailed;

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

        public async Task<bool> CheckFileAsync(string fileUri, IProgress<int> progress = null, CancellationToken cancellationToken = default)
        {
            try
            {
                using (var stream = File.OpenRead(fileUri))
                {
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Could not check file structure. See inner exception for details.", ex);
            }
        }

        public async Task<ImportResult<T>> ImportAsync<T>(string fileUri, IProgress<int> progress = null, CancellationToken cancellationToken = default)
        {
            CheckUtil.ThrowIfNullOrEmpty(() => fileUri);
            if (!Options.Valid)
            {
                throw new ApplicationException("Provided options are not valid!");
            }
            if (!File.Exists(fileUri))
            {
                throw new FileNotFoundException("Provided file not found.", fileUri);
            }
            var encoding = Encoding.Default;
            if (Options.AutoDetectEncoding)
            {
                // caller wants us to determine the encoding using the BOM automitically
                try
                {
                    encoding = FileUtils.GetEncoding(fileUri);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Error during encoding-auto-resolve. See inner exception for details.", ex);
                }
            }
            else
            {
                // caller passes the encoding in manually
                encoding = Options.Encoding;
            }
            var started = DateTimeOffset.Now;
            if (Options.CheckFileBeforeImport)
            {
                // caller wants us to check the file structure before we run the actual import process
                try
                {
                    var checkResult = await CheckFileAsync(fileUri, null, cancellationToken);
                    if (!checkResult)
                    {
                        return new ImportResult<T>(false, null, started, DateTimeOffset.Now);
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Error during operation. See inner exception for details.", ex);
                }
            }
            // we can start the import process now
            var currentLine = 0;
            var currentItem = 0;
            var skippedLines = 0;
            var headersPassed = false;
            var results = new List<T>();
            IEnumerable<string> fieldNames = null;
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
                                break;
                            }
                            currentLine++;
                            var line = await reader.ReadLineAsync();
                            if (line.IsNullOrEmpty())
                            {
                                // handle empty lines                                
                                if (Options.ThrowOnEmptyLines)
                                {
                                    throw new InvalidOperationException($"Line number {currentLine} is empty.");
                                }
                                // we can't do anything useful with an empty string so we go to the next line
                                continue;                                
                            }
                            if (regex != null)
                            {
                                // check every line for the passed regex
                                if (regex.IsMatch(line))
                                {
                                    // skip this line in any case because it matches the pattern
                                    continue;
                                }
                            }                        
                            if (!headersPassed)
                            {
                                // we are not in the data block yet because we are skipping lines or waiting for the header
                                if (Options.SkipLines > skippedLines)
                                {
                                    // skip this line because the caller wanted to do this for a certain amount of times
                                    skippedLines++;
                                    continue;                                    
                                }
                                if (Options.FirstReadedLineContainsHeader)
                                {
                                    // this should be line containing the field names
                                    fieldNames = line.Split(Options.Delimiter);
                                    if (!fieldNames.Any())
                                    {
                                        throw new InvalidOperationException("Could not read field names from file. Check delimiter option or file.");
                                    }
                                    headersPassed = true;
                                    continue;
                                }
                            }
                            // at this point the line should contain data
                            var items = line.Split(Options.Delimiter);

                        }
                    }
                }
                return new ImportResult<T>(true, results, started, DateTimeOffset.Now);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error during read operation. See inner exception for details.", ex);
            }
        }

        #endregion

        #region properties

        /// <summary>
        /// The options to use in this importer instance.
        /// </summary>
        public ImporterOptions Options { get; }

        #endregion
    }
}
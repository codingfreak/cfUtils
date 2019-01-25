namespace codingfreaks.cfUtils.Logic.Csv
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    using Core.Extensions;

    /// <summary>
    /// Defines options
    /// </summary>
    public class ImporterOptions
    {
        #region properties

        /// <summary>
        /// Defines if the logic should try to detect the file encoding using the BOM automatically.
        /// </summary>
        public bool AutoDetectEncoding { get; set; }

        /// <summary>
        /// Defines if the logic will check the file structure before the actual import starts.
        /// </summary>
        /// <remarks>
        /// <para>Defaults to <c>true</c>.</para>
        /// <para>If set to <c>false</c> every progress is reported absolutely only!</para>
        /// </remarks>
        public bool CheckFileBeforeImport { get; set; } = true;

        /// <summary>
        /// The culture to use when reading data and converting it.
        /// </summary>
        /// <remarks>
        /// Important for number- and date-conversion.
        /// </remarks>
        public CultureInfo Culture { get; set; }

        /// <summary>
        /// The styles for date and time conversions.
        /// </summary>
        public DateTimeStyles DateTimeStyles { get; set; } = DateTimeStyles.None;

        /// <summary>
        /// The delimiter char which is used to separate values in header and data rows.
        /// </summary>
        /// <remarks>
        /// Defaults to ';'.
        /// </remarks>
        public char Delimiter { get; set; } = ';';

        /// <summary>
        /// Defines if strings are wrapped by double quotes in the CSV.
        /// </summary>
        public bool DoubleQuotedStrings { get; set; }

        /// <summary>
        /// The encoding to use when reading in the file.
        /// </summary>
        /// <remarks>
        /// <para>Defaults to Encoding.Default.</para>
        /// <para>See <see cref="AutoDetectEncoding" />!</para>
        /// </remarks>
        public Encoding Encoding { get; set; } = Encoding.Default;

        /// <summary>
        /// Defines if the first readable line of the CSV contains the header names.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// </remarks>
        public bool FirstReadLineContainsHeader { get; set; } = true;

        /// <summary>
        /// Defines an optional regular expression which will lead to ignore matching lines during import.
        /// </summary>
        public string IgnoreLinesRegex { get; set; }

        /// <summary>
        /// An optional method passed in which gets called for logging messages.
        /// </summary>
        public Action<string> Logger { get; set; }

        /// <summary>
        /// Defines the maximum amount of concurrent worker processes during an import.
        /// </summary>
        /// <remarks>
        /// <para>Defaults to Environment.ProcessorCount.</para>
        /// </remarks>
        public uint MaxDegreeOfParallelism { get; set; } = (uint)Environment.ProcessorCount;

        /// <summary>
        /// 
        /// </summary>
        public uint ItemsPerWorker { get; set; }

        /// <summary>
        /// The styles for numeric conversions.
        /// </summary>
        public NumberStyles NumberStyles { get; set; } = NumberStyles.Any;

        /// <summary>
        /// A fixed amount of lines to skip before start interpretation of the file.
        /// </summary>
        public uint SkipLines { get; set; }

        /// <summary>
        /// Indicates if an empty line should be handled by throwing an exception.
        /// </summary>
        public bool ThrowOnEmptyLines { get; set; }

        /// <summary>
        /// Returns <c>true</c> if a basic check of this instance results in a valid combination of properties.
        /// </summary>
        public bool Valid
        {
            get
            {
                if (!AutoDetectEncoding && Encoding == null)
                {
                    // caller must provide an encoding if auto-detect is turned off
                    return false;
                }
                if (Delimiter == string.Empty.ToChar())
                {
                    // empty character is not valid
                    return false;
                }
                if (MaxDegreeOfParallelism > 1 && ItemsPerWorker == 0)
                {
                    // 
                    return false;
                }
                return true;
            }
        }

        #endregion
    }
}
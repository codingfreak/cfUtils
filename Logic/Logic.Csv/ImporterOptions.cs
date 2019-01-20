namespace codingfreaks.cfUtils.Logic.Csv
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Defines options
    /// </summary>
    public class ImporterOptions
    {
        #region properties

        public CultureInfo Culture { get; set; }

        public string Delimiter { get; set; }

        public bool DoubleQuotedStrings { get; set; }

        public Encoding Encoding { get; set; }

        public bool FirstReadedLineContainsHeader { get; set; }

        public string IgnoreLinesRegex { get; set; }

        public int MaxDegreeOfParallelism { get; set; }

        public int SkipLines { get; set; }

        public bool AutoDetectEncoding { get; set; }

        #endregion
    }
}
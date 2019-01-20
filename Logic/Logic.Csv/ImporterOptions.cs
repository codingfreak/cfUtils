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

        public CultureInfo Culture { get; set; }

        public char Delimiter { get; set; }

        public bool DoubleQuotedStrings { get; set; }        

        public bool ThrowOnEmptyLines { get; set; }

        public Encoding Encoding { get; set; }

        public bool FirstReadedLineContainsHeader { get; set; }

        public string IgnoreLinesRegex { get; set; }

        public uint MaxDegreeOfParallelism { get; set; }

        public uint SkipLines { get; set; }

        public bool CheckFileBeforeImport { get; set; }
        
        public bool AutoDetectEncoding { get; set; }
        
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
                return true;
            }
        }

        #endregion
    }
}
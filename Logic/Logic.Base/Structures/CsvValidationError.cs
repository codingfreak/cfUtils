using System;
using System.Linq;

namespace codingfreaks.cfUtils.Logic.Base.Structures
{
    /// <summary>
    /// Contains informations about a single CSV validation error.
    /// </summary>
    public class CsvValidationError
    {
        #region constructors and destructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="lineNumber">The number of the line in the file where the error occured.</param>
        /// <param name="errorText">An optional text for the error.</param>
        public CsvValidationError(int lineNumber, string errorText)
        {
            LineNumber = lineNumber;
            ErrorText = errorText;
        }

        #endregion

        #region properties

        /// <summary>
        /// The number of the line in the file where the error occured.
        /// </summary>
        public int LineNumber { get; private set; }

        /// <summary>
        /// An optional text for the error.
        /// </summary>
        public string ErrorText { get; private set; }

        #endregion
    }
}
namespace codingfreaks.cfUtils.Logic.Csv
{
    using System;
    using System.Linq;

    /// <summary>
    /// Is used to report progress information.
    /// </summary>
    public struct OperationProgress
    {
        #region constructors and destructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="currentLine">The absolute 1-based offset of the current item.</param>
        /// <param name="overallLines">The overall amount of lines.</param>
        public OperationProgress(long currentLine, long? overallLines = null)
        {
            CurrentLine = currentLine;
            if (overallLines.HasValue && overallLines.Value > 0)
            {
                Percentage = (int)(currentLine * 100 / overallLines.Value);
            }
            else
            {
                Percentage = null;
            }
        }

        #endregion

        #region properties

        /// <summary>
        /// The absolute 1-based offset of the current item.
        /// </summary>
        public long CurrentLine { get; }

        /// <summary>
        /// A percentage progress only available if complete amount of lines is given.
        /// </summary>
        public int? Percentage { get; }

        #endregion
    }
}
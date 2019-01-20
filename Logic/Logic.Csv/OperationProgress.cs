using System;
using System.Collections.Generic;
using System.Text;

namespace codingfreaks.cfUtils.Logic.Csv
{
    /// <summary>
    /// Is used to report progress informations.
    /// </summary>
    public struct OperationProgress
    {

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

        public long CurrentLine { get; }

        public int? Percentage { get; }

    }
}

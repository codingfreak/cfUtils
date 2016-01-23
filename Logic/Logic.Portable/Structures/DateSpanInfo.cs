namespace codingfreaks.cfUtils.Logic.Portable.Structures
{
    using System;

    /// <summary>
    /// Defines a data structure to store meta data of one calendar span in time.
    /// </summary>
    public struct DateSpanInfo
    {
        #region member vars

        /// <summary>
        /// The date of the last day of this span.
        /// </summary>
        public DateTime DateEnd;

        /// <summary>
        /// The date of the first day of this span.
        /// </summary>
        public DateTime DateStart;

        /// <summary>
        /// The offset of the span in a year.
        /// </summary>
        public int SpanNumber;

        #endregion

        #region methods

        /// <summary>
        /// Retrieves the string-representation of this type.
        /// </summary>
        /// <returns>The string representation.</returns>
        public override string ToString()
        {
            return $"#{SpanNumber:00} [{DateStart.ToString("d")} - {DateEnd.ToString("d")}]";
        }

        #endregion

        #region properties

        /// <summary>
        /// The amount of days.
        /// </summary>
        public int Days
        {
            get
            {
                return (int)DateEnd.Subtract(DateStart).TotalDays;
            }
        }

        /// <summary>
        /// The type this span is.
        /// </summary>
        public DateSpanType SpanType { get; set; }

        #endregion
    }
}
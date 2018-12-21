namespace codingfreaks.cfUtils.Logic.Core.Models
{
    using System;
    using System.Linq;

    /// <summary>
    /// Represents a type a <see cref="DateTimeSpanInfo" /> can be of.
    /// </summary>
    public enum DateSpanType
    {
        /// <summary>
        /// A free span.
        /// </summary>
        Free = 0,

        /// <summary>
        /// One calendar week.
        /// </summary>
        CalendarWeek = 1,

        /// <summary>
        /// One calendar month.
        /// </summary>
        CalendarMonth = 2,

        /// <summary>
        /// One quarter containing 3 months.
        /// </summary>
        CalendarQuarter = 3,

        /// <summary>
        /// One term containing 6 months.
        /// </summary>
        CalendarTerm = 4
    }
}
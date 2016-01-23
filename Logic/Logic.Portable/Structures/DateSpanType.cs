namespace s2.s2Utils.Logic.Portable.Structures
{
    /// <summary>
    /// Represents a type a <see cref="DateSpanInfo"/> can be of.
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
        /// One half year containing 6 months.
        /// </summary>
        CalendarHalfyear = 4
    }
}
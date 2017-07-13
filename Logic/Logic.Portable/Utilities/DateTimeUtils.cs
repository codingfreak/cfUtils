namespace codingfreaks.cfUtils.Logic.Portable.Utilities
{
    using System;
    using System.Linq;

    using Extensions;

    /// <summary>
    /// Provides logic for the <see cref="DateTime" /> type.
    /// </summary>
    public static class DateTimeUtils
    {
        #region methods

        /// <summary>
        /// Calculates the amount of calendar days inside a span of years.
        /// </summary>
        /// <param name="yearFrom">The first year of the span.</param>
        /// <param name="yearTo">The last year of the span.</param>
        /// <returns>The amount of calendar days.</returns>
        public static int GetDaysForYears(int yearFrom, int yearTo)
        {
            return Enumerable.Range(yearFrom, yearTo).Sum(i => DateTime.IsLeapYear(i) ? 366 : 365);
        }

        /// <summary>
        /// Calculates the amount of calendar days for a amount of past years
        /// to current year.
        /// </summary>
        /// <param name="intLastYears">The amount of years to look in the past including the current one.</param>
        /// <returns>The amount of calendar days.</returns>
        public static int GetDaysForYears(int intLastYears)
        {
            var nowYear = DateTime.Now.Year;
            return GetDaysForYears(nowYear - intLastYears, nowYear);
        }

        /// <summary>
        /// Calculates the last day of a month.
        /// </summary>
        /// <param name="year">The year of the month.</param>
        /// <param name="month">The month.</param>
        /// <returns>The date of the last day of the month.</returns>
        public static DateTime GetLastDayOfMonth(int year, int month)
        {
            var date = new DateTime(year, month, 28);
            while (date.Month == month)
            {
                date = date.AddDays(1);
            }
            return date.AddDays(-1).BeginOfDay();
        }

        /// <summary>
        /// Calculates the last day of a month.
        /// </summary>
        /// <param name="year">The year of the month.</param>
        /// <param name="month">The month.</param>
        /// <param name="utcOffset">The optional offset to keep for the result.</param>
        /// <returns>The date of the last day of the month.</returns>
        public static DateTimeOffset GetOffsetLastDayOfMonth(int year, int month, TimeSpan utcOffset = default(TimeSpan))
        {
            var date = new DateTimeOffset(year, month, 28, 0, 0, 0, utcOffset);
            while (date.Month == month)
            {
                date = date.AddDays(1);
            }
            return date.AddDays(-1).BeginOfDay();
        }

        /// <summary>
        /// Converts a timespan into a point in time on a given <paramref name="date" />.
        /// </summary>
        /// <param name="span">The span to add to the day.</param>
        /// <param name="date">The date on which the span should occur.</param>
        /// <returns>The time on the <paramref name="date" /> or the day after if the span is passed already.</returns>
        public static DateTime GetTimeByTimespan(this TimeSpan span, DateTime date)
        {
            var now = DateTime.Now;
            var desired = date.BeginOfDay().Add(span);
            return desired < now ? date.AddDays(1).BeginOfDay().Add(span) : desired;
        }

        /// <summary>
        /// Converts a timespan into a point in time on a given <paramref name="date" />.
        /// </summary>
        /// <param name="span">The span to add to the day.</param>
        /// <param name="date">The date on which the span should occur.</param>
        /// <returns>The time on the <paramref name="date" /> or the day after if the span is passed already.</returns>
        public static DateTimeOffset GetOffsetTimeByTimespan(this TimeSpan span, DateTimeOffset date)
        {
            var now = DateTimeOffset.Now;
            var desired = date.BeginOfDay().Add(span);
            return desired < now ? date.AddDays(1).BeginOfDay().Add(span) : desired;
        }

        #endregion
    }
}
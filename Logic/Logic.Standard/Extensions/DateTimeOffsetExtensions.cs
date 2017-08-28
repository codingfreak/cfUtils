namespace codingfreaks.cfUtils.Logic.Standard.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    using Structures;

    using Utilities;

    /// <summary>
    /// Provides extensions methods for the DateTimeOffset-caclulation area.
    /// </summary>
    public static class DateTimeOffsetExtensions
    {
        #region constants

        #region static fields

        /// <summary>
        /// The average amount of days per calendar year.
        /// </summary>
        public static double DaysPerYear = 365.2425;

        #endregion

        #endregion

        #region methods

        /// <summary>
        /// Retrieve the first posible moment of a given <paramref name="date" />.
        /// </summary>
        /// <param name="date">The original date.</param>
        /// <returns>The date representing the first moment.</returns>
        public static DateTimeOffset BeginOfDay(this DateTimeOffset date)
        {
            return new DateTimeOffset(date.Year, date.Month, date.Day, 0, 0, 0, date.Offset);
        }

        /// <summary>
        /// Retrieves the start of the half year in which the given <paramref name="date" /> lays.
        /// </summary>
        /// <param name="date">The original date.</param>
        /// <returns>The starting time point of the correct half year.</returns>
        public static DateTimeOffset BeginOfHalfYear(this DateTimeOffset date)
        {
            return date.GetCalendarHalfYearInfo().DateStart;
        }

        /// <summary>
        /// Retrieves the start of the month in which the given <paramref name="date" /> lays.
        /// </summary>
        /// <param name="date">The original date.</param>
        /// <returns>The starting time point of the correct month.</returns>
        public static DateTimeOffset BeginOfMonth(this DateTimeOffset date)
        {
            return date.GetCalendarMonthInfo().DateStart;
        }

        /// <summary>
        /// Retrieves the start of the quarter in which the given <paramref name="date" /> lays.
        /// </summary>
        /// <param name="date">The original date.</param>
        /// <returns>The starting time point of the correct quarter.</returns>
        public static DateTimeOffset BeginOfQuarter(this DateTimeOffset date)
        {
            return date.GetCalendarQuarterInfo().DateStart;
        }

        /// <summary>
        /// Retrieve the date of the week start for a given <paramref name="date" />.
        /// </summary>
        /// <param name="date">The original date.</param>
        /// <param name="culture">The culture to use or <c>null</c> if <see cref="CultureInfo.CurrentUICulture" /> should be taken.</param>
        /// <returns>The date representing the first day of the week.</returns>
        public static DateTimeOffset BeginOfWeek(this DateTimeOffset date, CultureInfo culture = null)
        {
            if (culture == null)
            {
                culture = CultureInfo.CurrentUICulture;
            }
            var dateToTest = date;
            while (dateToTest.DayOfWeek != culture.DateTimeFormat.FirstDayOfWeek)
            {
                dateToTest = dateToTest.AddDays(-1);
            }
            return dateToTest.BeginOfDay();
        }

        /// <summary>
        /// Retrieves the start of the year in which the given <paramref name="date" /> lays.
        /// </summary>
        /// <param name="date">The original date.</param>
        /// <returns>The starting time point of the correct year.</returns>
        public static DateTimeOffset BeginOfYear(this DateTimeOffset date)
        {
            return new DateTimeOffset(date.Year, 1, 1, 0, 0, 0, date.Offset);
        }

        /// <summary>
        /// Retrieves the given <paramref name="date" /> with the new <paramref name="newYear" />.
        /// </summary>
        /// <param name="date">The original date.</param>
        /// <param name="newYear">The new year value.</param>
        /// <returns>The new date.</returns>
        public static DateTimeOffset ChangeYear(this DateTimeOffset date, int newYear)
        {
            return new DateTimeOffset(newYear, date.Month, date.Day, date.Hour, date.Minute, date.Second, date.Millisecond, date.Offset);
        }

        /// <summary>
        /// Retrieve the last posible moment of a given <paramref name="date" />.
        /// </summary>
        /// <param name="date">The original date.</param>
        /// <returns>The date representing the last moment.</returns>
        public static DateTimeOffset EndOfDay(this DateTimeOffset date)
        {
            return new DateTimeOffset(date.Year, date.Month, date.Day, 23, 59, 59, 999, date.Offset);
        }

        /// <summary>
        /// Retrieves the end of the half year in which the given <paramref name="date" /> lays.
        /// </summary>
        /// <param name="date">The original date.</param>
        /// <returns>The end time point of the correct half year.</returns>
        public static DateTimeOffset EndOfHalfYear(this DateTimeOffset date)
        {
            return date.GetCalendarHalfYearInfo().DateEnd;
        }

        /// <summary>
        /// Retrieves the start of the month in which the given <paramref name="date" /> lays.
        /// </summary>
        /// <param name="date">The original date.</param>
        /// <returns>The starting time point of the correct month.</returns>
        public static DateTimeOffset EndOfMonth(this DateTimeOffset date)
        {
            return date.GetCalendarMonthInfo().DateEnd;
        }

        /// <summary>
        /// Retrieves the end of the quarter in which the given <paramref name="date" /> lays.
        /// </summary>
        /// <param name="date">The original date.</param>
        /// <returns>The end time point of the correct quarter.</returns>
        public static DateTimeOffset EndOfQuarter(this DateTimeOffset date)
        {
            return date.GetCalendarQuarterInfo().DateEnd;
        }

        /// <summary>
        /// Retrieve the date of the week end for a given <paramref name="date" />.
        /// </summary>
        /// <param name="date">The original date.</param>
        /// <param name="culture">The culture to use or <c>null</c> if <see cref="CultureInfo.CurrentUICulture" /> should be taken.</param>
        /// <returns>The date representing the last day of the week.</returns>
        public static DateTimeOffset EndOfWeek(this DateTimeOffset date, CultureInfo culture = null)
        {
            if (culture == null)
            {
                culture = CultureInfo.CurrentUICulture;
            }
            var dateToTest = date;
            while (dateToTest.DayOfWeek != culture.DateTimeFormat.FirstDayOfWeek)
            {
                dateToTest = dateToTest.AddDays(1);
            }
            return dateToTest.AddDays(-1).EndOfDay();
        }

        /// <summary>
        /// Retrieves the start of the year in which the given <paramref name="date" /> lays.
        /// </summary>
        /// <param name="date">The original date.</param>
        /// <returns>The starting time point of the correct year.</returns>
        public static DateTimeOffset EndOfYear(this DateTimeOffset date)
        {
            return new DateTimeOffset(date.Year, 12, 31, 23, 59, 59, 999, date.Offset);
        }

        /// <summary>
        /// Retrieves informations on the calendar half year of a given <paramref name="current" /> date.
        /// </summary>
        /// <param name="current">The date to extend.</param>
        /// <returns>The calendar half year information for the date.</returns>
        public static DateTimeOffsetSpanInfo GetCalendarHalfYearInfo(this DateTimeOffset current)
        {
            return current.Year.GetOffsetCalendarHalfYearsForYear(current.Offset).First(w => w.SpanNumber == current.DateTime.GetHalfYearNumber());
        }

        /// <summary>
        /// Retrieves informations on the calendar month of a given <paramref name="current" /> date.
        /// </summary>
        /// <param name="current">The date to extend.</param>
        /// <returns>The calendar month information for the date.</returns>
        public static DateTimeOffsetSpanInfo GetCalendarMonthInfo(this DateTimeOffset current)
        {
            return current.Year.GetOffsetCalendarMonthsForYear().First(w => w.SpanNumber == current.Month);
        }

        /// <summary>
        /// Retrieves informations on the calendar quarter of a given <paramref name="current" /> date.
        /// </summary>
        /// <param name="current">The date to extend.</param>
        /// <returns>The calendar quarter information for the date.</returns>
        public static DateTimeOffsetSpanInfo GetCalendarQuarterInfo(this DateTimeOffset current)
        {
            return current.Year.GetOffsetCalendarQuartersForYear(current.Offset).First(w => w.SpanNumber == current.DateTime.GetQuarterNumber());
        }

        /// <summary>
        /// Retrieves informations on the calendar week of a given <paramref name="current" /> date.
        /// </summary>
        /// <param name="current">The date to extend.</param>
        /// <returns>The calendar week information for the date.</returns>
        public static DateTimeOffsetSpanInfo GetCalendarWeekInfo(this DateTimeOffset current)
        {
            return current.Year.GetOffsetCalendarWeeksForYear(null, current.Offset).First(w => w.SpanNumber == current.DateTime.GetWeekNumber());
        }

        /// <summary>
        /// Returns a list of both half years of a given year.
        /// </summary>
        /// <param name="year">The year to examine.</param>
        /// <param name="utcOffset">The optional offset to keep for the result.</param>
        /// <returns>A list of meta-info objects describing the half-years.</returns>
        public static IEnumerable<DateTimeOffsetSpanInfo> GetOffsetCalendarHalfYearsForYear(this int year, TimeSpan utcOffset = default(TimeSpan))
        {
            var result = new List<DateTimeOffsetSpanInfo>();
            var currentHalfyear = 0;
            // 1. Get first and last day
            var startDate = new DateTimeOffset(year, 1, 1, 0, 0, 0, utcOffset);
            var endDate = new DateTimeOffset(year + 1, 1, 1, 0, 0, 0, utcOffset);
            // 2. Collect all halfyear-informations
            while (startDate < endDate)
            {
                result.Add(
                    new DateTimeOffsetSpanInfo
                    {
                        SpanType = DateSpanType.CalendarHalfyear,
                        SpanNumber = ++currentHalfyear,
                        DateStart = startDate,
                        DateEnd = DateTimeUtils.GetOffsetLastDayOfMonth(year, startDate.Month + 5, utcOffset).EndOfDay()
                    });
                startDate = startDate.AddMonths(6);
            }
            return result;
        }

        /// <summary>
        /// Returns a list of all calendar months of a given year.
        /// </summary>
        /// <param name="year">The year to examine.</param>
        /// <param name="utcOffset">The optional offset to keep for the result.</param>
        /// <returns>A list of meta-info objects describing the months.</returns>
        public static IEnumerable<DateTimeOffsetSpanInfo> GetOffsetCalendarMonthsForYear(this int year, TimeSpan utcOffset = default(TimeSpan))
        {
            var result = new List<DateTimeOffsetSpanInfo>();
            var currentMonth = 0;
            // 1. Get first and last day
            var startDate = new DateTimeOffset(year, 1, 1, 0, 0, 0, utcOffset);
            var endDate = new DateTimeOffset(year + 1, 1, 1, 0, 0, 0, utcOffset);
            // 2. Collect all week-informations
            while (startDate < endDate)
            {
                result.Add(
                    new DateTimeOffsetSpanInfo
                    {
                        SpanType = DateSpanType.CalendarMonth,
                        SpanNumber = ++currentMonth,
                        DateStart = startDate,
                        DateEnd = DateTimeUtils.GetOffsetLastDayOfMonth(year, currentMonth, utcOffset)
                    });
                startDate = startDate.AddMonths(1);
            }
            return result;
        }

        /// <summary>
        /// Returns a list of all calendar quarters of a given year.
        /// </summary>
        /// <param name="year">The year to examine.</param>
        /// <param name="utcOffset">The optional offset to keep for the result.</param>
        /// <returns>A list of meta-info objects describing the quarters.</returns>
        public static IEnumerable<DateTimeOffsetSpanInfo> GetOffsetCalendarQuartersForYear(this int year, TimeSpan utcOffset = default(TimeSpan))
        {
            var result = new List<DateTimeOffsetSpanInfo>();
            var currentQuarter = 0;
            // 1. Get first and last day
            var startDate = new DateTimeOffset(year, 1, 1, 0, 0, 0, utcOffset);
            var endDate = new DateTimeOffset(year + 1, 1, 1, 0, 0, 0, utcOffset);
            // 2. Collect all week-informations
            while (startDate < endDate)
            {
                result.Add(
                    new DateTimeOffsetSpanInfo
                    {
                        SpanType = DateSpanType.CalendarQuarter,
                        SpanNumber = ++currentQuarter,
                        DateStart = startDate,
                        DateEnd = DateTimeUtils.GetOffsetLastDayOfMonth(year, startDate.Month + 2, utcOffset).EndOfDay()
                    });
                startDate = startDate.AddMonths(3);
            }
            return result;
        }

        /// <summary>
        /// Returns a list of all calendar weeks of a given year.
        /// </summary>
        /// <param name="year">The year to examine.</param>
        /// <param name="culture">The culture to use or <c>null</c> if <see cref="CultureInfo.CurrentUICulture" /> should be taken.</param>
        /// <param name="utcOffset">The optional offset to keep for the result.</param>
        /// <returns>A list of meta-info objects describing the weeks.</returns>
        public static IEnumerable<DateTimeOffsetSpanInfo> GetOffsetCalendarWeeksForYear(this int year, CultureInfo culture = null, TimeSpan utcOffset = default(TimeSpan))
        {
            if (culture == null)
            {
                culture = CultureInfo.CurrentUICulture;
            }
            var result = new List<DateTimeOffsetSpanInfo>();
            var currentWeek = 0;
            // 1. Get first and last day
            var startDate = year.GetOffsetFirstDayOfYear(culture, utcOffset);
            var endDate = (year + 1).GetOffsetFirstDayOfYear(culture, utcOffset);
            // 2. Collect all week-informations
            while (startDate < endDate)
            {
                result.Add(
                    new DateTimeOffsetSpanInfo
                    {
                        SpanType = DateSpanType.CalendarWeek,
                        SpanNumber = ++currentWeek,
                        DateStart = startDate,
                        DateEnd = startDate.AddDays(6).EndOfDay()
                    });
                startDate = startDate.AddDays(7);
            }
            return result;
        }

        /// <summary>
        /// Retrieves the very first day of a year depending of the <see cref="CalendarWeekRule" /> of the given
        /// <paramref name="culture" />.
        /// </summary>
        /// <param name="year">The year to examine.</param>
        /// <param name="culture">The culture to use or <c>null</c> if <see cref="CultureInfo.CurrentUICulture" /> should be taken.</param>
        /// <param name="utcOffset">The optional offset to keep for the result.</param>
        /// <returns>The first day of the year.</returns>
        public static DateTimeOffset GetOffsetFirstDayOfYear(this int year, CultureInfo culture = null, TimeSpan utcOffset = default(TimeSpan))
        {
            if (culture == null)
            {
                culture = CultureInfo.CurrentUICulture;
            }
            var startDate = new DateTimeOffset(year, 1, 1, 0, 0, 0, utcOffset).AddDays(-10);
            var firstDayOfWeek = culture.DateTimeFormat.FirstDayOfWeek;
            // 1. find the start-day
            while (startDate.GetWeekNumber(culture) != 1)
            {
                startDate = startDate.AddDays(1);
            }
            // 2. Go back to nearest start-date
            while (startDate.DayOfWeek != firstDayOfWeek)
            {
                startDate = startDate.AddDays(-1);
            }
            return startDate.BeginOfDay();
        }

        /// <summary>
        /// Returns the calendar week of a given date.
        /// </summary>
        /// <param name="date">The date to examine.</param>
        /// <param name="culture">The culture to use or <c>null</c> if <see cref="CultureInfo.CurrentUICulture" /> should be taken.</param>
        /// <returns>The number of the calendar week.</returns>
        public static int GetWeekNumber(this DateTimeOffset date, CultureInfo culture = null)
        {
            if (culture == null)
            {
                culture = CultureInfo.CurrentUICulture;
            }
            var calendar = culture.Calendar;
            var weekRule = culture.DateTimeFormat.CalendarWeekRule;
            var firstDayOfWeek = culture.DateTimeFormat.FirstDayOfWeek;
            return calendar.GetWeekOfYear(date.DateTime, weekRule, firstDayOfWeek);
        }

        /// <summary>
        /// Calculates the years which are gone between the <paramref name="dateInFuture" /> and the given
        /// <paramref name="dateInPast" />.
        /// </summary>
        /// <remarks>
        /// Take this method if you expect <paramref name="dateInFuture" /> to be bigger than <paramref name="dateInPast" />.
        /// </remarks>
        /// <param name="dateInFuture">The date which is the younger part.</param>
        /// <param name="dateInPast">
        /// The date which is the older part. Will be used as <see cref="DateTime.Now" /> if <c>null</c>
        /// is given.
        /// </param>
        /// <returns>The difference in total years or <c>null</c> if <paramref name="dateInFuture" /> is <c>null</c>.</returns>
        public static int? GetYearsDifference(this DateTimeOffset? dateInFuture, DateTimeOffset? dateInPast = null)
        {
            if (dateInFuture == null)
            {
                // we cannot calculate because future date is null already
                return null;
            }
            return GetYearsDifference(dateInFuture.Value, dateInPast);
        }

        /// <summary>
        /// Calculates the years which are gone between the <paramref name="dateInFuture" /> and the given
        /// <paramref name="dateInPast" />.
        /// </summary>
        /// <remarks>
        /// Take this method if you expect <paramref name="dateInFuture" /> to be bigger than <paramref name="dateInPast" />.
        /// </remarks>
        /// <param name="dateInFuture">The date which is the younger part.</param>
        /// <param name="dateInPast">
        /// The date which is the older part. Will be used as <see cref="DateTime.Now" /> if <c>null</c>
        /// is given.
        /// </param>
        /// <returns>The difference in total years or <c>null</c> if <paramref name="dateInFuture" /> is <c>null</c>.</returns>
        public static int? GetYearsDifference(this DateTimeOffset dateInFuture, DateTimeOffset? dateInPast = null)
        {
            if (!dateInPast.HasValue)
            {
                // if no past-date is given, take current date
                dateInPast = DateTimeOffset.Now;
            }
            var totalDays = dateInFuture.Subtract(dateInPast.Value).TotalDays;
            if (totalDays.Equals(365d))
            {
                // this would end up in rounding issues if we would calculate it.
                return 1;
            }
            return (int)(totalDays / DaysPerYear);
        }

        /// <summary>
        /// Calculates the years which are gone between the <paramref name="dateInPast" /> and the given
        /// <paramref name="dateInFuture" />.
        /// </summary>
        /// <remarks>
        /// Take this method if you expect <paramref name="dateInPast" /> to be bigger than <paramref name="dateInFuture" />.
        /// </remarks>
        /// <param name="dateInPast">The date which is the younger part.</param>
        /// <param name="dateInFuture">
        /// The date which is the older part. Will be used as <see cref="DateTime.Now" /> if <c>null</c>
        /// is given.
        /// </param>
        /// <returns>The difference in total years or <c>null</c> if <paramref name="dateInPast" /> is <c>null</c>.</returns>
        public static int? GetYearsTilDifference(this DateTimeOffset? dateInPast, DateTimeOffset? dateInFuture = null)
        {
            if (dateInPast == null)
            {
                // we cannot calculate because future date is null already
                return null;
            }
            return GetYearsTilDifference(dateInPast.Value, dateInFuture);
        }

        /// <summary>
        /// Calculates the years which are gone between the <paramref name="dateInPast" /> and the given
        /// <paramref name="dateInFuture" />.
        /// </summary>
        /// <remarks>
        /// Take this method if you expect <paramref name="dateInPast" /> to be bigger than <paramref name="dateInFuture" />.
        /// </remarks>
        /// <param name="dateInPast">The date which is the younger part.</param>
        /// <param name="dateInFuture">
        /// The date which is the older part. Will be used as <see cref="DateTime.Now" /> if <c>null</c>
        /// is given.
        /// </param>
        /// <returns>The difference in total years or <c>null</c> if <paramref name="dateInPast" /> is <c>null</c>.</returns>
        public static int? GetYearsTilDifference(this DateTimeOffset dateInPast, DateTimeOffset? dateInFuture = null)
        {
            if (!dateInFuture.HasValue)
            {
                // if no future-date is given, take current date
                dateInFuture = DateTimeOffset.Now;
            }
            return (int)(dateInFuture.Value.Subtract(dateInPast).TotalDays / DaysPerYear);
        }

        /// <summary>
        /// Returns the decimal represenation of hour and minute of a given <paramref name="date" />.
        /// </summary>
        /// <param name="date">The date to convert.</param>
        /// <returns>A decimal value. For 16:30 it will return 16.5 e.g.</returns>
        public static decimal ToDecimalTime(this DateTimeOffset date)
        {
            var minute = Math.Round((decimal)date.Minute / 60, 2);
            return date.Hour + minute;
        }

        /// <summary>
        /// Retrieves the system long date string for the given nullable <paramref name="date" />.
        /// </summary>
        /// <param name="date">The date or <c>null</c>.</param>
        /// <returns>The formatted date or <see cref="string.Empty" /> if the <paramref name="date" /> is <c>null</c>.</returns>
        public static string ToLongDateString(this DateTimeOffset? date)
        {
            return date?.ToString("D") ?? string.Empty;
        }

        /// <summary>
        /// Retrieves the system long time string for the given nullable <paramref name="date" />.
        /// </summary>
        /// <param name="date">The date or <c>null</c>.</param>
        /// <returns>The formatted date or <see cref="string.Empty" /> if the <paramref name="date" /> is <c>null</c>.</returns>
        public static string ToLongTimeString(this DateTimeOffset? date)
        {
            return date?.ToString("T") ?? string.Empty;
        }

        /// <summary>
        /// Retrieves the system short date string for the given nullable <paramref name="date" />.
        /// </summary>
        /// <param name="date">The date or <c>null</c>.</param>
        /// <returns>The formatted date or <see cref="string.Empty" /> if the <paramref name="date" /> is <c>null</c>.</returns>
        public static string ToShortDateString(this DateTimeOffset? date)
        {
            return date?.ToString("d") ?? string.Empty;
        }

        /// <summary>
        /// Retrieves the system short time string for the given nullable <paramref name="date" />.
        /// </summary>
        /// <param name="date">The date or <c>null</c>.</param>
        /// <returns>The formatted date or <see cref="string.Empty" /> if the <paramref name="date" /> is <c>null</c>.</returns>
        public static string ToShortTimeString(this DateTimeOffset? date)
        {
            return date?.ToString("t") ?? string.Empty;
        }

        /// <summary>
        /// Retrieves the string representation for a given nullable <paramref name="date" /> depending on a given
        /// <paramref name="format" />.
        /// </summary>
        /// <param name="date">The date or <c>null</c>.</param>
        /// <param name="format">The format pattern to use.</param>
        /// <returns>The formatted date or <see cref="string.Empty" /> if the <paramref name="date" /> is <c>null</c>.</returns>
        public static string ToString(this DateTimeOffset? date, string format)
        {
            return date?.ToString(format) ?? string.Empty;
        }

        /// <summary>
        /// Retrieves the string representation for a given nullable <paramref name="date" /> depending on a given
        /// <paramref name="format" />.
        /// </summary>
        /// <param name="date">The date or <c>null</c>.</param>
        /// <param name="format">The format pattern to use.</param>
        /// <param name="provider">The format provider to use.</param>
        /// <returns>The formatted date or <see cref="string.Empty" /> if the <paramref name="date" /> is <c>null</c>.</returns>
        public static string ToString(this DateTimeOffset? date, string format, IFormatProvider provider)
        {
            return date?.ToString(format, provider) ?? string.Empty;
        }

        #endregion
    }
}
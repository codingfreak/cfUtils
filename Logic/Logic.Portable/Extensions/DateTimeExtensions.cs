namespace codingfreaks.cfUtils.Logic.Portable.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    using Structures;

    using Utilities;

    /// <summary>
    /// Provides extensions methods for the DateTime-caclulation area.
    /// </summary>
    public static class DateTimeExtensions
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
        public static DateTime BeginOfDay(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
        }

        /// <summary>
        /// Retrieves the start of the half year in which the given <paramref name="date" /> lays.
        /// </summary>
        /// <param name="date">The original date.</param>
        /// <returns>The starting time point of the correct half year.</returns>
        public static DateTime BeginOfHalfYear(this DateTime date)
        {
            return date.GetCalendarHalfYearInfo().DateStart;
        }

        /// <summary>
        /// Retrieves the start of the month in which the given <paramref name="date" /> lays.
        /// </summary>
        /// <param name="date">The original date.</param>
        /// <returns>The starting time point of the correct month.</returns>
        public static DateTime BeginOfMonth(this DateTime date)
        {
            return date.GetCalendarMonthInfo().DateStart;
        }

        /// <summary>
        /// Retrieves the start of the quarter in which the given <paramref name="date" /> lays.
        /// </summary>
        /// <param name="date">The original date.</param>
        /// <returns>The starting time point of the correct quarter.</returns>
        public static DateTime BeginOfQuarter(this DateTime date)
        {
            return date.GetCalendarQuarterInfo().DateStart;
        }

        /// <summary>
        /// Retrieve the date of the week start for a given <paramref name="date" />.
        /// </summary>
        /// <param name="date">The original date.</param>
        /// <param name="culture">The culture to use or <c>null</c> if <see cref="CultureInfo.CurrentUICulture" /> should be taken.</param>
        /// <returns>The date representing the first day of the week.</returns>
        public static DateTime BeginOfWeek(this DateTime date, CultureInfo culture = null)
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
        public static DateTime BeginOfYear(this DateTime date)
        {
            return new DateTime(date.Year, 1, 1, 0, 0, 0);
        }

        /// <summary>
        /// Retrieves the given <paramref name="date" /> with the new <paramref name="newYear" />.
        /// </summary>
        /// <param name="date">The original date.</param>
        /// <param name="newYear">The new year value.</param>
        /// <returns>The new date.</returns>
        public static DateTime ChangeYear(this DateTime date, int newYear)
        {
            return new DateTime(newYear, date.Month, date.Day, date.Hour, date.Minute, date.Second, date.Millisecond);
        }

        /// <summary>
        /// Retrieve the last posible moment of a given <paramref name="date" />.
        /// </summary>
        /// <param name="date">The original date.</param>
        /// <returns>The date representing the last moment.</returns>
        public static DateTime EndOfDay(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 23, 59, 59, 999);
        }

        /// <summary>
        /// Retrieves the end of the half year in which the given <paramref name="date" /> lays.
        /// </summary>
        /// <param name="date">The original date.</param>
        /// <returns>The end time point of the correct half year.</returns>
        public static DateTime EndOfHalfYear(this DateTime date)
        {
            return date.GetCalendarHalfYearInfo().DateEnd;
        }

        /// <summary>
        /// Retrieves the start of the month in which the given <paramref name="date" /> lays.
        /// </summary>
        /// <param name="date">The original date.</param>
        /// <returns>The starting time point of the correct month.</returns>
        public static DateTime EndOfMonth(this DateTime date)
        {
            return date.GetCalendarMonthInfo().DateEnd;
        }

        /// <summary>
        /// Retrieve the date of the week end for a given <paramref name="date" />.
        /// </summary>
        /// <param name="date">The original date.</param>
        /// <param name="culture">The culture to use or <c>null</c> if <see cref="CultureInfo.CurrentUICulture" /> should be taken.</param>
        /// <returns>The date representing the last day of the week.</returns>
        public static DateTime EndOfWeek(this DateTime date, CultureInfo culture = null)
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
        public static DateTime EndOfYear(this DateTime date)
        {
            return new DateTime(date.Year, 12, 31, 23, 59, 59, 999);
        }

        /// <summary>
        /// Retrieves the end of the quarter in which the given <paramref name="date" /> lays.
        /// </summary>
        /// <param name="date">The original date.</param>
        /// <returns>The end time point of the correct quarter.</returns>
        public static DateTime EnfOfQuarter(this DateTime date)
        {
            return date.GetCalendarQuarterInfo().DateEnd;
        }

        /// <summary>
        /// Retrieves informations on the calendar half year of a given <paramref name="current" /> date.
        /// </summary>
        /// <param name="current">The date to extend.</param>
        /// <returns>The calendar half year information for the date.</returns>
        public static DateTimeSpanInfo GetCalendarHalfYearInfo(this DateTime current)
        {
            return current.Year.GetCalendarHalfYearsForYear().First(w => w.SpanNumber == current.GetHalfYearNumber());
        }

        /// <summary>
        /// Returns a list of both half years of a given year.
        /// </summary>
        /// <param name="year">The year to examine.</param>
        /// <returns>A list of meta-info objects describing the half-years.</returns>
        public static IEnumerable<DateTimeSpanInfo> GetCalendarHalfYearsForYear(this int year)
        {
            var result = new List<DateTimeSpanInfo>();
            var currentHalfyear = 0;
            // 1. Get first and last day
            var startDate = new DateTime(year, 1, 1);
            var endDate = new DateTime(year + 1, 1, 1);
            // 2. Collect all halfyear-informations
            while (startDate < endDate)
            {
                result.Add(
                    new DateTimeSpanInfo
                    {
                        SpanType = DateSpanType.CalendarHalfyear,
                        SpanNumber = ++currentHalfyear,
                        DateStart = startDate,
                        DateEnd = DateTimeUtils.GetLastDayOfMonth(year, startDate.Month + 5)
                    });
                startDate = startDate.AddMonths(6);
            }
            return result;
        }

        /// <summary>
        /// Retrieves informations on the calendar month of a given <paramref name="current" /> date.
        /// </summary>
        /// <param name="current">The date to extend.</param>
        /// <returns>The calendar month information for the date.</returns>
        public static DateTimeSpanInfo GetCalendarMonthInfo(this DateTime current)
        {
            return current.Year.GetCalendarMonthsForYear().First(w => w.SpanNumber == current.Month);
        }

        /// <summary>
        /// Returns a list of all calendar months of a given year.
        /// </summary>
        /// <param name="year">The year to examine.</param>
        /// <returns>A list of meta-info objects describing the months.</returns>
        public static IEnumerable<DateTimeSpanInfo> GetCalendarMonthsForYear(this int year)
        {
            var result = new List<DateTimeSpanInfo>();
            var currentMonth = 0;
            // 1. Get first and last day
            var startDate = new DateTime(year, 1, 1);
            var endDate = new DateTime(year + 1, 1, 1);
            // 2. Collect all week-informations
            while (startDate < endDate)
            {
                result.Add(
                    new DateTimeSpanInfo
                    {
                        SpanType = DateSpanType.CalendarMonth,
                        SpanNumber = ++currentMonth,
                        DateStart = startDate,
                        DateEnd = DateTimeUtils.GetLastDayOfMonth(year, currentMonth)
                    });
                startDate = startDate.AddMonths(1);
            }
            return result;
        }

        /// <summary>
        /// Retrieves informations on the calendar quarter of a given <paramref name="current" /> date.
        /// </summary>
        /// <param name="current">The date to extend.</param>
        /// <returns>The calendar quarter information for the date.</returns>
        public static DateTimeSpanInfo GetCalendarQuarterInfo(this DateTime current)
        {
            return current.Year.GetCalendarQuartersForYear().First(w => w.SpanNumber == current.GetQuarterNumber());
        }

        /// <summary>
        /// Returns a list of all calendar quarters of a given year.
        /// </summary>
        /// <param name="year">The year to examine.</param>
        /// <returns>A list of meta-info objects describing the quarters.</returns>
        public static IEnumerable<DateTimeSpanInfo> GetCalendarQuartersForYear(this int year)
        {
            var result = new List<DateTimeSpanInfo>();
            var currentQuarter = 0;
            // 1. Get first and last day
            var startDate = new DateTime(year, 1, 1);
            var endDate = new DateTime(year + 1, 1, 1);
            // 2. Collect all week-informations
            while (startDate < endDate)
            {
                result.Add(
                    new DateTimeSpanInfo
                    {
                        SpanType = DateSpanType.CalendarQuarter,
                        SpanNumber = ++currentQuarter,
                        DateStart = startDate,
                        DateEnd = DateTimeUtils.GetLastDayOfMonth(year, currentQuarter + 2)
                    });
                startDate = startDate.AddMonths(3);
            }
            return result;
        }

        /// <summary>
        /// Retrieves informations on the calendar week of a given <paramref name="current" /> date.
        /// </summary>
        /// <param name="current">The date to extend.</param>
        /// <returns>The calendar week information for the date.</returns>
        public static DateTimeSpanInfo GetCalendarWeekInfo(this DateTime current)
        {
            return current.Year.GetCalendarWeeksForYear().First(w => w.SpanNumber == current.GetWeekNumber());
        }

        /// <summary>
        /// Returns a list of all calendar weeks of a given year.
        /// </summary>
        /// <param name="year">The year to examine.</param>
        /// <param name="culture">The culture to use or <c>null</c> if <see cref="CultureInfo.CurrentUICulture" /> should be taken.</param>
        /// <returns>A list of meta-info objects describing the weeks.</returns>
        public static IEnumerable<DateTimeSpanInfo> GetCalendarWeeksForYear(this int year, CultureInfo culture = null)
        {
            if (culture == null)
            {
                culture = CultureInfo.CurrentUICulture;
            }
            var result = new List<DateTimeSpanInfo>();
            var currentWeek = 0;
            // 1. Get first and last day
            var startDate = year.GetFirstDayOfYear(culture);
            var endDate = (year + 1).GetFirstDayOfYear(culture);
            // 2. Collect all week-informations
            while (startDate < endDate)
            {
                result.Add(
                    new DateTimeSpanInfo
                    {
                        SpanType = DateSpanType.CalendarWeek,
                        SpanNumber = ++currentWeek,
                        DateStart = startDate,
                        DateEnd = startDate.AddDays(6)
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
        /// <returns>The first day of the year.</returns>
        public static DateTime GetFirstDayOfYear(this int year, CultureInfo culture = null)
        {
            if (culture == null)
            {
                culture = CultureInfo.CurrentUICulture;
            }
            var startDate = new DateTime(year, 1, 1).AddDays(-10);
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
        /// Returns the calendar half year of a given date.
        /// </summary>
        /// <param name="date">The date to examine.</param>
        /// <returns>The number of the calendar half year.</returns>
        public static int GetHalfYearNumber(this DateTime date)
        {
            return date.Month < 7 ? 1 : 2;
        }

        /// <summary>
        /// Returns the calendar quarter of a given date.
        /// </summary>
        /// <param name="date">The date to examine.</param>
        /// <returns>The number of the calendar quarter.</returns>
        public static int GetQuarterNumber(this DateTime date)
        {
            return (int)Math.Ceiling((double)date.Month / 3);
        }

        /// <summary>
        /// Returns a string which contains the relative date span from the dtmBase-view. It returns words like 'tomorrow',
        /// 'yesterday' ...
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        /// <listheader>
        /// You need the following keys in your resource-file:
        /// </listheader>
        /// <item>
        /// <term>RelativeDateSpanToday</term><description>Today</description>
        /// </item>
        /// <item>
        /// <term>RelativeDateSpanYesterday</term><description>Yesterday</description>
        /// </item>
        /// <item>
        /// <term>RelativeDateSpanTomorrow</term><description>Tomorrow</description>
        /// </item>
        /// <item>
        /// <term>RelativeDateSpanBeforeYesterday</term><description>before 2 days</description>
        /// </item>
        /// <item>
        /// <term>RelativeDateSpanAfterTomorrow</term><description>in 2 days</description>
        /// </item>
        /// <item>
        /// <term>RelativeDateSpanDaysBefore</term><description>before {0} days</description>
        /// </item>
        /// <item>
        /// <term>RelativeDateSpanDaysAfter</term><description>in {0} days</description>
        /// </item>
        /// </list>
        /// </remarks>
        /// <param name="current">The date from which to look.</param>
        /// <param name="compareDate">The date to compare with dtmBase.</param>
        /// <param name="resourceType">The target resource where the search should occur.</param>
        /// <returns>A resource-based string matching the time span.</returns>
        public static string GetRelativeDateSpanText(this DateTime current, DateTime compareDate, int resourceType = 0)
        {
            current = current.Date;
            compareDate = compareDate.Date;
            var dayDiff = current.Subtract(compareDate).Days;
            var inPast = dayDiff < 0;
            dayDiff = Math.Abs(dayDiff);
            if (dayDiff == 0)
            {
                // it is today
                return PortableResourceUtil.Get<string>("RelativeDateSpanToday", resourceType);
            }
            if (dayDiff == 1)
            {
                return inPast ? PortableResourceUtil.Get<string>("RelativeDateSpanYesterday", resourceType) : PortableResourceUtil.Get<string>("RelativeDateSpanTomorrow", resourceType);
            }
            if (dayDiff == 2)
            {
                return inPast ? PortableResourceUtil.Get<string>("RelativeDateSpanBeforeYesterday", resourceType) : PortableResourceUtil.Get<string>("RelativeDateSpanAfterTomorrow", resourceType);
            }
            var strPattern = inPast ? PortableResourceUtil.Get<string>("RelativeDateSpanDaysBefore", resourceType) : PortableResourceUtil.Get<string>("RelativeDateSpanDaysAfter", resourceType);
            return string.Format(strPattern, dayDiff);
        }

        /// <summary>
        /// Returns the calendar week of a given date.
        /// </summary>
        /// <param name="date">The date to examine.</param>
        /// <param name="culture">The culture to use or <c>null</c> if <see cref="CultureInfo.CurrentUICulture" /> should be taken.</param>
        /// <returns>The number of the calendar week.</returns>
        public static int GetWeekNumber(this DateTime date, CultureInfo culture = null)
        {
            if (culture == null)
            {
                culture = CultureInfo.CurrentUICulture;
            }
            var calendar = culture.Calendar;
            var weekRule = culture.DateTimeFormat.CalendarWeekRule;
            var firstDayOfWeek = culture.DateTimeFormat.FirstDayOfWeek;
            return calendar.GetWeekOfYear(date, weekRule, firstDayOfWeek);
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
        public static int? GetYearsDifference(this DateTime? dateInFuture, DateTime? dateInPast = null)
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
        public static int? GetYearsDifference(this DateTime dateInFuture, DateTime? dateInPast = null)
        {
            if (!dateInPast.HasValue)
            {
                // if no past-date is given, take current date
                dateInPast = DateTime.Now;
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
        public static int? GetYearsTilDifference(this DateTime? dateInPast, DateTime? dateInFuture = null)
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
        public static int? GetYearsTilDifference(this DateTime dateInPast, DateTime? dateInFuture = null)
        {
            if (!dateInFuture.HasValue)
            {
                // if no future-date is given, take current date
                dateInFuture = DateTime.Now;
            }
            return (int)(dateInFuture.Value.Subtract(dateInPast).TotalDays / DaysPerYear);
        }

        /// <summary>
        /// Calculates a time value for a given decimal <paramref name="time" />.
        /// </summary>
        /// <param name="time">The time in decimal notation.</param>
        /// <returns>The date-time.</returns>
        public static DateTime ToDateTime(this decimal time)
        {
            var hours = (int)time;
            if (hours < 0 || hours > 23)
            {
                throw new ArgumentException("Can not convert this decimal to a time!");
            }
            var minutes = time - Convert.ToDecimal(hours);
            var minutesToUse = (int)(60 * minutes);
            if (minutesToUse < 0 || minutesToUse > 59)
            {
                throw new ArgumentException("Can not convert this decimal to a time!");
            }
            return DateTime.Parse(string.Format(CultureInfo.InvariantCulture, "{0}:{1}", hours, minutesToUse));
        }

        /// <summary>
        /// Returns the decimal represenation of hour and minute of a given <paramref name="date" />.
        /// </summary>
        /// <param name="date">The date to convert.</param>
        /// <returns>A decimal value. For 16:30 it will return 16.5 e.g.</returns>
        public static decimal ToDecimalTime(this DateTime date)
        {
            var minute = Math.Round((decimal)date.Minute / 60, 2);
            return date.Hour + minute;
        }

        /// <summary>
        /// Retrieves the system long date string for the given nullable <paramref name="date" />.
        /// </summary>
        /// <param name="date">The date or <c>null</c>.</param>
        /// <returns>The formatted date or <see cref="string.Empty" /> if the <paramref name="date" /> is <c>null</c>.</returns>
        public static string ToLongDateString(this DateTime? date)
        {
            return date?.ToString("D") ?? string.Empty;
        }

        /// <summary>
        /// Retrieves the system long time string for the given nullable <paramref name="date" />.
        /// </summary>
        /// <param name="date">The date or <c>null</c>.</param>
        /// <returns>The formatted date or <see cref="string.Empty" /> if the <paramref name="date" /> is <c>null</c>.</returns>
        public static string ToLongTimeString(this DateTime? date)
        {
            return date?.ToString("T") ?? string.Empty;
        }

        /// <summary>
        /// Retrieves the system short date string for the given nullable <paramref name="date" />.
        /// </summary>
        /// <param name="date">The date or <c>null</c>.</param>
        /// <returns>The formatted date or <see cref="string.Empty" /> if the <paramref name="date" /> is <c>null</c>.</returns>
        public static string ToShortDateString(this DateTime? date)
        {
            return date?.ToString("d") ?? string.Empty;
        }

        /// <summary>
        /// Retrieves the system short time string for the given nullable <paramref name="date" />.
        /// </summary>
        /// <param name="date">The date or <c>null</c>.</param>
        /// <returns>The formatted date or <see cref="string.Empty" /> if the <paramref name="date" /> is <c>null</c>.</returns>
        public static string ToShortTimeString(this DateTime? date)
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
        public static string ToString(this DateTime? date, string format)
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
        public static string ToString(this DateTime? date, string format, IFormatProvider provider)
        {
            return date?.ToString(format, provider) ?? string.Empty;
        }

        #endregion
    }
}
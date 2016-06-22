namespace codingfreaks.cfUtils.Logic.Portable.Utilities
{
    using System;
    using System.Globalization;
    using System.Text;

    /// <summary>
    /// Provides methods for conversion of units.
    /// </summary>
    public class ConversionUtil
    {
        #region methods

        /// <summary>
        /// Converts meters to m or ft depending on the <paramref name="localeId" />.
        /// </summary>
        /// <param name="localeId">The locale ID.</param>
        /// <param name="meters">The amount of altitude.</param>
        /// <returns>The converted amount of altitude.</returns>
        public static double GetAltitude(string localeId, int meters)
        {
            return GetRegion(localeId).IsMetric ? meters : UnitConversionCalculator.ConvertMetersToFeet(meters);
        }

        /// <summary>
        /// Converts meters to m or ft depending on the <paramref name="localeId" />.
        /// </summary>
        /// <param name="localeId">The locale ID.</param>
        /// <param name="meters">The amount of altitude.</param>
        /// <returns>The converted amount of altitude.</returns>
        public static double GetAltitude(string localeId, double meters)
        {
            return GetAltitude(localeId, (int)meters);
        }

        /// <summary>
        /// Retrieves the correct altitude unit text for a given <paramref name="localeId" />.
        /// </summary>
        /// <param name="localeId">The locale ID.</param>
        /// <returns>The correkt unit.</returns>
        public static string GetAltitudeUnit(string localeId)
        {
            return GetRegion(localeId).IsMetric ? UnitConversionCalculator.MetricAltitudeUnit : UnitConversionCalculator.NonMetricAltitudeUnit;
        }

        /// <summary>
        /// Converts meters to km or miles depending on the <paramref name="localeId" />.
        /// </summary>
        /// <param name="localeId">The locale ID.</param>
        /// <param name="meters">The amount of distance.</param>
        /// <returns>The converted amount of distance.</returns>
        public static double GetDistance(string localeId, int meters)
        {
            return GetRegion(localeId).IsMetric ? meters * UnitConversionCalculator.KilometersPerMeter : UnitConversionCalculator.ConvertMetersToMiles(meters);
        }

        /// <summary>
        /// Converts meters to km or miles depending on the <paramref name="localeId" />.
        /// </summary>
        /// <param name="localeId">The locale ID.</param>
        /// <param name="meters">The amount of distance.</param>
        /// <returns>The converted amount of distance.</returns>
        public static double GetDistance(string localeId, double meters)
        {
            return GetDistance(localeId, (int)meters);
        }

        /// <summary>
        /// Retrieves the correct distance unit text for a given <paramref name="localeId" />.
        /// </summary>
        /// <param name="localeId">The locale ID.</param>
        /// <returns>The correkt unit.</returns>
        public static string GetDistanceUnit(string localeId)
        {
            return GetRegion(localeId).IsMetric ? UnitConversionCalculator.MetricDistanceUnit : UnitConversionCalculator.NonMetricDistanceUnit;
        }

        /// <summary>
        /// Calls <see cref="GetAltitude(string,int)" /> and <see cref="GetAltitudeUnit" /> and merges the results to a completely
        /// formatted altitude depending on the given <paramref name="localeId" />.
        /// </summary>
        /// <param name="localeId">The locale ID.</param>
        /// <param name="meters">The amount of altitude.</param>
        /// <returns>The formatted altitude-text.</returns>
        public static string GetFormattedAltitude(string localeId, int meters)
        {
            return string.Format(new CultureInfo(localeId), "{0:N0} {1}", GetAltitude(localeId, meters), GetAltitudeUnit(localeId));
        }

        /// <summary>
        /// Calls <see cref="GetAltitude(string,int)" /> and <see cref="GetAltitudeUnit" /> and merges the results to a completely
        /// formatted altitude depending on the given <paramref name="localeId" />.
        /// </summary>
        /// <param name="localeId">The locale ID.</param>
        /// <param name="meters">The amount of altitude.</param>
        /// <returns>The formatted altitude-text.</returns>
        public static string GetFormattedAltitude(string localeId, double meters)
        {
            return string.Format(new CultureInfo(localeId), "{0:N0} {1}", GetAltitude(localeId, meters), GetAltitudeUnit(localeId));
        }

        /// <summary>
        /// Calls <see cref="GetDistance(string,int)" /> and <see cref="GetDistanceUnit" /> and merges the results to a completely
        /// formatted distance depending on the given <paramref name="localeId" />.
        /// </summary>
        /// <param name="localeId">The locale ID.</param>
        /// <param name="meters">The amount of distance.</param>
        /// <param name="enforceBigUnit">
        /// If set to <c>true</c>, no automatic switch between small and big distances will be
        /// applied.
        /// </param>
        /// <param name="decimalPlaces">The amount of places after the ','.</param>
        /// <returns>The formatted distance-text.</returns>
        public static string GetFormattedDistance(string localeId, int meters, bool enforceBigUnit, int decimalPlaces)
        {
            return GetFormattedDistance(localeId, (double)meters, enforceBigUnit, decimalPlaces);
        }

        /// <summary>
        /// Calls <see cref="GetDistance(string,int)" /> and <see cref="GetDistanceUnit" /> and merges the results to a completely
        /// formatted distance depending on the given <paramref name="localeId" />.
        /// </summary>
        /// <param name="localeId">The locale ID.</param>
        /// <param name="meters">The amount of distance.</param>
        /// <returns>The formatted distance-text.</returns>
        public static string GetFormattedDistance(string localeId, int meters)
        {
            return GetFormattedDistance(localeId, (double)meters);
        }

        /// <summary>
        /// Calls <see cref="GetDistance(string,int)" /> and <see cref="GetDistanceUnit" /> and merges the results to a completely
        /// formatted distance depending on the given <paramref name="localeId" />.
        /// </summary>
        /// <param name="localeId">The locale ID.</param>
        /// <param name="meters">The amount of distance.</param>
        /// <returns>The formatted distance-text.</returns>
        public static string GetFormattedDistance(string localeId, double meters)
        {
            return GetFormattedDistance(localeId, meters, false, 2);
        }

        /// <summary>
        /// Calls <see cref="GetDistance(string,int)" /> and <see cref="GetDistanceUnit" /> and merges the results to a completely
        /// formatted distance depending on the given <paramref name="localeId" />.
        /// </summary>
        /// <param name="localeId">The locale ID.</param>
        /// <param name="meters">The amount of distance.</param>
        /// <param name="enforceBigUnit">
        /// If set to <c>true</c>, no automatic switch between small and big distances will be
        /// applied.
        /// </param>
        /// <param name="decimalPlaces">The amount of places after the ','.</param>
        /// <returns>The formatted distance-text.</returns>
        public static string GetFormattedDistance(string localeId, double meters, bool enforceBigUnit, int decimalPlaces)
        {
            var distance = GetDistance(localeId, meters);
            var unit = GetDistanceUnit(localeId);
            if (distance < 1 && !enforceBigUnit)
            {
                distance = GetSmallDistance(localeId, meters);
                unit = GetSmallDistanceUnit(localeId);
            }
            var formatPattern = "{0:N" + decimalPlaces + "} {1}";
            return string.Format(new CultureInfo(localeId), formatPattern, distance, unit);
        }

        /// <summary>
        /// Converts a timespan in seconds into a localized version.
        /// </summary>
        /// <param name="localeId">The locale ID.</param>
        /// <param name="seconds">The duration in seconds.</param>
        /// <returns>The timespan-string.</returns>
        public static string GetFormattedDuration(string localeId, long seconds)
        {
            var span = TimeSpan.FromSeconds(seconds);
            return span.ToString("c", new CultureInfo(localeId));
        }

        /// <summary>
        /// Calls <see cref="GetHeight(string,int)" /> and <see cref="GetHeightUnit" /> and merges the results to a completely
        /// formatted height depending on the given <paramref name="localeId" />.
        /// </summary>
        /// <param name="localeId">The locale ID.</param>
        /// <param name="centiMeters">The amount of height.</param>
        /// <returns>The formatted height-text.</returns>
        public static string GetFormattedHeight(string localeId, int centiMeters)
        {
            return string.Format(new CultureInfo(localeId), "{0:N0} {1}", GetHeight(localeId, centiMeters), GetHeightUnit(localeId));
        }

        /// <summary>
        /// Calls <see cref="GetSpeed(string,int)" /> and <see cref="GetSpeedUnit" /> and merges the results to a completely
        /// formatted speed depending on the given <paramref name="localeId" />.
        /// </summary>
        /// <param name="localeId">The locale ID.</param>
        /// <param name="metersPerSecond">The amount of speed.</param>
        /// <returns>The formatted speed-text.</returns>
        public static string GetFormattedSpeed(string localeId, int metersPerSecond)
        {
            return string.Format(new CultureInfo(localeId), "{0:N0} {1}", GetSpeed(localeId, metersPerSecond), GetSpeedUnit(localeId));
        }

        /// <summary>
        /// Calls <see cref="GetSpeed(string,int)" /> and <see cref="GetSpeedUnit" /> and merges the results to a completely
        /// formatted speed depending on the given <paramref name="localeId" />.
        /// </summary>
        /// <param name="localeId">The locale ID.</param>
        /// <param name="metersPerSecond">The amount of speed.</param>
        /// <returns>The formatted speed-text.</returns>
        public static string GetFormattedSpeed(string localeId, double metersPerSecond)
        {
            return string.Format(new CultureInfo(localeId), "{0:N0} {1}", GetSpeed(localeId, metersPerSecond), GetSpeedUnit(localeId));
        }

        /// <summary>
        /// Calls <see cref="GetSpeed(string,int)" /> and <see cref="GetSpeedUnit" /> and merges the results to a completely
        /// formatted speed depending on the given <paramref name="localeId" />.
        /// </summary>
        /// <param name="localeId">The locale ID.</param>
        /// <param name="metersPerSecond">The amount of speed.</param>
        /// <returns>The formatted speed-text with a precision of 2.</returns>
        public static string GetFormattedSpeedAccurate(string localeId, double metersPerSecond)
        {
            return string.Format(new CultureInfo(localeId), "{0:N2} {1}", GetSpeed(localeId, metersPerSecond), GetSpeedUnit(localeId));
        }

        /// <summary>
        /// Retrieves a timespan formatted in a given <paramref name="localeId" />.
        /// </summary>
        /// <param name="localeId">The locale ID.</param>
        /// <param name="value">The timepart.</param>
        /// <returns>The formatted altitude-text.</returns>
        public static string GetFormattedTimespan(string localeId, TimeSpan value)
        {
            return value.ToString("c", new CultureInfo(localeId));
        }

        /// <summary>
        /// Calls <see cref="GetWeight(string,int)" /> and <see cref="GetWeightUnit" /> and merges the results to a completely
        /// formatted weight depending on the given <paramref name="localeId" />.
        /// </summary>
        /// <param name="localeId">The locale ID.</param>
        /// <param name="kiloGrams">The amount of weight.</param>
        /// <returns>The formatted weight-text.</returns>
        public static string GetFormattedWeight(string localeId, int kiloGrams)
        {
            return string.Format(new CultureInfo(localeId), "{0:N0} {1}", GetWeight(localeId, kiloGrams), GetWeightUnit(localeId));
        }

        /// <summary>
        /// Converts centimeters to cm or inch depending on the <paramref name="localeId" />.
        /// </summary>
        /// <param name="localeId">The locale ID.</param>
        /// <param name="centiMeters">The amount of height.</param>
        /// <returns>The converted amount of height.</returns>
        public static double GetHeight(string localeId, int centiMeters)
        {
            return GetRegion(localeId).IsMetric ? centiMeters : UnitConversionCalculator.ConvertCentimetersToInches(centiMeters);
        }

        /// <summary>
        /// Retrieves the correct height unit text for a given <paramref name="localeId" />.
        /// </summary>
        /// <param name="localeId">The locale ID.</param>
        /// <returns>The correkt unit.</returns>
        public static string GetHeightUnit(string localeId)
        {
            return GetRegion(localeId).IsMetric ? UnitConversionCalculator.MetricHeightUnit : UnitConversionCalculator.NonMetricHeightUnit;
        }

        /// <summary>
        /// Retrieves a DateTime in a HTML formatted manner. The date is bigger than the time.
        /// </summary>
        /// <param name="original">The datetime as a string.</param>
        /// <returns>The HTML-version.</returns>
        public static string GetOptimizedTimeHtmlString(string original)
        {
            var parts = original.Split(' ');
            return parts.Length == 2 ? string.Format("{0} <span style=\"font-size:0.8em\">{1}</span>", parts[0], parts[1]) : original;
        }

        /// <summary>
        /// Converts meters to meters or yards depending on the <paramref name="localeId" />.
        /// </summary>
        /// <param name="localeId">The locale ID.</param>
        /// <param name="meters">The amount of distance.</param>
        /// <returns>The converted amount of distance.</returns>
        public static double GetSmallDistance(string localeId, int meters)
        {
            return GetRegion(localeId).IsMetric ? meters : UnitConversionCalculator.ConvertMetersToYards(meters);
        }

        /// <summary>
        /// Converts meters to km or yards depending on the <paramref name="localeId" />.
        /// </summary>
        /// <param name="localeId">The locale ID.</param>
        /// <param name="meters">The amount of distance.</param>
        /// <returns>The converted amount of distance.</returns>
        public static double GetSmallDistance(string localeId, double meters)
        {
            return GetSmallDistance(localeId, (int)meters);
        }

        /// <summary>
        /// Retrieves the correct distance unit for small distances text for a given <paramref name="localeId" />.
        /// </summary>
        /// <param name="localeId">The locale ID.</param>
        /// <returns>The correkt unit.</returns>
        public static string GetSmallDistanceUnit(string localeId)
        {
            return GetRegion(localeId).IsMetric ? UnitConversionCalculator.MetricDistanceSmallUnit : UnitConversionCalculator.NonMetricDistanceSmallUnit;
        }

        /// <summary>
        /// Generates a speaking text for a given amount of <paramref name="seconds" />.
        /// </summary>
        /// <param name="localeId">The locale ID.</param>
        /// <param name="seconds">The amount of seconds representing the amount of time.</param>
        /// <param name="resourceResolver">An optional function that this method uses to retrieve resource-strings.</param>
        /// <returns>The speaking text.</returns>
        public static string GetSpeakingTimeString(string localeId, long seconds, Func<string, string> resourceResolver = null)
        {
            if (resourceResolver == null)
            {
                resourceResolver = key => PortableResourceUtil.Get(localeId, key);
            }
            var span = TimeSpan.FromSeconds(seconds);
            var builder = new StringBuilder();
            var resourceKey = string.Empty;
            if (span.Days > 365)
            {
                var years = span.Days / 365;
                span = span.Subtract(TimeSpan.FromSeconds(years * UnitConversionCalculator.SecondsPerYear));
                resourceKey = years != 1 ? "TimePartYearPlural" : "TimePartYearSingular";
                builder.AppendFormat("{0} {1} ", years, resourceResolver(resourceKey));
            }
            if (span.Days > 29)
            {
                var months = span.Days / 30;
                span = span.Subtract(TimeSpan.FromSeconds(months * UnitConversionCalculator.SecondsPerMonth));
                resourceKey = months != 1 ? "TimePartMonthPlural" : "TimePartMonthSingular";
                builder.AppendFormat("{0} {1} ", months, resourceResolver(resourceKey));
            }
            if (span.Days > 6)
            {
                var weeks = span.Days / 7;
                span = span.Subtract(TimeSpan.FromSeconds(weeks * UnitConversionCalculator.SecondsPerWeek));
                resourceKey = weeks != 1 ? "TimePartWeekPlural" : "TimePartWeekSingular";
                builder.AppendFormat("{0} {1} ", weeks, resourceResolver(resourceKey));
            }
            if (span.Days >= 1)
            {
                resourceKey = span.Days != 1 ? "TimePartDayPlural" : "TimePartDaySingular";
                builder.AppendFormat("{0} {1} ", span.Days, resourceResolver(resourceKey));
                span = span.Subtract(TimeSpan.FromSeconds(span.Days * UnitConversionCalculator.SecondsPerDay));
            }
            resourceKey = span.Hours != 1 ? "TimePartHourPlural" : "TimePartHourSingular";
            builder.AppendFormat("{0} {1} ", span.Hours, resourceResolver(resourceKey));
            span = span.Subtract(TimeSpan.FromSeconds(span.Hours * 60 * 60));
            resourceKey = span.Minutes != 1 ? "TimePartMinutePlural" : "TimePartMinuteSingular";
            builder.AppendFormat("{0} {1} ", span.Minutes, resourceResolver(resourceKey));
            span = span.Subtract(TimeSpan.FromSeconds(span.Hours * 60));
            resourceKey = span.Seconds != 1 ? "TimePartSecondPlural" : "TimePartSecondSingular";
            builder.AppendFormat("{0} {1} ", span.Seconds, resourceResolver(resourceKey));
            return builder.ToString().Trim();
        }

        /// <summary>
        /// Generates a speaking text for a given amount of <paramref name="seconds" /> with abbreviations.
        /// </summary>
        /// <param name="localeId">The locale ID.</param>
        /// <param name="seconds">The amount of seconds representing the amount of time.</param>
        /// <param name="resourceResolver">An optional function that this method uses to retrieve resource-strings.</param>
        /// <returns>The speaking text.</returns>
        public static string GetSpeakingTimeStringShort(string localeId, long seconds, Func<string, string> resourceResolver = null)
        {
            if (resourceResolver == null)
            {
                resourceResolver = key => PortableResourceUtil.Get(localeId, key);
            }
            var span = TimeSpan.FromSeconds(seconds);
            var builder = new StringBuilder();
            if (span.Days > 365)
            {
                var years = span.Days / 365;
                span = span.Subtract(TimeSpan.FromSeconds(years * UnitConversionCalculator.SecondsPerYear));
                builder.AppendFormat("{0}{1},", years, resourceResolver("TimePartYearShort"));
            }
            if (span.Days > 29)
            {
                var months = span.Days / 30;
                span = span.Subtract(TimeSpan.FromSeconds(months * UnitConversionCalculator.SecondsPerMonth));
                builder.AppendFormat("{0}{1},", months, resourceResolver("TimePartMonthShort"));
            }
            if (span.Days > 6)
            {
                var weeks = span.Days / 7;
                span = span.Subtract(TimeSpan.FromSeconds(weeks * UnitConversionCalculator.SecondsPerWeek));
                builder.AppendFormat("{0}{1},", weeks, resourceResolver("TimePartWeekShort"));
            }
            if (span.Days >= 1)
            {
                builder.AppendFormat("{0}{1},", span.Days, resourceResolver("TimePartDayShort"));
                span = span.Subtract(TimeSpan.FromSeconds(span.Days * UnitConversionCalculator.SecondsPerDay));
            }
            builder.AppendFormat("{0}{1},", span.Hours, resourceResolver("TimePartHourShort"));
            span = span.Subtract(TimeSpan.FromSeconds(span.Hours * 60 * 60));
            builder.AppendFormat("{0}{1},", span.Minutes, resourceResolver("TimePartMinuteShort"));
            span = span.Subtract(TimeSpan.FromSeconds(span.Hours * 60));
            builder.AppendFormat("{0}{1}", span.Seconds, resourceResolver("TimePartSecondShort"));
            return builder.ToString().Trim();
        }

        /// <summary>
        /// Generates a speaking text for a given amount of <paramref name="seconds" /> with abbrevations for HTML-output.
        /// </summary>
        /// <param name="localeId">The locale ID.</param>
        /// <param name="seconds">The amount of seconds representing the amount of time.</param>
        /// <param name="classNameUnits">The name of the CSS-class to format the units with.</param>
        /// <param name="resourceResolver">An optional function that this method uses to retrieve resource-strings.</param>
        /// <returns>The speaking text in HTML.</returns>
        public static string GetSpeakingTimeStringShortHtml(string localeId, long seconds, string classNameUnits, Func<string, string> resourceResolver = null)
        {
            if (resourceResolver == null)
            {
                resourceResolver = key => PortableResourceUtil.Get(localeId, key);
            }
            var span = TimeSpan.FromSeconds(seconds);
            var builder = new StringBuilder();
            if (span.Days > 365)
            {
                var years = span.Days / 365;
                span = span.Subtract(TimeSpan.FromSeconds(years * UnitConversionCalculator.SecondsPerYear));
                builder.AppendFormat("{0}<span class=\"{1}\">{2}</span>", years, classNameUnits, resourceResolver("TimePartYearShort"));
            }
            if (span.Days > 29)
            {
                var months = span.Days / 30;
                span = span.Subtract(TimeSpan.FromSeconds(months * UnitConversionCalculator.SecondsPerMonth));
                builder.AppendFormat("{0}<span class=\"{1}\">{2}</span>", months, classNameUnits, resourceResolver("TimePartMonthShort"));
            }
            if (span.Days > 6)
            {
                var weeks = span.Days / 7;
                span = span.Subtract(TimeSpan.FromSeconds(weeks * UnitConversionCalculator.SecondsPerWeek));
                builder.AppendFormat("{0}<span class=\"{1}\">{2}</span>", weeks, classNameUnits, resourceResolver("TimePartWeekShort"));
            }
            if (span.Days >= 1)
            {
                builder.AppendFormat("{0}<span class=\"{1}\">{2}</span>", span.Days, classNameUnits, resourceResolver("TimePartDayShort"));
                span = span.Subtract(TimeSpan.FromSeconds(span.Days * UnitConversionCalculator.SecondsPerDay));
            }
            builder.AppendFormat("{0}<span class=\"{1}\">{2}</span>", span.Hours, classNameUnits, resourceResolver("TimePartHourShort"));
            span = span.Subtract(TimeSpan.FromSeconds(span.Hours * 60 * 60));
            builder.AppendFormat("{0}<span class=\"{1}\">{2}</span>", span.Minutes, classNameUnits, resourceResolver("TimePartMinuteShort"));
            span = span.Subtract(TimeSpan.FromSeconds(span.Hours * 60));
            builder.AppendFormat("{0}<span class=\"{1}\">{2}</span>", span.Seconds, classNameUnits, resourceResolver("TimePartSecondShort"));
            return builder.ToString().Trim();
        }

        /// <summary>
        /// Converts meters/s to km/h or miles/h depending on the <paramref name="localeId" />.
        /// </summary>
        /// <param name="localeId">The locale ID.</param>
        /// <param name="metersPerSecond">The amount of speed.</param>
        /// <returns>The converted amount of speed.</returns>
        public static double GetSpeed(string localeId, int metersPerSecond)
        {
            if (string.IsNullOrEmpty(localeId))
            {
                throw new ArgumentException("localeId");
            }
            return GetRegion(localeId).IsMetric
                ? UnitConversionCalculator.ConvertMetersPerSecondToKilometersPerHour(metersPerSecond)
                : UnitConversionCalculator.ConvertMetersPerSecondToMilesPerHour(metersPerSecond);
        }

        /// <summary>
        /// Converts meters/s to km/h or miles/h depending on the <paramref name="localeId" />.
        /// </summary>
        /// <param name="localeId">The locale ID.</param>
        /// <param name="metersPerSecond">The amount of speed.</param>
        /// <returns>The converted amount of speed.</returns>
        public static double GetSpeed(string localeId, double metersPerSecond)
        {
            if (string.IsNullOrEmpty(localeId))
            {
                throw new ArgumentException("localeId");
            }
            return GetSpeed(localeId, (int)metersPerSecond);
        }

        /// <summary>
        /// Retrieves the correct speed unit text for a given <paramref name="localeId" />.
        /// </summary>
        /// <param name="localeId">The locale ID.</param>
        /// <returns>The correkt unit.</returns>
        public static string GetSpeedUnit(string localeId)
        {
            return GetRegion(localeId).IsMetric ? UnitConversionCalculator.MetricSpeedUnit : UnitConversionCalculator.NonMetricSpeedUnit;
        }

        /// <summary>
        /// Converts kilograms to kg or pound depending on the <paramref name="localeId" />.
        /// </summary>
        /// <param name="localeId">The locale ID.</param>
        /// <param name="kiloGrams">The amount of weight.</param>
        /// <returns>The converted amount of weight.</returns>
        public static double GetWeight(string localeId, int kiloGrams)
        {
            return GetRegion(localeId).IsMetric ? kiloGrams : UnitConversionCalculator.ConvertKilogramToPounds(kiloGrams);
        }

        /// <summary>
        /// Retrieves the correct weight unit text for a given <paramref name="localeId" />.
        /// </summary>
        /// <param name="localeId">The locale ID.</param>
        /// <returns>The correkt unit.</returns>
        public static string GetWeightUnit(string localeId)
        {
            return GetRegion(localeId).IsMetric ? UnitConversionCalculator.MetricWeightUnit : UnitConversionCalculator.NonMetricWeightUnit;
        }

        /// <summary>
        /// Retrieves either the <see cref="RegionInfo" /> for <paramref name="localeId" />.
        /// </summary>
        /// <param name="localeId">The specific culture ID in the form xx-XX.</param>
        /// <exception cref="CultureNotFoundException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <returns>A usable region info.</returns>
        private static RegionInfo GetRegion(string localeId)
        {
            if (string.IsNullOrEmpty(localeId))
            {
                throw new ArgumentException("localeId");
            }
            var culture = new CultureInfo(localeId);
            if (culture.IsNeutralCulture)
            {
                // Try to get the specific culture 
                throw new ArgumentException("Provided culture not specific!", "localeId");
            }
            var result = new RegionInfo(localeId);
            return result;
        }

        #endregion
    }
}
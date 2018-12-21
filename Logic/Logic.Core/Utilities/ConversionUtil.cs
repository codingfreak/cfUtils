namespace codingfreaks.cfUtils.Logic.Core.Utilities
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Provides methods for conversion of units.
    /// </summary>
    public class ConversionUtil
    {
        #region constants

        /// <summary>
        /// The tolerance on comparison operations.
        /// </summary>
        private const double CompareTolerance = 0.000000001;

        #endregion

        #region methods

        /// <summary>
        /// Converts meters to m or ft depending on the <paramref name="localeId" />.
        /// </summary>
        /// <param name="localeId">The locale ID.</param>
        /// <param name="meters">The amount of altitude.</param>
        /// <returns>The converted amount of altitude.</returns>
        public static double GetAltitude(string localeId, double meters)
        {
            CheckUtil.ThrowIfNullOrEmpty(() => localeId);
            if (Math.Abs(meters) < CompareTolerance)
            {
                return meters;
            }
            return GetRegion(localeId).IsMetric ? meters : UnitConversionCalculator.ConvertMetersToFeet(meters);
        }

        /// <summary>
        /// Retrieves the correct altitude unit text for a given <paramref name="localeId" />.
        /// </summary>
        /// <param name="localeId">The locale ID.</param>
        /// <returns>The correct unit.</returns>
        public static string GetAltitudeUnit(string localeId)
        {
            CheckUtil.ThrowIfNullOrEmpty(() => localeId);
            return GetRegion(localeId).IsMetric ? MetricAltitudeUnit : ImperialAltitudeUnit;
        }

        /// <summary>
        /// Converts meters to km or miles depending on the <paramref name="localeId" />.
        /// </summary>
        /// <param name="localeId">The locale ID.</param>
        /// <param name="meters">The amount of distance.</param>
        /// <returns>The converted amount of distance.</returns>
        public static double GetDistance(string localeId, double meters)
        {
            CheckUtil.ThrowIfNullOrEmpty(() => localeId);
            if (Math.Abs(meters) < CompareTolerance)
            {
                return meters;
            }
            return GetRegion(localeId).IsMetric ? meters * Constants.KilometersPerMeter : UnitConversionCalculator.ConvertMetersToMiles(meters);
        }

        /// <summary>
        /// Retrieves the correct distance unit text for a given <paramref name="localeId" />.
        /// </summary>
        /// <param name="localeId">The locale ID.</param>
        /// <returns>The correct unit.</returns>
        public static string GetDistanceUnit(string localeId)
        {
            CheckUtil.ThrowIfNullOrEmpty(() => localeId);
            return GetRegion(localeId).IsMetric ? MetricDistanceUnit : ImperialDistanceUnit;
        }

        /// <summary>
        /// Calls <see cref="GetAltitude" /> and <see cref="GetAltitudeUnit" /> and merges the results to a completely
        /// formatted altitude depending on the given <paramref name="localeId" />.
        /// </summary>
        /// <param name="localeId">The locale ID.</param>
        /// <param name="meters">The amount of altitude.</param>
        /// <param name="decimalPlaces">The optional amount of decimal places for the output (defaults to 0).</param>
        /// <returns>The formatted altitude-text.</returns>
        public static string GetFormattedAltitude(string localeId, double meters, int decimalPlaces = 0)
        {
            CheckUtil.ThrowIfNullOrEmpty(() => localeId);
            return GetFormattedValue(localeId, meters, GetAltitude, GetAltitudeUnit, decimalPlaces);
        }

        /// <summary>
        /// Calls <see cref="GetDistance" /> and <see cref="GetDistanceUnit" /> and merges the results to a completely
        /// formatted distance depending on the given <paramref name="localeId" />.
        /// </summary>
        /// <param name="localeId">The locale ID.</param>
        /// <param name="meters">The amount of distance.</param>
        /// <param name="enforceBigUnit">
        /// If set to <c>true</c>, no automatic switch between small and big distances will be applied.
        /// </param>
        /// <param name="decimalPlaces">The optional amount of decimal places for the output (defaults to 0).</param>
        /// <returns>The formatted distance-text.</returns>
        public static string GetFormattedDistance(string localeId, double meters, bool enforceBigUnit, int decimalPlaces = 0)
        {
            CheckUtil.ThrowIfNullOrEmpty(() => localeId);
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
        /// Calls <see cref="GetHeight" /> and <see cref="GetHeightUnit" /> and merges the results to a completely
        /// formatted height depending on the given <paramref name="localeId" />.
        /// </summary>
        /// <param name="localeId">The locale ID.</param>
        /// <param name="centiMeters">The amount of height.</param>
        /// <param name="decimalPlaces">The optional amount of decimal places for the output (defaults to 0).</param>
        /// <returns>The formatted height-text.</returns>
        public static string GetFormattedHeight(string localeId, double centiMeters, int decimalPlaces = 0)
        {
            CheckUtil.ThrowIfNullOrEmpty(() => localeId);
            return GetFormattedValue(localeId, centiMeters, GetHeight, GetHeightUnit, decimalPlaces);
        }

        /// <summary>
        /// Converts a timespan in seconds into a localized version.
        /// </summary>
        /// <param name="localeId">The locale ID.</param>
        /// <param name="seconds">The duration in seconds.</param>
        /// <returns>The timespan-string.</returns>
        public static string GetFormattedSeconds(string localeId, long seconds)
        {
            CheckUtil.ThrowIfNullOrEmpty(() => localeId);
            var span = TimeSpan.FromSeconds(seconds);
            return span.ToString("c", new CultureInfo(localeId));
        }

        /// <summary>
        /// Calls <see cref="GetSpeed" /> and <see cref="GetSpeedUnit" /> and merges the results to a completely
        /// formatted speed depending on the given <paramref name="localeId" />.
        /// </summary>
        /// <param name="localeId">The locale ID.</param>
        /// <param name="metersPerSecond">The amount of speed.</param>
        /// <param name="decimalPlaces">The optional amount of decimal places for the output (defaults to 0).</param>
        /// <returns>The formatted speed-text.</returns>
        public static string GetFormattedSpeed(string localeId, double metersPerSecond, int decimalPlaces = 0)
        {
            CheckUtil.ThrowIfNullOrEmpty(() => localeId);
            return GetFormattedValue(localeId, metersPerSecond, GetSpeed, GetSpeedUnit, decimalPlaces);
        }

        /// <summary>
        /// Retrieves a timespan formatted in a given <paramref name="localeId" />.
        /// </summary>
        /// <param name="localeId">The locale ID.</param>
        /// <param name="value">The timepart.</param>
        /// <returns>The formatted altitude-text.</returns>
        public static string GetFormattedTimespan(string localeId, TimeSpan value)
        {
            CheckUtil.ThrowIfNullOrEmpty(() => localeId);
            return value.ToString("c", new CultureInfo(localeId));
        }

        /// <summary>
        /// Calls <see cref="GetWeight" /> and <see cref="GetWeightUnit" /> and merges the results to a completely
        /// formatted weight depending on the given <paramref name="localeId" />.
        /// </summary>
        /// <param name="localeId">The locale ID.</param>
        /// <param name="kiloGrams">The amount of weight.</param>
        /// <param name="decimalPlaces">The optional amount of decimal places for the output (defaults to 0).</param>
        /// <returns>The formatted weight-text.</returns>
        public static string GetFormattedWeight(string localeId, double kiloGrams, int decimalPlaces = 0)
        {
            CheckUtil.ThrowIfNullOrEmpty(() => localeId);
            return GetFormattedValue(localeId, kiloGrams, GetWeight, GetWeightUnit, decimalPlaces);
        }

        /// <summary>
        /// Converts centimeters to cm or inch depending on the <paramref name="localeId" />.
        /// </summary>
        /// <param name="localeId">The locale ID.</param>
        /// <param name="centiMeters">The amount of height.</param>
        /// <returns>The converted amount of height.</returns>
        public static double GetHeight(string localeId, double centiMeters)
        {
            CheckUtil.ThrowIfNullOrEmpty(() => localeId);
            return GetRegion(localeId).IsMetric ? centiMeters : UnitConversionCalculator.ConvertCentimetersToInches(centiMeters);
        }

        /// <summary>
        /// Retrieves the correct height unit text for a given <paramref name="localeId" />.
        /// </summary>
        /// <param name="localeId">The locale ID.</param>
        /// <returns>The correct unit.</returns>
        public static string GetHeightUnit(string localeId)
        {
            CheckUtil.ThrowIfNullOrEmpty(() => localeId);
            return GetRegion(localeId).IsMetric ? MetricHeightUnit : ImperialHeightUnit;
        }

        /// <summary>
        /// Retrieves a DateTime in a HTML formatted manner. The date is bigger than the time.
        /// </summary>
        /// <param name="original">The datetime as a string.</param>
        /// <returns>The HTML-version.</returns>
        public static string GetOptimizedTimeHtmlString(string original)
        {
            CheckUtil.ThrowIfNullOrEmpty(() => original);
            var parts = original.Split(' ');
            return parts.Length == 2 ? $"{parts[0]} <span style=\"font-size:0.8em\">{parts[1]}</span>" : original;
        }

        /// <summary>
        /// Converts meters to km or yards depending on the <paramref name="localeId" />.
        /// </summary>
        /// <param name="localeId">The locale ID.</param>
        /// <param name="meters">The amount of distance.</param>
        /// <returns>The converted amount of distance.</returns>
        public static double GetSmallDistance(string localeId, double meters)
        {
            CheckUtil.ThrowIfNullOrEmpty(() => localeId);
            return GetRegion(localeId).IsMetric ? meters : UnitConversionCalculator.ConvertMetersToYards(meters);
        }

        /// <summary>
        /// Retrieves the correct distance unit for small distances text for a given <paramref name="localeId" />.
        /// </summary>
        /// <param name="localeId">The locale ID.</param>
        /// <returns>The correct unit.</returns>
        public static string GetSmallDistanceUnit(string localeId)
        {
            CheckUtil.ThrowIfNullOrEmpty(() => localeId);
            return GetRegion(localeId).IsMetric ? MetricDistanceSmallUnit : ImperialDistanceSmallUnit;
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
            CheckUtil.ThrowIfNullOrEmpty(() => localeId);
            if (resourceResolver == null)
            {
                resourceResolver = key => "?";
            }
            var span = TimeSpan.FromSeconds(seconds);
            var builder = new StringBuilder();
            string resourceKey;
            if (span.Days > 365)
            {
                var years = span.Days / 365;
                span = span.Subtract(TimeSpan.FromSeconds(years * Constants.SecondsPerYear));
                resourceKey = years != 1 ? "TimePartYearPlural" : "TimePartYearSingular";
                builder.AppendFormat("{0} {1} ", years, resourceResolver(resourceKey));
            }
            if (span.Days > 29)
            {
                var months = span.Days / 30;
                span = span.Subtract(TimeSpan.FromSeconds(months * Constants.SecondsPerMonth));
                resourceKey = months != 1 ? "TimePartMonthPlural" : "TimePartMonthSingular";
                builder.AppendFormat("{0} {1} ", months, resourceResolver(resourceKey));
            }
            if (span.Days > 6)
            {
                var weeks = span.Days / 7;
                span = span.Subtract(TimeSpan.FromSeconds(weeks * Constants.SecondsPerWeek));
                resourceKey = weeks != 1 ? "TimePartWeekPlural" : "TimePartWeekSingular";
                builder.AppendFormat("{0} {1} ", weeks, resourceResolver(resourceKey));
            }
            if (span.Days >= 1)
            {
                resourceKey = span.Days != 1 ? "TimePartDayPlural" : "TimePartDaySingular";
                builder.AppendFormat("{0} {1} ", span.Days, resourceResolver(resourceKey));
                span = span.Subtract(TimeSpan.FromSeconds(span.Days * Constants.SecondsPerDay));
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
            CheckUtil.ThrowIfNullOrEmpty(() => localeId);
            if (resourceResolver == null)
            {
                resourceResolver = key => "?";
            }
            var span = TimeSpan.FromSeconds(seconds);
            var builder = new StringBuilder();
            if (span.Days > 365)
            {
                var years = span.Days / 365;
                span = span.Subtract(TimeSpan.FromSeconds(years * Constants.SecondsPerYear));
                builder.AppendFormat("{0}{1},", years, resourceResolver("TimePartYearShort"));
            }
            if (span.Days > 29)
            {
                var months = span.Days / 30;
                span = span.Subtract(TimeSpan.FromSeconds(months * Constants.SecondsPerMonth));
                builder.AppendFormat("{0}{1},", months, resourceResolver("TimePartMonthShort"));
            }
            if (span.Days > 6)
            {
                var weeks = span.Days / 7;
                span = span.Subtract(TimeSpan.FromSeconds(weeks * Constants.SecondsPerWeek));
                builder.AppendFormat("{0}{1},", weeks, resourceResolver("TimePartWeekShort"));
            }
            if (span.Days >= 1)
            {
                builder.AppendFormat("{0}{1},", span.Days, resourceResolver("TimePartDayShort"));
                span = span.Subtract(TimeSpan.FromSeconds(span.Days * Constants.SecondsPerDay));
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
            CheckUtil.ThrowIfNullOrEmpty(() => localeId);
            if (resourceResolver == null)
            {
                resourceResolver = key => "?";
            }
            var span = TimeSpan.FromSeconds(seconds);
            var builder = new StringBuilder();
            if (span.Days > 365)
            {
                var years = span.Days / 365;
                span = span.Subtract(TimeSpan.FromSeconds(years * Constants.SecondsPerYear));
                builder.AppendFormat("{0}<span class=\"{1}\">{2}</span>", years, classNameUnits, resourceResolver("TimePartYearShort"));
            }
            if (span.Days > 29)
            {
                var months = span.Days / 30;
                span = span.Subtract(TimeSpan.FromSeconds(months * Constants.SecondsPerMonth));
                builder.AppendFormat("{0}<span class=\"{1}\">{2}</span>", months, classNameUnits, resourceResolver("TimePartMonthShort"));
            }
            if (span.Days > 6)
            {
                var weeks = span.Days / 7;
                span = span.Subtract(TimeSpan.FromSeconds(weeks * Constants.SecondsPerWeek));
                builder.AppendFormat("{0}<span class=\"{1}\">{2}</span>", weeks, classNameUnits, resourceResolver("TimePartWeekShort"));
            }
            if (span.Days >= 1)
            {
                builder.AppendFormat("{0}<span class=\"{1}\">{2}</span>", span.Days, classNameUnits, resourceResolver("TimePartDayShort"));
                span = span.Subtract(TimeSpan.FromSeconds(span.Days * Constants.SecondsPerDay));
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
        public static double GetSpeed(string localeId, double metersPerSecond)
        {
            CheckUtil.ThrowIfNullOrEmpty(() => localeId);
            if (Math.Abs(metersPerSecond) < CompareTolerance)
            {
                return 0;
            }
            if (string.IsNullOrEmpty(localeId))
            {
                throw new ArgumentException("localeId");
            }
            return GetRegion(localeId).IsMetric
                ? UnitConversionCalculator.ConvertMetersPerSecondToKilometersPerHour(metersPerSecond)
                : UnitConversionCalculator.ConvertMetersPerSecondToMilesPerHour(metersPerSecond);
        }

        /// <summary>
        /// Retrieves the correct speed unit text for a given <paramref name="localeId" />.
        /// </summary>
        /// <param name="localeId">The locale ID.</param>
        /// <returns>The correct unit.</returns>
        public static string GetSpeedUnit(string localeId)
        {
            CheckUtil.ThrowIfNullOrEmpty(() => localeId);
            return GetRegion(localeId).IsMetric ? MetricSpeedUnit : ImperialSpeedUnit;
        }

        /// <summary>
        /// Converts kilograms to kg or pound depending on the <paramref name="localeId" />.
        /// </summary>
        /// <param name="localeId">The locale ID.</param>
        /// <param name="kiloGrams">The amount of weight.</param>
        /// <returns>The converted amount of weight.</returns>
        public static double GetWeight(string localeId, double kiloGrams)
        {
            CheckUtil.ThrowIfNullOrEmpty(() => localeId);
            if (Math.Abs(kiloGrams) < CompareTolerance)
            {
                return 0;
            }
            return GetRegion(localeId).IsMetric ? kiloGrams : UnitConversionCalculator.ConvertKilogramToPounds(kiloGrams);
        }

        /// <summary>
        /// Retrieves the correct weight unit text for a given <paramref name="localeId" />.
        /// </summary>
        /// <param name="localeId">The locale ID.</param>
        /// <returns>The correct unit.</returns>
        public static string GetWeightUnit(string localeId)
        {
            CheckUtil.ThrowIfNullOrEmpty(() => localeId);
            return GetRegion(localeId).IsMetric ? MetricWeightUnit : ImperialWeightUnit;
        }

        /// <summary>
        /// Wrapper for methods in this type which will return formatted values depending on <paramref name="localeId" /> and
        /// <paramref name="decimalPlaces" />.
        /// </summary>
        /// <param name="localeId">The locale ID.</param>
        /// <param name="value">The value to convert and format.</param>
        /// <param name="calcMethod">A delegate for a method that performs the value calculation.</param>
        /// <param name="unitRetrieveMenthod">A delegate for a method that can retrieve the unit text to use.</param>
        /// <param name="decimalPlaces">The optional amount of decimal places for the output (defaults to 0).</param>
        /// <returns>The returnable text-representation of the calculated value.</returns>
        private static string GetFormattedValue(string localeId, double value, Func<string, double, double> calcMethod, Func<string, string> unitRetrieveMenthod, int decimalPlaces = 0)
        {
            CheckUtil.ThrowIfNullOrEmpty(() => localeId);
            var culture = new CultureInfo(localeId);
            var formatPattern = "N" + decimalPlaces;
            var formatted = calcMethod(localeId, value).ToString(formatPattern, culture);
            return string.Format(culture, "{0} {1}", formatted, unitRetrieveMenthod(localeId));
        }

        /// <summary>
        /// Wrapper for methods in this type which will return formatted values depending on <paramref name="localeId" /> and
        /// <paramref name="decimalPlaces" />.
        /// </summary>
        /// <param name="localeId">The locale ID.</param>
        /// <param name="value">The value to convert and format.</param>
        /// <param name="calcMethod">A delegate for a method that performs the value calculation.</param>
        /// <param name="unit">The already resólved unit.</param>
        /// <param name="decimalPlaces">The optional amount of decimal places for the output (defaults to 0).</param>
        /// <returns>The returnable text-representation of the calculated value.</returns>
        private static string GetFormattedValue(string localeId, double value, Func<string, double, double> calcMethod, string unit, int decimalPlaces = 0)
        {
            CheckUtil.ThrowIfNullOrEmpty(() => localeId);
            var culture = new CultureInfo(localeId);
            var formatPattern = "{0:N" + decimalPlaces + "} {1}";
            var formatted = calcMethod(localeId, value).ToString(formatPattern, culture);
            return string.Format(culture, "{0} {1}", formatted, unit);
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
            CheckUtil.ThrowIfNullOrEmpty(() => localeId);           
            var culture = new CultureInfo(localeId);
            if (culture.IsNeutralCulture)
            {
                // Try to get the specific culture 
                throw new ArgumentException("Provided culture not specific!", nameof(localeId));
            }
            var result = new RegionInfo(localeId);
            return result;
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets/sets the altitude-unit-text for imperial outputs.
        /// </summary>
        public static string ImperialAltitudeUnit { get; set; } = "ft";

        /// <summary>
        /// Gets/sets the distance-unit-text for imperial outputs.
        /// </summary>
        public static string ImperialDistanceSmallUnit { get; set; } = "yd";

        /// <summary>
        /// Gets/sets the distance-unit-text for imperial outputs.
        /// </summary>
        public static string ImperialDistanceUnit { get; set; } = "mi";

        /// <summary>
        /// Gets/sets the height-unit-text for imperial outputs.
        /// </summary>
        public static string ImperialHeightUnit { get; set; } = "in";

        /// <summary>
        /// Gets/sets the speed-unit-text for imperial outputs.
        /// </summary>
        public static string ImperialSpeedUnit { get; set; } = "mph";

        /// <summary>
        /// Gets/sets the weight-unit-text for imperial outputs.
        /// </summary>
        public static string ImperialWeightUnit { get; set; } = "lbs";

        /// <summary>
        /// Gets/sets the altitude-unit-text for metric outputs.
        /// </summary>
        public static string MetricAltitudeUnit { get; set; } = "m";

        /// <summary>
        /// Gets/sets the distance-unit-text for metric outputs.
        /// </summary>
        public static string MetricDistanceSmallUnit { get; set; } = "m";

        /// <summary>
        /// Gets/sets the distance-unit-text for metric outputs.
        /// </summary>
        public static string MetricDistanceUnit { get; set; } = "km";

        /// <summary>
        /// Gets/sets the height-unit-text for metric outputs.
        /// </summary>
        public static string MetricHeightUnit { get; set; } = "cm";

        /// <summary>
        /// Gets/sets the speed-unit-text for metric outputs.
        /// </summary>
        public static string MetricSpeedUnit { get; set; } = "km/h";

        /// <summary>
        /// Gets/sets the weight-unit-text for metric outputs.
        /// </summary>
        public static string MetricWeightUnit { get; set; } = "kg";

        #endregion
    }
}
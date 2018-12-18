using System;
using System.Linq;

namespace codingfreaks.cfUtils.Logic.Core.Utilities
{
    using System;

    /// <summary>
    /// Provides methods for conversion of units.
    /// </summary>
    public static class UnitConversionCalculator
    {
        #region constants

        /// <summary>
        /// Defines the factor to calculate from m/s² to G-force.
        /// </summary>
        public const double AccelerationToG = 0.1019367991845056;

        /// <summary>
        /// Defines how many feets are included in one meter.
        /// </summary>
        public const double FeetsPerMeter = 3.2808399;

        /// <summary>
        /// Defines how many inches are included in one centimeter.
        /// </summary>
        public const double InchesPerCentimeter = 0.393701;

        /// <summary>
        /// Defines how many kilometers are included in one meter.
        /// </summary>
        public const double KilometersPerMeter = 0.001;

        /// <summary>
        /// Defines the factor to calculate from m/s to km/h.
        /// </summary>
        public const double MetersPerSecondToKilometersPerHourFactor = 3.6;

        /// <summary>
        /// Defines the factor to calculate from m/s to mph.
        /// </summary>
        public const double MetersPerSecondToMilesPerHourFactor = 2.23;

        /// <summary>
        /// Defines how many miles are included in one meter.
        /// </summary>
        public const double MilesPerMeter = 0.000621371192;

        /// <summary>
        /// Defines how many yards are included in one meter.
        /// </summary>
        public const double YardsPerMeter = 0.9144;

        /// <summary>
        /// Defines how many pounds are included in one kilogram.
        /// </summary>
        public const double PoundPerKilogram = 2.20462;

        /// <summary>
        /// Defines the amount of seconds per day.
        /// </summary>
        public const double SecondsPerDay = 86400;

        /// <summary>
        /// Defines the prroximate amount of seconds per calendar month.
        /// </summary>
        public const double SecondsPerMonth = 2592000;

        /// <summary>
        /// Defines the amount of seconds per calendar week.
        /// </summary>
        public const double SecondsPerWeek = 604800;

        /// <summary>
        /// Defines the approximate amount of seconds per calendar month.
        /// </summary>
        public const double SecondsPerYear = 31536000;

        #endregion

        #region properties

        /// <summary>
        /// Gets/sets the altitude-unit-text for metric outputs.
        /// </summary>
        public static string MetricAltitudeUnit { get; set; } = "m";

        /// <summary>
        /// Gets/sets the distance-unit-text for metric outputs.
        /// </summary>
        public static string MetricDistanceUnit { get; set; } = "km";

        /// <summary>
        /// Gets/sets the distance-unit-text for metric outputs.
        /// </summary>
        public static string MetricDistanceSmallUnit { get; set; } = "m";

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

        /// <summary>
        /// Gets/sets the altitude-unit-text for non-metric outputs.
        /// </summary>
        public static string NonMetricAltitudeUnit { get; set; } = "ft";

        /// <summary>
        /// Gets/sets the distance-unit-text for non-metric outputs.
        /// </summary>
        public static string NonMetricDistanceUnit { get; set; } = "mi";

        /// <summary>
        /// Gets/sets the distance-unit-text for non-metric outputs.
        /// </summary>
        public static string NonMetricDistanceSmallUnit { get; set; } = "yd";

        /// <summary>
        /// Gets/sets the height-unit-text for non-metric outputs.
        /// </summary>
        public static string NonMetricHeightUnit { get; set; } = "in";

        /// <summary>
        /// Gets/sets the speed-unit-text for non-metric outputs.
        /// </summary>
        public static string NonMetricSpeedUnit { get; set; } = "mph";

        /// <summary>
        /// Gets/sets the weight-unit-text for non-metric outputs.
        /// </summary>
        public static string NonMetricWeightUnit { get; set; } = "lbs";

        #endregion

        #region methods

        /// <summary>
        /// Converts a m/s²-acceleration to G force.
        /// </summary>
        /// <param name="metersPerSecondSquare">The acceleration in m/s².</param>
        /// <returns>The acceleration in G.</returns>
        public static double ConvertAccelerationToGForce(double metersPerSecondSquare)
        {
            return metersPerSecondSquare * AccelerationToG;
        }

        /// <summary>
        /// Calculates centimeters to inches.
        /// </summary>
        /// <param name="centiMeters">The length in cm.</param>
        /// <returns>The length in in.</returns>
        public static double ConvertCentimetersToInches(double centiMeters)
        {
            return centiMeters * InchesPerCentimeter;
        }

        /// <summary>
        /// Calculates kilogram to pounds.
        /// </summary>
        /// <param name="kiloGram">The weight in kg.</param>
        /// <returns>The weight in lbs.</returns>
        public static double ConvertKilogramToPounds(int kiloGram)
        {
            return kiloGram * PoundPerKilogram;
        }

        /// <summary>
        /// Calculates the speed in km/h for a given <paramref name="metersPerSecond"/>.
        /// </summary>
        /// <param name="metersPerSecond">The speed in m/s.</param>
        /// <returns>The speed in km/h.</returns>
        public static double ConvertMetersPerSecondToKilometersPerHour(int metersPerSecond)
        {
            return metersPerSecond * MetersPerSecondToKilometersPerHourFactor;
        }

        /// <summary>
        /// Calculates the speed in mp/h for a given <paramref name="metersPerSecond"/>.
        /// </summary>
        /// <param name="metersPerSecond">The speed in m/s.</param>
        /// <returns>The speed in mp/h.</returns>
        public static double ConvertMetersPerSecondToMilesPerHour(int metersPerSecond)
        {
            return metersPerSecond * MetersPerSecondToMilesPerHourFactor;
        }

        /// <summary>
        /// Calculates meters to feet.
        /// </summary>
        /// <param name="meters">The distance in m.</param>
        /// <returns>The distance in ft.</returns>
        public static double ConvertMetersToFeet(int meters)
        {
            return meters * FeetsPerMeter;
        }

        /// <summary>
        /// Calculates meters to miles.
        /// </summary>
        /// <param name="meters">The distance in m.</param>
        /// <returns>The distance in mi.</returns>
        public static double ConvertMetersToMiles(int meters)
        {
            return meters * MilesPerMeter;
        }

        /// <summary>
        /// Calculates meters to yards.
        /// </summary>
        /// <param name="meters">The distance in m.</param>
        /// <returns>The distance in yd.</returns>
        public static double ConvertMetersToYards(int meters)
        {
            return meters * YardsPerMeter;
        }

        /// <summary>
        /// Calculates meters to miles.
        /// </summary>
        /// <param name="meters">The distance in m.</param>
        /// <returns>The distance in mi.</returns>
        public static double ConvertMetersToMiles(long meters)
        {
            return meters * MilesPerMeter;
        }

        /// <summary>
        /// Retrieves the city including country and zip out of a Racety default location-string.
        /// </summary>
        /// <param name="original"></param>
        /// <returns>The name of the city of </returns>
        public static string GetPlaceCity(string original)
        {
            if (string.IsNullOrEmpty(original))
            {
                return string.Empty;
            }
            var parts = original.Split(';');
            return parts.Length >= 3 ? string.Format("{0} ({1}{2})", parts[2], parts[0], parts[1].Length >= 2 ? parts[1].Substring(0, 2) : parts[1]) : string.Empty;
        }

        /// <summary>
        /// Takes an iOS-Place-information and converts it to a more readable format.
        /// </summary>
        /// <param name="original">The iOS-version.</param>
        /// <returns>The readable version.</returns>
        public static string GetPlaceString(string original)
        {
            if (string.IsNullOrEmpty(original))
            {
                return string.Empty;
            }
            var retVal = original;
            var parts = original.Split(';');
            if (parts.Length >= 5)
            {
                // 0 is Country
                // 1 is Zip
                // 2 is City
                // 3 is Street
                // 4 is Housenumber or additional
                retVal = string.Format("{0}-{1} {2}, {3} {4}", parts[0], parts[1], parts[2], parts[3], parts[4]);
            }
            return retVal;
        }

        #endregion
    }
}
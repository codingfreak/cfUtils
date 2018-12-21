namespace codingfreaks.cfUtils.Logic.Core.Utilities
{
    using System;
    using System.Linq;

    /// <summary>
    /// Provides methods for conversion of units.
    /// </summary>
    public static class UnitConversionCalculator
    {
        #region methods

        /// <summary>
        /// Converts a m/s²-acceleration to G force.
        /// </summary>
        /// <param name="metersPerSecondSquare">The acceleration in m/s².</param>
        /// <returns>The acceleration in G.</returns>
        public static double ConvertAccelerationToGForce(double metersPerSecondSquare)
        {
            return metersPerSecondSquare * Constants.AccelerationToG;
        }

        /// <summary>
        /// Calculates centimeters to inches.
        /// </summary>
        /// <param name="centiMeters">The length in cm.</param>
        /// <returns>The length in in.</returns>
        public static double ConvertCentimetersToInches(double centiMeters)
        {
            return centiMeters * Constants.InchesPerCentimeter;
        }

        /// <summary>
        /// Calculates kilogram to pounds.
        /// </summary>
        /// <param name="kiloGram">The weight in kg.</param>
        /// <returns>The weight in lbs.</returns>
        public static double ConvertKilogramToPounds(double kiloGram)
        {
            return kiloGram * Constants.PoundPerKilogram;
        }

        /// <summary>
        /// Calculates the speed in km/h for a given <paramref name="metersPerSecond" />.
        /// </summary>
        /// <param name="metersPerSecond">The speed in m/s.</param>
        /// <returns>The speed in km/h.</returns>
        public static double ConvertMetersPerSecondToKilometersPerHour(double metersPerSecond)
        {
            return metersPerSecond * Constants.MetersPerSecondToKilometersPerHourFactor;
        }

        /// <summary>
        /// Calculates the speed in mp/h for a given <paramref name="metersPerSecond" />.
        /// </summary>
        /// <param name="metersPerSecond">The speed in m/s.</param>
        /// <returns>The speed in mp/h.</returns>
        public static double ConvertMetersPerSecondToMilesPerHour(double metersPerSecond)
        {
            return metersPerSecond * Constants.MetersPerSecondToMilesPerHourFactor;
        }

        /// <summary>
        /// Calculates meters to feet.
        /// </summary>
        /// <param name="meters">The distance in m.</param>
        /// <returns>The distance in ft.</returns>
        public static double ConvertMetersToFeet(double meters)
        {
            return meters * Constants.FeetsPerMeter;
        }

        /// <summary>
        /// Calculates meters to miles.
        /// </summary>
        /// <param name="meters">The distance in m.</param>
        /// <returns>The distance in mi.</returns>
        public static double ConvertMetersToMiles(double meters)
        {
            return meters * Constants.MilesPerMeter;
        }

        /// <summary>
        /// Calculates meters to yards.
        /// </summary>
        /// <param name="meters">The distance in m.</param>
        /// <returns>The distance in yd.</returns>
        public static double ConvertMetersToYards(double meters)
        {
            return meters * Constants.YardsPerMeter;
        }

        /// <summary>
        /// Takes an iOS-Place-information and converts it to a more readable format.
        /// </summary>
        /// <param name="original">The iOS-version.</param>
        /// <returns>The readable version.</returns>
        public static string GetApplePlaceString(string original)
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
                retVal = $"{parts[0]}-{parts[1]} {parts[2]}, {parts[3]} {parts[4]}";
            }
            return retVal;
        }

        #endregion
    }
}
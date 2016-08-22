using System;
using System.Linq;

namespace codingfreaks.cfUtils.Logic.Portable.Utilities
{
    /// <summary>
    /// Provides methods regarding georgraphy calculation.
    /// </summary>
    public static class SimpleGeographyUtil
    {
        #region methods

        /// <summary>
        /// Converts a given <paramref name="distance" /> (in kilometers) into latitude degrees.
        /// </summary>
        /// <param name="distance">The distance in km.</param>
        /// <returns>The latitude degrees.</returns>
        public static double KilometresToLatitudeDegrees(double distance)
        {
            return distance / Constants.EarthRadiusInKilometer * Constants.RadiansToDegrees;
        }

        /// <summary>
        /// Converts a given <paramref name="distance" /> (in kilometers) into longitude degrees.
        /// </summary>
        /// <param name="distance">The distance in km.</param>
        /// <param name="latitude">The latitude at which to measure the length.</param>
        /// <returns>The longitude degrees.</returns>
        public static double KilometresToLongitudeDegrees(double distance, double latitude)
        {
            var radiusAtLatitude = Constants.EarthRadiusInKilometer * Math.Cos(latitude * Constants.DegreesToRadians);
            return distance / radiusAtLatitude * Constants.RadiansToDegrees;
        }

        #endregion
    }
}
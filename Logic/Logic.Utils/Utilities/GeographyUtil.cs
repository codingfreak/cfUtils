namespace codingfreaks.cfUtils.Logic.Utils.Utilities
{
    using System.Data.Entity.Spatial;
    using System.Device.Location;
    using System.Globalization;

    /// <summary>
    /// Provides helper methods for dealing with geography-types.
    /// </summary>
    public static class GeographyUtil
    {
        #region methods

        /// <summary>
        /// Generates a geography type using given <paramref name="latitude"/> and <paramref name="longitude"/>.
        /// </summary>        
        /// <param name="latitude">The latitude of the position.</param>
        /// <param name="longitude">The longitude of the position.</param>
        /// <param name="coordinateSystemId">The coordination system id (defaults to 4326).</param>
        /// <returns>The appropriate geography element.</returns>
        public static DbGeography FromLatLon(double latitude, double longitude, int coordinateSystemId = 4326)
        {
            return DbGeography.PointFromText($"POINT({longitude.ToString(CultureInfo.InvariantCulture)} {latitude.ToString(CultureInfo.InvariantCulture)})", coordinateSystemId);
        }

        /// <summary>
        /// Calculates the shortest distance in meters between two coordinates.
        /// </summary>
        /// <param name="currentPoint">The point from wich to start the calculation.</param>
        /// <param name="lastPoint">The point at which the distance should end.</param>
        /// <returns>The distance between the provided points in meters.</returns>
        public static double GetDistance(GeoCoordinate currentPoint, GeoCoordinate lastPoint)
        {
            return currentPoint.GetDistanceTo(lastPoint);
        }

        /// <summary>
        /// Calculates the shortest distance in meters between two database coordinates.
        /// </summary>
        /// <param name="currentPoint">The point from wich to start the calculation.</param>
        /// <param name="lastPoint">The point at which the distance should end.</param>
        /// <returns>The distance between the provided points in meters.</returns>
        public static double GetDistance(DbGeography currentPoint, DbGeography lastPoint)
        {
            return GetDistance(GetCoordinate(currentPoint), GetCoordinate(lastPoint));
        }

        /// <summary>
        /// Generates a <see cref="GeoCoordinate"/> out of a <paramref name="lat"/> and <paramref name="lon"/>.
        /// </summary>
        /// <param name="lat">The latitude of the position.</param>
        /// <param name="lon">The longitude of the position.</param>
        /// <returns>A coordinate.</returns>
        private static GeoCoordinate GetCoordinate(double lat, double lon)
        {
            return new GeoCoordinate(lat, lon);
        }

        /// <summary>
        /// Generates a <see cref="GeoCoordinate"/> out of a database location.
        /// </summary>
        /// <param name="location">The location as it comes from the database.</param>
        /// <returns>A coordinate.</returns>
        private static GeoCoordinate GetCoordinate(DbGeography location)
        {
            return new GeoCoordinate(location.Latitude ?? 0, location.Longitude ?? 0);
        }

        #endregion
    }
}
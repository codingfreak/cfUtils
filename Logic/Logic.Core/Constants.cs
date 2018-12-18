namespace codingfreaks.cfUtils.Logic.Core
{
    using System;
    using System.Linq;

    /// <summary>
    /// Contains constant values for global use.
    /// </summary>
    public static class Constants
    {
        #region constants

        /// <summary>
        /// Defines the factor to calculate from m/s² to G-force.
        /// </summary>
        public const double AccelerationToG = 0.1019367991845056;

        /// <summary>
        /// Conversion unit to get degrees from radians.
        /// </summary>
        public const double DegreesToRadians = Math.PI / 180.0;

        /// <summary>
        /// The earth radius in kilometers.
        /// </summary>
        public const double EarthRadiusInKilometer = 6371.0;

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
        /// The global regex-pattern for mail-address-check.
        /// </summary>
        public const string MatchEmailPattern = @"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*@((([\-\w]+\.)+[a-zA-Z]{2,10})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$";

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
        /// Defines how many pounds are included in one kilogram.
        /// </summary>
        public const double PoundPerKilogram = 2.20462;

        /// <summary>
        /// Conversion unit to get radians from degrees.
        /// </summary>
        public const double RadiansToDegrees = 180.0 / Math.PI;

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

        /// <summary>
        /// Defines how many yards are included in one meter.
        /// </summary>
        public const double YardsPerMeter = 0.9144;

        #endregion
    }
}
namespace codingfreaks.cfUtils.Logic.Standard
{
    using System;

    /// <summary>
    /// Contains constant values for global use.
    /// </summary>
    public static class Constants
    {
        #region constants

        /// <summary>
        /// The global regex-pattern for mail-address-check.
        /// </summary>
        public const string MatchEmailPattern = @"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*@((([\-\w]+\.)+[a-zA-Z]{2,10})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$";

        /// <summary>
        /// Conversion unit to get degrees from radians.
        /// </summary>
        public const double DegreesToRadians = Math.PI / 180.0;

        /// <summary>
        /// The earth radius in kilometers.
        /// </summary>
        public const double EarthRadiusInKilometer = 6371.0;

        /// <summary>
        /// Conversion unit to get radians from degrees.
        /// </summary>
        public const double RadiansToDegrees = 180.0 / Math.PI;

        #endregion
    }
}
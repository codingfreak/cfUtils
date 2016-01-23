namespace codingfreaks.cfUtils.Logic.Portable
{
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

        #endregion
    }
}
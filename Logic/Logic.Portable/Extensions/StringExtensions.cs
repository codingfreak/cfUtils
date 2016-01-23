namespace s2.s2Utils.Logic.Portable.Extensions
{
    using System;
    using System.Text;
    using System.Text.RegularExpressions;

    using s2.s2Utils.Logic.Portable;

    /// <summary>
    /// Collects extension methods for the type <see cref="System.String"/>.
    /// </summary>
    public static class StringExtensions
    {
        #region methods

        /// <summary>
        /// Checks, if the <paramref name="original"/> is not null or empty.
        /// </summary>
        /// <param name="original">The original string.</param>
        /// <returns><c>true</c> if the content has at least one char, otherwise <c>false</c>.</returns>
        public static bool HasContent(this string original)
        {
            return !string.IsNullOrEmpty(original);
        }

        /// <summary>
        /// Checks, if the <paramref name="original"/> is null or empty.
        /// </summary>
        /// <param name="original">The original string.</param>
        /// <returns><c>true</c> if the content has not even one, otherwise <c>false</c>.</returns>
        public static bool IsNullOrEmpty(this string original)
        {
            return string.IsNullOrEmpty(original);
        }

        /// <summary>
        /// Checks, if the <paramref name="original"/> is a valid E-Mail-Address.
        /// </summary>
        /// <param name="original">The email-address to check.</param>
        /// <returns><c>True</c> if the <paramref name="original"/> was recognized as an mail-address, otherwise <c>false</c>.</returns>
        public static bool IsValidEmailAddress(this string original)
        {
            return !string.IsNullOrEmpty(original) && Regex.IsMatch(original, Constants.MatchEmailPattern);
        }

        /// <summary>
        /// Extension method for <see cref="System.String"/> which wraps converting a string to its Base64-pendent.
        /// </summary>
        /// <remarks>
        /// <para>Uses the UTF-8 encoding.</para>
        /// <para>Is very useful when you want to convert a string which should be passed via an URL e.g.</para>
        /// </remarks>
        /// <param name="original">The string to convert to Base64.</param>
        /// <returns>The Base64-encoded string.</returns>
        public static string ToBase64String(this string original)
        {
            return string.IsNullOrEmpty(original) ? null : Convert.ToBase64String(Encoding.UTF8.GetBytes(original));
        }

        /// <summary>
        /// Converts the first position of a string into a char.
        /// </summary>
        /// <param name="val">The original string.</param>
        /// <returns>The first position of the <paramref name="val"/> as a char.</returns>
        public static char ToChar(this string val)
        {
            if (!string.IsNullOrEmpty(val))
            {
                return val.ToCharArray()[0];
            }
            return default(Char);
        }

        #endregion
    }
}
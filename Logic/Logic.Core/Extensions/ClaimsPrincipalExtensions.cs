namespace codingfreaks.cfUtils.Logic.Core.Extensions
{
    using System;
    using System.Linq;
    using System.Security.Claims;

    /// <summary>
    /// Extends the type <see cref="ClaimsPrincipal" />.
    /// </summary>
    public static class ClaimsPrincipalExtensions
    {
        #region methods

        /// <summary>
        /// Tries to read a value with a given <paramref name="type" /> from the <paramref name="user" />.
        /// </summary>
        /// <param name="user">The claim based user.</param>
        /// <param name="type">The type of the property to read coming from <see cref="ClaimTypes" /> constants.</param>
        /// <param name="defaultValue">The default value, if reading fails.</param>
        /// <returns>The result.</returns>
        public static string GetPropertyValue(this ClaimsPrincipal user, string type, string defaultValue = "")
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            try
            {
                var item = user.FindFirst(type);
                return item == null ? defaultValue : item.Value;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Could not find claim.", ex);
            }
        }

        #endregion
    }
}
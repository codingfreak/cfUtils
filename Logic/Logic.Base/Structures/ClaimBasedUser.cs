namespace codingfreaks.cfUtils.Logic.Base.Structures
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading;

    using codingfreaks.cfUtils.Logic.Base.Extensions;
    using codingfreaks.cfUtils.Logic.Portable.Utilities;

    using Utilities;

    /// <summary>
    /// Represents a user read from ClaimsPrincipal.
    /// </summary>
    public class ClaimBasedUser
    {
        #region methods

        /// <summary>
        /// Creates a fresh instance using the <paramref name="thread" /> taking the user from it.
        /// </summary>
        /// <param name="thread">The thread holding the user.</param>
        /// <returns>The user or <c>null</c> if none could be determined.</returns>
        public static ClaimBasedUser Create(Thread thread)
        {
            return Create(Thread.CurrentPrincipal as ClaimsPrincipal);
        }

        /// <summary>
        /// Creates a fresh instance using the <paramref name="user" />.
        /// </summary>
        /// <param name="user">The claim principal user.</param>
        /// <returns>The user or <c>null</c> if none could be determined.</returns>
        public static ClaimBasedUser Create(ClaimsPrincipal user)
        {
            if (user == null)
            {
                return null;
            }
            var result = new ClaimBasedUser
            {
                Values = new Dictionary<string, string>()
            };
            typeof(ClaimTypes).GetPublicConstants().ToList().ForEach(
                c =>
                {
                    result.Values.Add(c.Name, user.GetPropertyValue(c.GetRawConstantValue().ToString()));
                });
            return result;
        }

        /// <summary>
        /// TRies to read a value with a given <paramref name="key" /> from <see cref="Values" />.
        /// </summary>
        /// <param name="key">The key inside the <see cref="Values" />.</param>
        /// <returns>The value or <see cref="string.Empty" />.</returns>
        private string TryGetValue(string key)
        {
            var result = string.Empty;
            if (Values != null)
            {
                Values.TryGetValue(key, out result);
            }
            return result;
        }

        #endregion

        #region properties

        /// <summary>
        /// Convenient access to the firstname in <see cref="Values" />.
        /// </summary>
        public string Firstname
        {
            get
            {
                return TryGetValue("GivenName");
            }
        }

        /// <summary>
        /// Convenient access to the lastname in <see cref="Values" />.
        /// </summary>
        public string Lastname
        {
            get
            {
                return TryGetValue("Surname");
            }
        }

        /// <summary>
        /// Holds all values coming from the original user.
        /// </summary>
        public Dictionary<string, string> Values { get; set; }

        #endregion
    }
}
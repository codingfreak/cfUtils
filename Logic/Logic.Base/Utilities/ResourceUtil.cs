namespace s2.s2Utils.Logic.Base.Utilities
{
    using System;
    using System.Globalization;
    using System.Resources;

    /// <summary>
    /// Provides wrapping logic for accessing ressources.
    /// </summary>
    public static class ResourceUtil
    {
        #region properties

        /// <summary>
        /// Must be set by the caller once to define how this util will resolve the <see cref="ResourceManager"/>.
        /// </summary>      
        /// <remarks>
        /// The provided value should be an Enum in the caller.
        /// </remarks> 
        public static Func<int, ResourceManager> ManagerResolver { get; set; }

        #endregion

        #region methods

        /// <summary>
        /// Tries to retrieve a ressource string with a given <paramref name="resourceKey"/> for a given <paramref name="localeId"/> out of
        /// the default resources.
        /// </summary>
        /// <param name="localeId">The ISO-conform identifier of the culture (xx or xx-XX).</param>
        /// <param name="resourceKey">The unique key of a resource inside the default resx.</param>
        /// <param name="args">Arguments to apply to the resource identified by the <paramref name="resourceKey"/>.</param>
        /// <returns>The formatted text or <c>string.Empty</c> on error.</returns>
        public static string Format(string localeId, string resourceKey, params object[] args)
        {
            return Format(localeId, 0, resourceKey, args);
        }

        /// <summary>
        /// Tries to retrieve a ressource string with a given <paramref name="resourceKey"/> for a given <paramref name="localeId"/> out of
        /// the resources specified by the <paramref name="resourceType"/>.
        /// </summary>
        /// <param name="localeId">The ISO-conform identifier of the culture (xx or xx-XX).</param>
        /// <param name="resourceType">The target resource where the search should occur.</param>
        /// <param name="resourceKey">The unique key of a resource inside the resx targeted by <paramref name="resourceType"/>.</param>        
        /// <param name="args">Arguments to apply to the resource identified by the <paramref name="resourceKey"/>.</param>
        /// <returns>The formatted text or <c>string.Empty</c> on error.</returns>
        public static string Format(string localeId, int resourceType, string resourceKey, params object[] args)
        {
            var format = Get(localeId, resourceKey, resourceType);
            if (string.IsNullOrEmpty(format))
            {
                // no format obtained from resources
                return string.Empty;
            }
            CultureInfo cultureInfo;
            if (localeId.TryToCultureInfo(out cultureInfo))
            {
                // all variables obtained
                return string.Format(cultureInfo, format, args);
            }
            // something went wrong
            return string.Empty;
        }

        /// <summary>
        /// Tries to retrieve a ressource <typeparamref name="T"/> with a given <paramref name="resourceKey"/> for the current UI culture out of
        /// the resources specified by the <paramref name="resourceType"/>.
        /// </summary>        
        /// <param name="resourceKey">The unique key of a resource inside the resx targeted by <paramref name="resourceType"/>.</param>
        /// <param name="resourceType">The target resource where the search should occur.</param>
        /// <returns>The value or <c>null</c> if an error occurs.</returns>
        public static T Get<T>(string resourceKey, int resourceType = 0)
        {
            return Get<T>(CultureInfo.CurrentUICulture.Name, resourceKey, resourceType);
        }

        /// <summary>
        /// Tries to retrieve a ressource <typeparamref name="T"/> with a given <paramref name="resourceKey"/> for a given <paramref name="localeId"/> out of
        /// the resources specified by the <paramref name="resourceType"/>.
        /// </summary>
        /// <param name="localeId">The ISO-conform identifier of the culture (xx or xx-XX).</param>
        /// <param name="resourceKey">The unique key of a resource inside the resx targeted by <paramref name="resourceType"/>.</param>
        /// <param name="resourceType">The target resource where the search should occur.</param>
        /// <returns>The value or <c>null</c> if an error occurs.</returns>
        public static T Get<T>(string localeId, string resourceKey, int resourceType = 0)
        {
            if (resourceKey == null)
            {
                // cannot search for non-given key.
                return default(T);
            }
            var manager = ManagerResolver(resourceType);
            if (manager == null)
            {
                // manager not found
                return default(T);
            }
            CultureInfo cultureInfo;
            if (localeId.TryToCultureInfo(out cultureInfo))
            {
                // culture was valid                
                return (T)manager.GetObject(resourceKey, cultureInfo); 
            }
            // culture invalid
            return default(T);
        }

        /// <summary>
        /// Tries to retrieve a ressource string with a given <paramref name="resourceKey"/> for a given <paramref name="localeId"/> out of
        /// the resources specified by the <paramref name="resourceType"/>.
        /// </summary>
        /// <param name="localeId">The ISO-conform identifier of the culture (xx or xx-XX).</param>
        /// <param name="resourceKey">The unique key of a resource inside the resx targeted by <paramref name="resourceType"/>.</param>
        /// <param name="resourceType">The target resource where the search should occur.</param>
        /// <returns>The value or <c>null</c> if an error occurs.</returns>
        public static string Get(string localeId, string resourceKey, int resourceType = 0)
        {
            return Get<string>(localeId, resourceKey, resourceType);
        }

        /// <summary>
        /// Extension method which allows to convert a given <paramref name="localeId"/> to a valid <see cref="CultureInfo"/>.
        /// </summary>
        /// <param name="localeId">The ISO-conform identifier of the culture (xx or xx-XX).</param>
        /// <returns>The ready-to-use <see cref="CultureInfo"/>. If an error occurs an exception is thrown.</returns>
        public static CultureInfo ToCultureInfo(this string localeId)
        {
            if (string.IsNullOrEmpty(localeId))
            {
                throw new CultureNotFoundException("localeId", localeId, "Invalid localeId.");
            }
            return new CultureInfo(localeId);
        }

        /// <summary>
        /// Extension method which allows to convert a given <paramref name="localeId"/> to a valid <see cref="CultureInfo"/>.
        /// </summary>
        /// <param name="localeId">The ISO-conform identifier of the culture (xx or xx-XX).</param>
        /// <param name="result">The resulting <see cref="CultureInfo"/> or <c>null</c> if the operation fails.</param>
        /// <returns><b>true</b> if the operation succeeds otherwise <c>false</c>. If <c>false</c> is returned, <paramref name="result"/> will be <c>null</c>.</returns>
        public static bool TryToCultureInfo(this string localeId, out CultureInfo result)
        {
            result = null;
            if (string.IsNullOrEmpty(localeId))
            {
                return false;
            }
            try
            {
                result = new CultureInfo(localeId);
                return true;
            }
            catch
            {    
                return false;
            }
        }

        #endregion
    }
}
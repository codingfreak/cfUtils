namespace codingfreaks.cfUtils.Logic.Base.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Globalization;

    using codingfreaks.cfUtils.Logic.Standard.Utilities;

    /// <summary>
    /// Provides logic for accessing .NET configurations.
    /// </summary>
    public static class ConfigurationUtil
    {
        #region static fields

        /// <summary>
        /// Holds a collection of all application settings from the calling assemblies configuration.
        /// </summary>
        private static readonly NameValueCollection AppSettings = ConfigurationManager.AppSettings;

        /// <summary>
        /// Holds a collection of all connection strings from the calling assemblies configuration.
        /// </summary>
        private static readonly ConnectionStringSettingsCollection ConnectionSettings = ConfigurationManager.ConnectionStrings;

        #endregion

        #region methods

        /// <summary>
        /// Searches for an app setting with the provided <paramref name="key"/> from the calling configuration and returns its value converted to <typeparamref name="T"/>.        
        /// </summary>
        /// <remarks>
        /// This method will throw exceptions on any failure.
        /// </remarks>
        /// <param name="key">The unique key out of the app-settings.</param>        
        /// <typeparam name="T">Target type which has to be an <see cref="IConvertible"/>.</typeparam>        
        /// <returns>The value of type <typeparamref name="T"/> if one could be obtained.</returns>
        public static T Get<T>(string key) where T : IConvertible
        {
            CheckUtil.ThrowIfNull(() => key);
            var value = AppSettings[key];
            if (value == null)
            {
                var error = string.Format(CultureInfo.InvariantCulture, "Cannot find key '{0}' in config-file.", key);
                throw new KeyNotFoundException(error);
            }
            try
            {
                var result = (T)Convert.ChangeType(value, typeof(T), CultureInfo.CurrentCulture);
                return result;
            }
            catch (Exception ex)
            {
                var error = string.Format(CultureInfo.InvariantCulture, "Cannot convert '{0}' to type '{1}': {2}", value, typeof(T), ex);
                throw new InvalidOperationException(error, ex);
            }
        }

        /// <summary>
        /// Searches for an app setting with the provided <paramref name="key"/> from the calling configuration and returns its value converted to <typeparamref name="T"/>.        
        /// </summary>
        /// <remarks>
        /// This method will throw exceptions on any failure except if the key is not found.
        /// </remarks>
        /// <param name="key">The unique key out of the app-settings.</param>
        /// <param name="defaultValue">A default value to use, if the key is not set.</param>
        /// <typeparam name="T">Target type which has to be an <see cref="IConvertible"/>.</typeparam>        
        /// <returns>The value of type <typeparamref name="T"/> if one could be obtained, otherwise the <paramref name="defaultValue"/>.</returns>
        public static T Get<T>(string key, T defaultValue) where T : IConvertible
        {
            try
            {
                return Get<T>(key);
            }
            catch (KeyNotFoundException)
            {
                return defaultValue;
            }
            catch
            {
                return default(T);
            }            
        }

        /// <summary>
        /// Tries to retrieve a configuration section from the current calling assembly.
        /// </summary>
        /// <param name="sectionName">The name of the section in the configuration.</param>
        /// <typeparam name="T">A type which derices from <see cref="ConfigurationSection"/>.</typeparam>
        /// <returns>The section or <c>null</c> if no matching section was found.</returns>
        public static T GetConfigurationSection<T>(string sectionName) where T : ConfigurationSection
        {
            var section = ConfigurationManager.GetSection(sectionName) as T;
            if (section == null)
            {
                // Either no section with the name contextOptions was found or the section could not
                // be converted to the type
                return null;
            }
            // Valid section was found.
            return section;
        }

        /// <summary>
        /// Retrieves the connections string identified by the <paramref name="key"/>.
        /// </summary>
        /// <remarks>
        /// This method will throw exceptions on any failure.
        /// </remarks>
        /// <param name="key">The unique key out of the connection string.</param>        
        /// <returns>The connection string for the <paramref name="key"/>.</returns>
        public static string GetConnectionString(string key)
        {
            CheckUtil.ThrowIfNullOrWhitespace(() => key);
            var connectionString = ConnectionSettings[key];
            if (connectionString == null)
            {
                var error = string.Format(CultureInfo.InvariantCulture, "Cannot find connection-string '{0}' in config-file.", key);
                throw new InvalidOperationException(error);
            }
            return connectionString.ConnectionString;
        }

        /// <summary>
        /// Retrieves the provider name of a connection string identified by the <paramref name="key"/>.
        /// </summary>
        /// <remarks>
        /// This method will throw exceptions on any failure.
        /// </remarks>
        /// <param name="key">The unique key out of the connection string.</param>        
        /// <returns>The provider name for the <paramref name="key"/>.</returns>
        public static string GetProviderName(string key)
        {
            CheckUtil.ThrowIfNullOrWhitespace(() => key);
            var connectionString = ConnectionSettings[key];
            if (connectionString == null)
            {
                var error = string.Format(CultureInfo.InvariantCulture, "Cannot find connection-string '{0}' in config-file.", key);
                throw new InvalidOperationException(error);
            }
            return connectionString.ProviderName;
        }

        /// <summary>
        /// Searches for an app setting with the provided <paramref name="key"/> from the calling configuration and tries to return 
        /// its value converted to <typeparamref name="T"/>.        
        /// </summary>
        /// <remarks>
        /// This method will thor exception 
        /// </remarks>
        /// <param name="key">The unique key out of the app-settings.</param>
        /// <param name="defaultValue"></param>
        /// <typeparam name="T">Target type which has to be an <see cref="IConvertible"/>.</typeparam>        
        /// <returns>The value of type <typeparamref name="T"/> or <paramref name="defaultValue"/> if no value could be obtained.</returns>
        public static T TryGet<T>(string key, T defaultValue) where T : IConvertible
        {
            CheckUtil.ThrowIfNullOrWhitespace(() => key);
            var value = AppSettings[key];
            if (value == null)
            {
                // there was no key inside the settings, so retrieve the default
                return defaultValue;
            }
            try
            {
                var result = (T)Convert.ChangeType(value, typeof(T), CultureInfo.CurrentCulture);
                return result;
            }
            catch
            {
                return defaultValue;
            }
        }

        #endregion
    }
}
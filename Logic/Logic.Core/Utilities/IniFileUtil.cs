using System;
using System.Linq;

namespace codingfreaks.cfUtils.Logic.Core.Utilities
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// Provides logic for handling Windows INI files.
    /// </summary>
    public static class IniFileUtil
    {
        #region methods

        /// <summary>
        /// Retrieves the value for a given <paramref name="itemName"/> inside a <paramref name="groupName"/> from a <paramref name="fileName"/>.
        /// </summary>
        /// <param name="fileName">The full path to the INI-file.</param>
        /// <param name="groupName">The name of the group in which the setting is expected or <c>null</c> if the item is in no group.</param>
        /// <param name="itemName">The name of the item.</param>
        /// <param name="defaultValue">A default value if no value could be found.</param>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <returns>The value associated with the <paramref name="itemName"/>.</returns>
        /// <exception cref="FileNotFoundException">Is thrown if the file could not be found.</exception>
        /// <exception cref="InvalidOperationException">Is thrown in case of any other exception.</exception>
        public static T GetValue<T>(string fileName, string groupName, string itemName, T defaultValue = default(T))
        {
            CheckUtil.ThrowIfNullOrEmpty(() => fileName);
            CheckUtil.ThrowIfNullOrEmpty(() => itemName);
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException("Provided file not found.", fileName);
            }
            string[] linesInFile;
            try
            {
                linesInFile = File.ReadAllLines(fileName);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error during read of file.", ex);
            }
            if (linesInFile == null || !linesInFile.Any())
            {
                throw new InvalidOperationException("Could not read from file.");
            }
            var groupSearch = string.Format(CultureInfo.InvariantCulture, "[{0}]", groupName);
            var groupFound = !string.IsNullOrEmpty(groupName) && linesInFile.Any(line => line.Equals(groupSearch, StringComparison.OrdinalIgnoreCase));
            if (!groupFound)
            {
                return defaultValue;
            }
            var inGroup = !string.IsNullOrEmpty(groupName);
            var retVal = defaultValue;
            linesInFile.ToList().ForEach(
                line =>
                {
                    if (line.StartsWith("["))
                    {
                        // check if we will start the parsing or if we stop the complete process
                        if (inGroup)
                        {
                            // we where already inside the desired group
                            return;
                        }
                        inGroup = line.Equals(groupSearch, StringComparison.OrdinalIgnoreCase);
                    }
                    else
                    {
                        if (!inGroup || !line.StartsWith(itemName, StringComparison.OrdinalIgnoreCase))
                        {
                            return;
                        }
                        // this is the item we are looking for
                        var value = line.Split('=')[1].Trim();
                        if (string.IsNullOrEmpty(value))
                        {
                            return;
                        }
                        try
                        {
                            retVal = (T)Convert.ChangeType(value, typeof(T));
                        }
                        catch (Exception ex)
                        {
                            TraceUtil.WriteTraceError(ex.Message);
                        }
                    }
                });
            return retVal;
        }

        #endregion
    }
}
namespace codingfreaks.cfUtils.Logic.Base.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Provides logic for parsing and generating CSV-files.
    /// </summary>
    public static class CsvUtil
    {
        #region methods

        /// <summary>
        /// Checks if a given <paramref name="fileUri" /> can be interpreted as CSV without any errors.
        /// </summary>
        /// <param name="fileUri">The location of the CSV file.</param>
        /// <param name="containsHeaders"><c>true</c> if headers are present in the topmose line.</param>
        /// <param name="separator">The char which seperates columns.</param>
        /// <returns><c>true</c> if the file is valid otherwise <c>false</c>.</returns>
        public static bool IsValidCsvFile(string fileUri, bool containsHeaders = false, char separator = ',')
        {
            return IsValidCsvFile(fileUri, Encoding.Default, containsHeaders, separator);
        }

        /// <summary>
        /// Checks if a given <paramref name="fileUri" /> can be interpreted as CSV without any errors.
        /// </summary>
        /// <param name="fileUri">The location of the CSV file.</param>
        /// <param name="encoding">The encoding to use.</param>
        /// <param name="containsHeaders"><c>true</c> if headers are present in the topmose line.</param>
        /// <param name="separator">The char which seperates columns.</param>
        /// <returns><c>true</c> if the file is valid otherwise <c>false</c>.</returns>
        public static bool IsValidCsvFile(string fileUri, Encoding encoding, bool containsHeaders = false, char separator = ',')
        {
            if (!File.Exists(fileUri))
            {
                throw new FileNotFoundException("Provided file not found.", fileUri);
            }
            string[] lines = null;
            try
            {
                lines = File.ReadAllLines(fileUri, encoding);
            }
            catch
            {
            }
            if (lines == null || !lines.Any())
            {
                return false;
            }
            var firstLine = lines[0].Split(separator);
            if (!firstLine.Any())
            {
                return false;
            }
            var result = true;
            var lineNo = 1;
            foreach (var line in lines)
            {
                result &= line.Split(separator).Count() == firstLine.Count();
                lineNo++;
                if (!result)
                {
                    TraceUtil.WriteTraceError("Line #{0}", lineNo);
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// Retrieves an enumerator for the provided <paramref name="fileUri" /> where each result will contain a key-value-pair
        /// with the column-name as the key and the corresponding value.
        /// </summary>
        /// <remarks>
        /// Each line returned will consist of as many <see cref="KeyValuePair{TKey,TValue}" /> as there are fields in the line.
        /// If there are any errors regarding the field-count the error-line will be omitted in result if
        /// <paramref name="checkValidity" /> is <c>false</c>.
        /// </remarks>
        /// <param name="fileUri">The location of the CSV file.</param>
        /// <param name="containsHeaders"><c>true</c> if headers are present in the topmose line.</param>
        /// <param name="separator">The char which seperates columns.</param>
        /// <param name="checkValidity">
        /// Indicates if this method should return <c>null</c>, if the file did not pass
        /// <see cref="IsValidCsvFile(string,bool,char)" />.
        /// </param>
        /// <returns>All items inside a key-value-structure as an enumerator.</returns>
        public static IEnumerable<IEnumerable<KeyValuePair<string, string>>> ParseFile(string fileUri, bool containsHeaders = false, char separator = ',', bool checkValidity = false)
        {
            return ParseFile(fileUri, Encoding.Default, containsHeaders, separator, checkValidity);
        }

        /// <summary>
        /// Retrieves an enumerator for the provided <paramref name="fileUri" /> where each result will contain a key-value-pair
        /// with the column-name as the key and the corresponding value.
        /// </summary>
        /// <remarks>
        /// Each line returned will consist of as many <see cref="KeyValuePair{TKey,TValue}" /> as there are fields in the line.
        /// If there are any errors regarding the field-count the error-line will be omitted in result if
        /// <paramref name="checkValidity" /> is <c>false</c>.
        /// </remarks>
        /// <param name="fileUri">The location of the CSV file.</param>
        /// <param name="encoding">The encoding to use.</param>
        /// <param name="containsHeaders"><c>true</c> if headers are present in the topmose line.</param>
        /// <param name="separator">The char which seperates columns.</param>
        /// <param name="checkValidity">
        /// Indicates if this method should return <c>null</c>, if the file did not pass
        /// <see cref="IsValidCsvFile(string,bool,char)" />.
        /// </param>
        /// <returns>All items inside a key-value-structure as an enumerator.</returns>
        public static IEnumerable<IEnumerable<KeyValuePair<string, string>>> ParseFile(
            string fileUri,
            Encoding encoding,
            bool containsHeaders = false,
            char separator = ',',
            bool checkValidity = false)
        {
            if (!File.Exists(fileUri))
            {
                throw new FileNotFoundException("Provided file not found.", fileUri);
            }
            if (checkValidity && !IsValidCsvFile(fileUri, encoding, containsHeaders, separator))
            {
                // the structure of the file is invalid
                return null;
            }
            string[] lines = null;
            try
            {
                lines = File.ReadAllLines(fileUri, encoding);
            }
            catch
            {
            }
            if (lines == null || !lines.Any())
            {
                return null;
            }
            var firstLine = lines[0].Split(separator);
            if (!firstLine.Any())
            {
                return null;
            }
            var fieldNames = new List<string>();
            Enumerable.Range(1, firstLine.Count()).ToList().ForEach(
                i =>
                {
                    fieldNames.Add(containsHeaders ? GetCleanedValue(firstLine[i - 1]) : i.ToString(CultureInfo.InvariantCulture));
                });
            return ItemIterator(lines, fieldNames.ToArray(), containsHeaders, separator);
        }

        /// <summary>
        /// Converts a double-quoted value as a clean value.
        /// </summary>
        /// <param name="fileValue">The value potentially including double quotes.</param>
        /// <returns>The value without double quotes.</returns>
        public static string GetCleanedValue(string fileValue)
        {
            if (string.IsNullOrEmpty(fileValue))
            {
                return fileValue;
            }
            return fileValue.Replace('"', ' ').Trim();
        }

        /// <summary>
        /// Extension method which will provide easy access to enumerations of <see cref="KeyValuePair{TKey,TValue}" />.
        /// </summary>
        /// <param name="list">The enumeration to extend.</param>
        /// <param name="key">The case-insensitive key to look for.</param>
        /// <returns>The value for the <paramref name="key" /> or <c>null</c> if no key was found.</returns>
        public static string ValueByKey(this IEnumerable<KeyValuePair<string, string>> list, string key)
        {
            var keyValuePairs = list as KeyValuePair<string, string>[] ?? list.ToArray();
            return !keyValuePairs.Any(e => e.Key.Equals(key, StringComparison.OrdinalIgnoreCase))
                ? null
                : keyValuePairs.SingleOrDefault(e => e.Key.Equals(key, StringComparison.OrdinalIgnoreCase)).Value;
        }

        /// <summary>
        /// Internal wrapper for implementing <see cref="ParseFile(string,Encoding,bool,char,bool)" /> as an iterator.
        /// </summary>
        /// <param name="lines">The complete set of lines.</param>
        /// <param name="containsHeaders"><c>true</c> if headers are present in the topmose line.</param>
        /// <param name="fieldNames">The names of the fields in the file order.</param>
        /// <param name="separator">The char which seperates columns.</param>
        /// <returns>An enumerator for all lines.</returns>
        private static IEnumerable<IEnumerable<KeyValuePair<string, string>>> ItemIterator(
            IReadOnlyList<string> lines,
            IReadOnlyList<string> fieldNames,
            bool containsHeaders = false,
            char separator = ',')
        {
            for (var line = containsHeaders ? 1 : 0; line < lines.Count; line++)
            {
                var lineItem = new List<KeyValuePair<string, string>>();
                var fields = lines[line].Split(separator);
                if (fields.Length != fieldNames.Count)
                {
                    continue;
                }
                for (var col = 0; col < fields.Count(); col++)
                {
                    lineItem.Add(new KeyValuePair<string, string>(fieldNames[col], GetCleanedValue(fields[col])));
                }
                yield return lineItem;
            }
        }

        #endregion
    }
}
using System;
using System.Linq;

namespace codingfreaks.cfUtils.Logic.Core.Extensions
{
    using System;
    using System.Data;

    using Utilities;

    /// <summary>
    /// Provides extension methods for <see cref="IDataReader" />
    /// </summary>
    public static class DataReaderExtensions
    {
        #region methods

        /// <summary>
        /// Retrieves a bool for the given <paramref name="columnName" />.
        /// </summary>
        /// <param name="reader">The reader to extend.</param>
        /// <param name="columnName">The name of the column in the reader result.</param>
        /// <returns>The value.</returns>
        public static bool? GetBoolValue(this IDataReader reader, string columnName)
        {
            CheckUtil.ThrowIfNull(() => reader);
            CheckUtil.ThrowIfNullOrEmpty(() => columnName);
            var offset = GetColumnOffset(reader, columnName);
            return reader.IsDBNull(offset) ? default(bool?) : reader.GetBoolean(offset);
        }

        /// <summary>
        /// Retrieves a byte for the given <paramref name="columnName" />.
        /// </summary>
        /// <param name="reader">The reader to extend.</param>
        /// <param name="columnName">The name of the column in the reader result.</param>
        /// <returns>The value.</returns>
        public static byte? GetByteValue(this IDataReader reader, string columnName)
        {
            CheckUtil.ThrowIfNull(() => reader);
            CheckUtil.ThrowIfNullOrEmpty(() => columnName);
            var offset = GetColumnOffset(reader, columnName);
            return reader.IsDBNull(offset) ? default(byte?) : reader.GetByte(offset);
        }

        /// <summary>
        /// Retrieves a char for the given <paramref name="columnName" />.
        /// </summary>
        /// <param name="reader">The reader to extend.</param>
        /// <param name="columnName">The name of the column in the reader result.</param>
        /// <returns>The value.</returns>
        public static char? GetCharValue(this IDataReader reader, string columnName)
        {
            CheckUtil.ThrowIfNull(() => reader);
            CheckUtil.ThrowIfNullOrEmpty(() => columnName);
            var offset = GetColumnOffset(reader, columnName);
            return reader.IsDBNull(offset) ? default(char?) : reader.GetChar(offset);
        }

        /// <summary>
        /// Retrieves the offset of a column with the specified <paramref name="columnName" />.
        /// </summary>
        /// <param name="reader">The reader to extend.</param>
        /// <param name="columnName">The name of the column in the reader result.</param>
        /// <returns>The index of the column.</returns>
        public static int GetColumnOffset(this IDataReader reader, string columnName)
        {
            CheckUtil.ThrowIfNull(() => reader);
            CheckUtil.ThrowIfNullOrEmpty(() => columnName);
            if (reader.IsClosed)
            {
                throw new InvalidOperationException("Reader is closed");
            }
            return reader.GetOrdinal(columnName);
        }

        /// <summary>
        /// Retrieves a DateTime for the given <paramref name="columnName" />.
        /// </summary>
        /// <param name="reader">The reader to extend.</param>
        /// <param name="columnName">The name of the column in the reader result.</param>
        /// <returns>The value.</returns>
        public static DateTime? GetDateTimeValue(this IDataReader reader, string columnName)
        {
            CheckUtil.ThrowIfNull(() => reader);
            CheckUtil.ThrowIfNullOrEmpty(() => columnName);
            var offset = GetColumnOffset(reader, columnName);
            return reader.IsDBNull(offset) ? default(DateTime?) : reader.GetDateTime(offset);
        }

        /// <summary>
        /// Retrieves a decimal for the given <paramref name="columnName" />.
        /// </summary>
        /// <param name="reader">The reader to extend.</param>
        /// <param name="columnName">The name of the column in the reader result.</param>
        /// <returns>The value.</returns>
        public static decimal? GetDecimalValue(this IDataReader reader, string columnName)
        {
            CheckUtil.ThrowIfNull(() => reader);
            CheckUtil.ThrowIfNullOrEmpty(() => columnName);
            var offset = GetColumnOffset(reader, columnName);
            return reader.IsDBNull(offset) ? default(decimal?) : reader.GetChar(offset);
        }

        /// <summary>
        /// Retrieves a double for the given <paramref name="columnName" />.
        /// </summary>
        /// <param name="reader">The reader to extend.</param>
        /// <param name="columnName">The name of the column in the reader result.</param>
        /// <returns>The value.</returns>
        public static double? GetDoubleValue(this IDataReader reader, string columnName)
        {
            CheckUtil.ThrowIfNull(() => reader);
            CheckUtil.ThrowIfNullOrEmpty(() => columnName);
            var offset = GetColumnOffset(reader, columnName);
            return reader.IsDBNull(offset) ? default(double?) : reader.GetChar(offset);
        }

        /// <summary>
        /// Retrieves an Int16 for the given <paramref name="columnName" />.
        /// </summary>
        /// <param name="reader">The reader to extend.</param>
        /// <param name="columnName">The name of the column in the reader result.</param>
        /// <returns>The value.</returns>
        public static short? GetInt16Value(this IDataReader reader, string columnName)
        {
            CheckUtil.ThrowIfNull(() => reader);
            CheckUtil.ThrowIfNullOrEmpty(() => columnName);
            var offset = GetColumnOffset(reader, columnName);
            return reader.IsDBNull(offset) ? default(short?) : reader.GetInt16(offset);
        }

        /// <summary>
        /// Retrieves an Int32 for the given <paramref name="columnName" />.
        /// </summary>
        /// <param name="reader">The reader to extend.</param>
        /// <param name="columnName">The name of the column in the reader result.</param>
        /// <returns>The value.</returns>
        public static int? GetInt32Value(this IDataReader reader, string columnName)
        {
            CheckUtil.ThrowIfNull(() => reader);
            CheckUtil.ThrowIfNullOrEmpty(() => columnName);
            var offset = GetColumnOffset(reader, columnName);
            return reader.IsDBNull(offset) ? default(int?) : reader.GetInt32(offset);
        }

        /// <summary>
        /// Retrieves an Int64 for the given <paramref name="columnName" />.
        /// </summary>
        /// <param name="reader">The reader to extend.</param>
        /// <param name="columnName">The name of the column in the reader result.</param>
        /// <returns>The value.</returns>
        public static long? GetInt64Value(this IDataReader reader, string columnName)
        {
            CheckUtil.ThrowIfNull(() => reader);
            CheckUtil.ThrowIfNullOrEmpty(() => columnName);
            var offset = GetColumnOffset(reader, columnName);
            return reader.IsDBNull(offset) ? default(long?) : reader.GetInt64(offset);
        }

        /// <summary>
        /// Retrieves a string for the given <paramref name="columnName" />.
        /// </summary>
        /// <param name="reader">The reader to extend.</param>
        /// <param name="columnName">The name of the column in the reader result.</param>
        /// <returns>The value.</returns>
        public static string GetStringValue(this IDataReader reader, string columnName)
        {
            CheckUtil.ThrowIfNull(() => reader);
            CheckUtil.ThrowIfNullOrEmpty(() => columnName);
            var offset = GetColumnOffset(reader, columnName);
            return reader.IsDBNull(offset) ? default(string) : reader.GetString(offset);
        }

        /// <summary>
        /// Retrieves a value from a reader.
        /// </summary>
        /// <typeparam name="TResult">The expected result.</typeparam>
        /// <param name="reader">The reader to extend.</param>
        /// <param name="columnName">The name of the column in the reader result.</param>
        /// <returns>The value of the column.</returns>
        public static TResult GetValue<TResult>(this IDataReader reader, string columnName)
        {
            CheckUtil.ThrowIfNull(() => reader);
            CheckUtil.ThrowIfNullOrEmpty(() => columnName);
            var offset = GetColumnOffset(reader, columnName);
            var value = reader.GetValue(offset);
            return (TResult)value;
        }

        #endregion
    }
}
namespace codingfreaks.cfUtils.Logic.Excel.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Utilities;

    /// <summary>
    /// Provides useful extension methods for convenient access of Excel utilities.
    /// </summary>
    public static class ExcelExtensions
    {
        #region methods

        /// <summary>
        /// Create a excel file from a set elements of any type.
        /// </summary>
        /// <typeparam name="T">The type of the elements in <paramref name="data" />.</typeparam>
        /// <param name="data">A list of elements to put into the excel result.</param>
        /// <param name="worksheetName">An optional name for the worksheet in workbook.</param>
        /// <returns>A byte array representing the new workbook.</returns>
        public static byte[] ToExcelBytes<T>(this IEnumerable<T> data, string worksheetName = "Worksheet")
        {
            return ExcelUtil.GetByteArray(data, worksheetName);
        }

        /// <summary>
        /// Converts any given numeric <paramref name="offset" /> into an Excel column name.
        /// </summary>
        /// <param name="offset">The numeric offset beginning with 1 for column A.</param>
        /// <returns>The Excel column name.</returns>
        public static string ToExcelColumnIndex(this int offset)
        {
            if (offset <= 0 || offset > 702)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), offset, "Offset must be grater between 1 and 702.");
            }
            var result = string.Empty;
            var alpha = offset / 27;
            var remain = offset - alpha * 26;
            if (alpha > 0)
            {
                result = ((char)(alpha + 64)).ToString();
            }
            if (remain > 0)
            {
                result += ((char)(remain + 64)).ToString();
            }
            return result;
        }

        /// <summary>
        /// Converts any given numeric <paramref name="offset" /> into an Excel column name.
        /// </summary>
        /// <param name="offset">The numeric offset beginning with 1 for column A.</param>
        /// <param name="row">The row offset to add to the address.</param>
        /// <returns>The Excel column name.</returns>
        public static string ToExcelColumnIndexWithRow(this int offset, int row)
        {
            var result = ToExcelColumnIndex(offset);
            return $"{result}{row}";
        }

        #endregion
    }
}
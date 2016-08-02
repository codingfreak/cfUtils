namespace codingfreaks.cfUtils.Logic.Excel.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    using Extensions;

    using System.Linq;

    using OfficeOpenXml;

    /// <summary>
    /// Provides logic for Excel.
    /// </summary>
    public static class ExcelUtil
    {
        #region methods

        /// <summary>
        /// Create a excel file from a set elements of any type.
        /// </summary>
        /// <typeparam name="T">The type of the elements in <paramref name="data" />.</typeparam>
        /// <param name="data">A list of elements to put into the excel result.</param>
        /// <param name="worksheetName">An optional name for the worksheet in workbook.</param>
        /// <param name="formatHeaderBold">Indicates whether the first line should be formatted bold.</param>
        /// <returns>A byte array representing the new workbook.</returns>
        public static byte[] GetByteArray<T>(IEnumerable<T> data, string worksheetName = "Worksheet", bool formatHeaderBold = true)
        {
            try
            {
                //using (var package = new ExcelPackage(new FileInfo(filepath)))
                using (var package = new ExcelPackage())
                {
                    //Create the worksheet
                    var worksheet = package.Workbook.Worksheets.Add(worksheetName);
                    //get our column headings
                    var t = typeof(T);
                    var properties = t.GetProperties().ToList();
                    if (!properties.Any())
                    {
                        return null;
                    }
                    for (var i = 0; i < properties.Count(); i++)
                    {
                        worksheet.Cells[1, i + 1].Value = properties[i].Name;
                    }
                    // populate our data
                    var enumerableData = data as T[] ?? data.ToArray();
                    if (enumerableData.Any())
                    {
                        worksheet.Cells[1.ToExcelColumnIndexWithRow(2)].LoadFromCollection(enumerableData);
                    }
                    if (formatHeaderBold)
                    {
                        // make header in bold
                        var from = 1.ToExcelColumnIndexWithRow(1);
                        var to = properties.Count.ToExcelColumnIndexWithRow(1);
                        using (var headerRange = worksheet.Cells[$"{from}:{to}"])
                        {
                            headerRange.Style.Font.Bold = true;
                        }
                    }
                    return package.GetAsByteArray();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
                return null;
            }
        }

        #endregion
    }
}
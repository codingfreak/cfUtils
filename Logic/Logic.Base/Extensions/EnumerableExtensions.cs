using System;
using System.Linq;

namespace codingfreaks.cfUtils.Logic.Base.Extensions
{
    using System.Collections.Generic;
    using System.Data;
    using System.Reflection;

    /// <summary>
    /// Provides extensions for <see cref="IEnumerable{T}" />.
    /// </summary>
    public static class EnumerableExtensions
    {
        #region methods

        /// <summary>
        /// Creates a DataTable from some IEnumerable of type T. Columns of the table will be the properties of T. The order of
        /// columns is the order or properies in T.
        /// </summary>
        /// <typeparam name="T">Type of the objects in the IEnumerable.</typeparam>
        /// <param name="list">IEnumerable to convert to a DataTable.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(this IEnumerable<T> list, string tableName)
        {
            var type = typeof(T);
            var propertyInfos = type.GetProperties();
            var table = new DataTable(tableName);
            foreach (var info in propertyInfos)
            {
                table.Columns.Add(info.Name, Nullable.GetUnderlyingType(info.PropertyType) != null ? typeof(object) : info.PropertyType);
            }
            foreach (var element in list)
            {
                var newRow = new object[propertyInfos.Length];
                for (var i = 0; i <= newRow.Length - 1; i++)
                {
                    newRow[i] = type.InvokeMember(propertyInfos[i].Name, BindingFlags.GetProperty, null, element, new object[0]);
                }
                table.LoadDataRow(newRow, true);
            }
            return table;
        }

        #endregion
    }
}
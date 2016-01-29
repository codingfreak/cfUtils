using System;
using System.Linq;

namespace codingfreaks.cfUtils.Logic.Portable.Extensions
{
    using System.Collections.Generic;

    /// <summary>
    /// Adds extension methods to <see cref="List{T}"/> LINQ-liqe.
    /// </summary>
    public static class ListExtensions
    {
        #region methods

        /// <summary>
        /// Sets the <paramref name="list"/> to <c>null</c> including clearing and trimming it
        /// and calls the collection of GC in one step.
        /// </summary>
        /// <typeparam name="T">The type of the items in the list.</typeparam>
        /// <param name="list">The list to free.</param>
        public static void FreeFromMemory<T>(this List<T> list)
        {
            list.Clear();
            list.TrimExcess();
            // ReSharper disable once RedundantAssignment
            list = null;
            GC.Collect();
        }

        #endregion
    }
}
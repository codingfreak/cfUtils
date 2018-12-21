namespace codingfreaks.cfUtils.Logic.Core.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Utilities;

    /// <summary>
    /// Adds extension methods to <see cref="IEnumerable{T}" /> LINQ-like.
    /// </summary>
    public static class EnumerableExtensions
    {
        #region methods

        /// <summary>
        /// Iterates through a list of T and retrieves the index of the <paramref name="element" /> inside the enumerable.
        /// </summary>
        /// <typeparam name="T">The type of an element inside the <paramref name="enumerable" />.</typeparam>
        /// <param name="enumerable">The enumerable to extend.</param>
        /// <param name="element">The element to find.</param>
        /// <returns>The offset of the element or -1 if the element wasn't found in the enumerable.</returns>
        public static int GetIndexOf<T>(this IEnumerable<T> enumerable, T element)
            where T : IComparable
        {
            CheckUtil.ThrowIfNull(() => enumerable);
            return enumerable.GetIndexOf(t => t.Equals(element));
        }

        /// <summary>
        /// Iterates through a list of T and retrieves the index of the element matching the <paramref name="compareFunc" /> inside
        /// the enumerable.
        /// </summary>
        /// <typeparam name="T">The type of an element inside the <paramref name="enumerable" />.</typeparam>
        /// <param name="enumerable">The enumerable to extend.</param>
        /// <param name="compareFunc">The element to find.</param>
        /// <returns>The offset of the element or -1 if the element wasn't found in the enumerable.</returns>
        public static int GetIndexOf<T>(this IEnumerable<T> enumerable, Func<T, bool> compareFunc)
        {
            var enumerableToUse = enumerable as T[] ?? enumerable.ToArray();
            CheckUtil.ThrowIfNull(() => enumerableToUse);
            for (var i = 0; i < enumerableToUse.Count(); i++)
            {
                if (compareFunc(enumerableToUse[i]))
                {
                    return i;
                }
            }
            return -1;
        }

        #endregion
    }
}
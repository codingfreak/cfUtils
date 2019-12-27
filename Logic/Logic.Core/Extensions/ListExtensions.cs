namespace codingfreaks.cfUtils.Logic.Core.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Adds extension methods to <see cref="List{T}" /> and <see cref="IList{T}" /> LINQ-like.
    /// </summary>
    public static class ListExtensions
    {
        #region methods

        /// <summary>
        /// Sets the <paramref name="list" /> to <c>null</c> including clearing and trimming it
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

        /// <summary>
        /// Replaces all matches of <paramref name="matchPredicate" /> in a <paramref name="list" /> with the
        /// <paramref name="newItem" />.
        /// </summary>
        /// <remarks>
        /// This method is not thread safe. Apply your own thread-safe logic around this call!
        /// </remarks>
        /// <typeparam name="T">The type of the items in the list.</typeparam>
        /// <param name="list">The list where to perform the replacement.</param>
        /// <param name="newItem">The item which to insert instead of the old item(s).</param>
        /// <param name="matchPredicate">A predicate which will retrieve the old items to replace.</param>
        public static void Replace<T>(this IList<T> list, T newItem, Func<T, bool> matchPredicate)
        {
            if (!list.Any())
            {
                // nothing to do
                return;
            }
            var item = list.FirstOrDefault(matchPredicate);
            var lastIndex = -1;
            while (item != null)
            {
                var index = list.IndexOf(item);
                if (index <= 0 || index == lastIndex)
                {
                    // either the item wasn't found or it is the same as the
                    // last item
                    break;
                }
                // retrieve index and replace item
                lastIndex = index;
                list[index] = newItem;
                // prepare for next iteration
                item = list.FirstOrDefault(matchPredicate);
            }
        }

        #endregion
    }
}
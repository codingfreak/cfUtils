namespace codingfreaks.cfUtils.Logic.Screenshot
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A bunch of <see cref="Screenshot"/>s.
    /// </summary>
    public class ScreenshotCollection : List<Screenshot>
    {
        #region constants

        #region static fields

        [ThreadStatic]
        private static IntPtr _checkHWnd;

        #endregion

        #endregion

        #region constructors and destructors

        /// <summary>
        /// Initializes the collection including the screenshots.
        /// </summary>
        /// <param name="items">The screenshots collection.</param>
        /// <param name="asReadOnly"><c>true</c> if the collection should be read-only.</param>
        public ScreenshotCollection(IEnumerable<Screenshot> items, bool asReadOnly)
        {
            base.AddRange(items);
            TrimExcess();
            ReadOnly = asReadOnly;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ScreenshotCollection()
        {
            ReadOnly = false;
        }

        #endregion

        #region methods

        /// <summary>
        /// Adds a screenshot to the collection.
        /// </summary>
        /// <param name="item">The screenshot.</param>
        public new void Add(Screenshot item)
        {
            if (ReadOnly)
            {
                ThrowReadonlyException();
            }
            base.Add(item);
        }

        /// <summary>
        /// Adds multiple screenshots to the collection.
        /// </summary>
        /// <param name="collection">The screenshots.</param>
        public new void AddRange(IEnumerable<Screenshot> collection)
        {
            if (ReadOnly)
            {
                ThrowReadonlyException();
            }
            base.AddRange(collection);
        }

        /// <summary>
        /// Clears all screenshots from the list.
        /// </summary>
        public new void Clear()
        {
            if (ReadOnly)
            {
                ThrowReadonlyException();
            }
            base.Clear();
        }

        /// <summary>
        /// Searches for a screenshot with the given <paramref name="hWnd"/>.
        /// </summary>
        /// <param name="hWnd">The window handle to search for.</param>
        /// <returns><c>true</c> if the screenshot was found, othewise <c>false</c>.</returns>
        public bool Contains(IntPtr hWnd)
        {
            return GetScreenshot(hWnd) != null;
        }

        /// <summary>
        /// Retrieves all minimized screeenshots.
        /// </summary>
        /// <returns>The screenhots of all minimized windows.</returns>
        public ScreenshotCollection GetAllMinimized()
        {
            var wsCol = (ScreenshotCollection)FindAll(IsMinimizedPredict);
            return wsCol;
        }

        /// <summary>
        /// Retrieves a single screenshot.
        /// </summary>
        /// <param name="hWnd">The window handle to search for.</param>
        /// <returns>The screenshot or <c>null</c>.</returns>
        public Screenshot GetScreenshot(IntPtr hWnd)
        {
            _checkHWnd = hWnd;
            return Find(IshWndPredict);
        }

        /// <summary>
        /// Inserts a screenshot at a given position.
        /// </summary>
        /// <param name="index">The position of the new screenshot.</param>
        /// <param name="item">The screenshot.</param>
        public new void Insert(int index, Screenshot item)
        {
            if (ReadOnly)
            {
                ThrowReadonlyException();
            }
            base.Insert(index, item);
        }

        /// <summary>
        /// Inserts a collection of screenshots at a given position.
        /// </summary>
        /// <param name="index">The position of the screenshots.</param>
        /// <param name="collection">The screenshots.</param>
        public new void InsertRange(int index, IEnumerable<Screenshot> collection)
        {
            if (ReadOnly)
            {
                ThrowReadonlyException();
            }
            base.InsertRange(index, collection);
        }

        /// <summary>
        /// Removes a single screenshot.
        /// </summary>
        /// <param name="item">The screenshot to remove.</param>
        public new void Remove(Screenshot item)
        {
            if (ReadOnly)
            {
                ThrowReadonlyException();
            }
            base.Remove(item);
        }

        /// <summary>
        /// Removes all screenshots matching a given predicate.
        /// </summary>
        /// <param name="match">The predicate that defines a matching.</param>
        public new void RemoveAll(Predicate<Screenshot> match)
        {
            if (ReadOnly)
            {
                ThrowReadonlyException();
            }
            base.RemoveAll(match);
        }

        /// <summary>
        /// Removes a screenhot at a given position.
        /// </summary>
        /// <param name="index">The position of the screenshot to remove.</param>
        public new void RemoveAt(int index)
        {
            if (ReadOnly)
            {
                ThrowReadonlyException();
            }
            base.RemoveAt(index);
        }

        /// <summary>
        /// Removes an amount of screenshots from a given position on.
        /// </summary>
        /// <param name="index">The start offset for removing.</param>
        /// <param name="count">The amlunt of items to remove.</param>
        public new void RemoveRange(int index, int count)
        {
            if (ReadOnly)
            {
                ThrowReadonlyException();
            }
            base.RemoveRange(index, count);
        }

        /// <summary>
        /// Reverses the order of screenshots in a given window.
        /// </summary>
        /// <param name="index">The start offset for reversing.</param>
        /// <param name="count">The amlunt of items to reversing.</param>
        public new void Reverse(int index, int count)
        {
            if (ReadOnly)
            {
                ThrowReadonlyException();
            }
            base.Reverse(index, count);
        }

        /// <summary>
        /// Reverses the order of screenshots.
        /// </summary>
        public new void Reverse()
        {
            if (ReadOnly)
            {
                ThrowReadonlyException();
            }
            base.Reverse();
        }

        /// <summary>
        /// Sorts the internal collection.
        /// </summary>
        public new void Sort()
        {
            if (ReadOnly)
            {
                ThrowReadonlyException();
            }
            base.Sort();
        }

        /// <summary>
        /// Sorts the internal collection.
        /// </summary>
        /// <param name="comparison">The comparison to sort with.</param>
        public new void Sort(Comparison<Screenshot> comparison)
        {
            if (ReadOnly)
            {
                ThrowReadonlyException();
            }
            base.Sort(comparison);
        }

        /// <summary>
        /// Sorts the internal collection.
        /// </summary>        
        /// <param name="comparer">The comparer to sort with.</param>        
        public new void Sort(IComparer<Screenshot> comparer)
        {
            if (ReadOnly)
            {
                ThrowReadonlyException();
            }
            base.Sort(comparer);
        }

        /// <summary>
        /// Sorts the internal collection in a given window.
        /// </summary>
        /// <param name="index">The start offset for reversing.</param>
        /// <param name="count">The amlunt of items to reversing.</param>
        /// <param name="comparer">The comparer to sort with.</param>       
        public new void Sort(int index, int count, IComparer<Screenshot> comparer)
        {
            if (ReadOnly)
            {
                ThrowReadonlyException();
            }
            base.Sort(index, count, comparer);
        }

        /// <summary>
        /// Updates a specfic screenshot.
        /// </summary>
        /// <param name="item">The item to update.</param>
        public void Update(Screenshot item)
        {
            lock (this)
            {
                var oldSnap = GetScreenshot(item.Handle);
                if (oldSnap == null)
                {
                    return;
                }
                Remove(oldSnap);
                Add(item);
            }
        }

        /// <summary>
        /// A method that checks, if a single <paramref name="screenshot"/> handle matches a local variable.
        /// </summary>
        /// <param name="screenshot">The screenshot to work on.</param>
        /// <returns><c>true</c> if the screeenshot matches the local handle.</returns>
        private static bool IshWndPredict(Screenshot screenshot)
        {
            return screenshot.Handle == _checkHWnd;
        }

        /// <summary>
        /// Checks if the given <paramref name="screenshot"/> is minimized.
        /// </summary>
        /// <param name="screenshot">The screenshot to work on.</param>
        /// <returns><c>true</c> if the screenshot is minimized.</returns>
        private static bool IsMinimizedPredict(Screenshot screenshot)
        {
            return screenshot.IsMinimized;
        }

        /// <summary>
        /// Throws the exception stating that the collection is set to read-only.
        /// </summary>
        private static void ThrowReadonlyException()
        {
            throw new InvalidOperationException("The collection is marked as Read-Only and it cannot be modified");
        }

        #endregion

        #region properties

        /// <summary>
        /// Indicates whether this instance is set to read-only.
        /// </summary>
        public bool ReadOnly { get; }

        #endregion
    }
}
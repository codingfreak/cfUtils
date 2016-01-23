using System;
using System.Linq;

namespace s2.s2Utils.Logic.Wpf.Components
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;

    /// <summary>
    /// Optimized implementation of <see cref="ObservableCollection{T}"/> which reduces the amount of events propageted
    /// to the UI and adds the <see cref="AddRange"/> functionallity.
    /// </summary>
    /// <typeparam name="T">The type of items in this collection.</typeparam>
    public class OptimizedObservableCollection<T> : ObservableCollection<T>
    {
        #region member vars

        private bool _preventEvent;

        #endregion

        #region methods

        /// <summary>
        /// Adds a bunch of <paramref name="items"/> in one step.
        /// </summary>
        /// <param name="items">The items to add.</param>
        public void AddRange(IEnumerable<T> items)
        {
            PreventCollectionChanged();
            try
            {
                foreach (var item in items)
                {
                    InsertItem(Count, item);
                }
            }
            finally
            {
                ResumeCollectionChanged();
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        /// <summary>
        /// Prevents the collection from propagating collection-change-events.
        /// </summary>
        public void PreventCollectionChanged()
        {
            _preventEvent = true;
        }

        /// <summary>
        /// Resumes propagation of collection-change-events.
        /// </summary>
        public void ResumeCollectionChanged()
        {
            _preventEvent = false;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Collections.ObjectModel.ObservableCollection`1.CollectionChanged"/> event with the provided arguments.
        /// </summary>
        /// <param name="e">Arguments of the event being raised.</param>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (!_preventEvent)
            {
                base.OnCollectionChanged(e);
            }
        }

        #endregion
    }
}
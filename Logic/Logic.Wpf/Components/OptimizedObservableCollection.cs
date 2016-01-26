using System;
using System.Linq;

namespace codingfreaks.cfUtils.Logic.Wpf.Components
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Windows.Threading;

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

        #region events

        /// <summary>
        /// Occurs when an item is added, removed, changed, moved, or the entire list is refreshed.
        /// </summary>
        public override event NotifyCollectionChangedEventHandler CollectionChanged;

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
            if (_preventEvent)
            {
                return;
            }
            var eh = CollectionChanged;
            if (eh == null)
            {
                return;
            }
            var dispatcher = (from NotifyCollectionChangedEventHandler nh in eh.GetInvocationList() let dpo = nh.Target as DispatcherObject where dpo != null select dpo.Dispatcher).FirstOrDefault();
            if (dispatcher != null && dispatcher.CheckAccess() == false)
            {
                dispatcher.Invoke(DispatcherPriority.DataBind, (Action)(() => OnCollectionChanged(e)));
            }
            else
            {
                foreach (var @delegate in eh.GetInvocationList())
                {
                    var nh = (NotifyCollectionChangedEventHandler)@delegate;
                    nh.Invoke(this, e);
                }
            }
        }

        #endregion
    }
}
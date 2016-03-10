using System;
using System.Linq;

namespace codingfreaks.cfUtils.Logic.Wpf.Components
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Windows.Threading;

    /// <summary>
    /// Optimized implementation of <see cref="BindingList{T}"/> which reduces the amount of events propageted
    /// to the UI and adds the <see cref="AddRange"/> functionallity.
    /// </summary>
    /// <typeparam name="T">The type of items in this collection.</typeparam>
    public class OptimizedBindingList<T> : BindingList<T>
    {
        #region member vars

        private bool _preventEvent;

        #endregion

        #region events

        /// <summary>
        /// Occurs when an item is added, removed, changed, moved, or the entire list is refreshed.
        /// </summary>
        public new event ListChangedEventHandler ListChanged;

        #endregion

        #region constructors and destructors

        /// <summary>
        /// Initializes a new empty instance of <see cref="T:System.ComponentModel.BindingList`1"/>-type.
        /// </summary>
        public OptimizedBindingList()
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="T:System.ComponentModel.BindingList`1"/>-type using the given <paramref name="list"/>.
        /// </summary>
        /// <param name="list">A <see cref="T:System.Collections.Generic.IList`1"/> of elements which should be contained in <see cref="T:System.ComponentModel.BindingList`1"/>.</param>
        public OptimizedBindingList(IList<T> list) : base(list)
        {
        }

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
                OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, 0, 0));
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
        /// Raises the <see cref="E:System.ComponentModel.BindingList`1.ListChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.ComponentModel.ListChangedEventArgs"/> that contains the event data. </param>
        protected override void OnListChanged(ListChangedEventArgs e)
        {
            if (_preventEvent)
            {
                return;
            }
            var eventHandler = ListChanged;
            if (eventHandler == null)
            {
                return;
            }
            var dispatcher =
                (from NotifyCollectionChangedEventHandler nh in eventHandler.GetInvocationList() let dpo = nh.Target as DispatcherObject where dpo != null select dpo.Dispatcher).FirstOrDefault();
            if (dispatcher != null && dispatcher.CheckAccess() == false)
            {
                dispatcher.Invoke(DispatcherPriority.DataBind, (Action)(() => OnListChanged(e)));
            }
            else
            {
                foreach (var @delegate in eventHandler.GetInvocationList())
                {
                    var nh = (ListChangedEventHandler)@delegate;
                    nh.Invoke(this, e);
                }
            }
        }

        #endregion
    }
}
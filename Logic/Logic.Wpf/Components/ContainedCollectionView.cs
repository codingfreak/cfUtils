namespace codingfreaks.cfUtils.Logic.Wpf.Components
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Threading;

    using Annotations;

    /// <inheritdoc />
    /// <summary>
    /// Complete wrapper for <see cref="ListCollectionView" /> making it easy to connect internal data with bindable view
    /// representation.
    /// </summary>
    /// <typeparam name="TItem">
    /// The type of each element in
    /// <see cref="P:codingfreaks.cfUtils.Logic.Wpf.Components.ContainedCollectionView`1.Items" />.
    /// </typeparam>
    public class ContainedCollectionView<TItem> : INotifyPropertyChanged
        where TItem : INotifyPropertyChanged
    {
        #region events

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region constructors and destructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ContainedCollectionView()
        {
        }

        /// <summary>
        /// Constructor to use when data should be passed directly.
        /// </summary>
        /// <param name="items">The list of items.</param>
        public ContainedCollectionView(IEnumerable<TItem> items)
        {
            InitItems(items ?? Enumerable.Empty<TItem>());
        }

        #endregion

        #region methods

        /// <summary>
        /// Adds a single <paramref name="item" /> to the internal data collection.
        /// </summary>
        /// <param name="item"></param>
        public void Add(TItem item)
        {
            if (Items == null)
            {
                InitItems(Enumerable.Empty<TItem>());
            }
            item.PropertyChanged += OnItemPropertyChanged;
            Items.Add(item);
        }

        /// <summary>
        /// Adds a bunch of <paramref name="items" /> to the internal data collection.
        /// </summary>
        /// <remarks>
        /// Calls <see cref="Add" /> for each item internally.
        /// </remarks>
        /// <param name="items">The items to add.</param>
        public void AddRange(IEnumerable<TItem> items)
        {
            foreach (var item in items)
            {
                Add(item);
            }
        }

        /// <summary>
        /// Clears the internal list of items.
        /// </summary>
        public void Clear()
        {
            Items?.Clear();
        }

        /// <summary>
        /// Retrieves the index of the first occurrence <paramref name="item" /> in the internal data collection or -1.
        /// </summary>
        /// <param name="item">The item to look for.</param>
        /// <returns>The index or -1 if items is <c>null</c>.</returns>
        public int IndexOf([NotNull] TItem item)
        {
            return Items?.IndexOf(item) ?? -1;
        }

        /// <summary>
        /// Resets the <see cref="Items" /> and <see cref="ItemsView" /> in one step.
        /// </summary>
        /// <param name="items">The items to take as the current data source.</param>
        /// <param name="performResetBefore">
        /// If set tot <c>true</c> <see cref="Reset" /> will be called before any init (defaults
        /// to <c>true</c>).
        /// </param>
        public void InitItems(IEnumerable<TItem> items, bool performResetBefore = true)
        {
            if (performResetBefore)
            {
                Reset();
            }
            Items = new ObservableCollection<TItem>(items);
            // connect events for any added item
            foreach (var item in Items)
            {
                item.PropertyChanged += OnItemPropertyChanged;
            }
            // ensure that future items are connected and items that are removed are disconnected from events
            Items.CollectionChanged += (s, e) =>
            {
                if (e.NewItems != null)
                {
                    foreach (INotifyPropertyChanged added in e.NewItems)
                    {
                        added.PropertyChanged += OnItemPropertyChanged;
                    }
                }
                if (e.OldItems != null)
                {
                    foreach (INotifyPropertyChanged removed in e.OldItems)
                    {
                        removed.PropertyChanged -= OnItemPropertyChanged;
                    }
                }
            };
            // create the bindable view representation of the data
            ItemsView = CollectionViewSource.GetDefaultView(Items) as ListCollectionView;
            if (ItemsView == null)
            {
                // very strange because it indicates that GetDefaultView could not be casted to ListCollectionView
                return;
            }
            // ensure that the CurrentItem property will notify about the fact that the view changed it's current item
            ItemsView.CurrentChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(CurrentItem));
            };
            BindingOperations.EnableCollectionSynchronization(Items, ListLock);
        }

        /// <summary>
        /// Removes a given <paramref name="item" /> from the internal data collection.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        public void Remove([NotNull] TItem item)
        {
            Items?.Remove(item);
        }

        /// <summary>
        /// Replaces all matches of <paramref name="matchPredicate" /> in the internal data collection with the
        /// <paramref name="newItem" />.
        /// </summary>
        /// <param name="newItem">The item which to insert instead of the old item(s).</param>
        /// <param name="matchPredicate">A predicate which will retrieve the old items to replace.</param>
        public void Replace(TItem newItem, Func<TItem, bool> matchPredicate)
        {
            if (!Items.Any())
            {
                // nothing to do
                return;
            }
            var item = Items.FirstOrDefault(matchPredicate);
            var lastIndex = -1;
            while (item != null)
            {
                var index = Items.IndexOf(item);
                if (index <= 0 || index == lastIndex)
                {
                    // either the item wasn't found or it is the same as the
                    // last item
                    break;
                }
                // retrieve index and replace item
                lastIndex = index;
                Items[index] = newItem;
                item = Items.FirstOrDefault(matchPredicate);
            }
        }

        /// <summary>
        /// Performs a complete nulling of items.
        /// </summary>
        public void Reset()
        {
            Clear();
            Items = null;
            ItemsView = null;
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged" /> event if any listener is active.
        /// </summary>
        /// <param name="propertyName">The name of the changed property (passed in automatically).</param>
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Reacts on any property change of any item in <see cref="Items" />.
        /// </summary>
        /// <param name="sender">The item that has a changed property.</param>
        /// <param name="e">The event args.</param>
        private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ItemsView.Refresh();
        }

        #endregion

        #region properties

        /// <summary>
        /// The amount of data items currently present.
        /// </summary>
        public int Count => Items?.Count ?? 0;

        /// <summary>
        /// Gets/sets the current selected item.
        /// </summary>
        public TItem CurrentItem
        {
            get
            {
                var result = default(TItem);
                Application.Current.Dispatcher.Invoke(
                    () =>
                    {
                        result = (TItem)ItemsView.CurrentItem;
                    });
                return result;
            }
            set
            {
                Application.Current.Dispatcher.Invoke(
                    () =>
                    {
                        ItemsView.MoveCurrentTo(value);
                        OnPropertyChanged();
                    });                
            }
        }

        /// <summary>
        /// Gets/sets the item on a given <paramref name="index" />.
        /// </summary>
        /// <param name="index">The 0-based index of the item.</param>
        /// <exception cref="InvalidOperationException">Is thrown when the internal data collection is <c>null</c>.</exception>
        public TItem this[int index]
        {
            get
            {
                if (Items == null)
                {
                    throw new InvalidOperationException("Items not initialized yet.");
                }
                return Items[index];
            }
            set
            {
                if (Items == null)
                {
                    throw new InvalidOperationException("Items not initialized yet.");
                }
                Items[index] = value;
            }
        }

        /// <summary>
        /// Retrieves an enumerable version of the internal data collection.
        /// </summary>
        public IEnumerable<TItem> ItemsEnum => Items?.AsEnumerable();

        /// <summary>
        /// Retrieves a queryable version of the internal data collection.
        /// </summary>
        public IQueryable<TItem> ItemsQuery => Items?.AsQueryable();

        /// <summary>
        /// The bindable view of the internal <see cref="Items" />.
        /// </summary>
        public ListCollectionView ItemsView { get; set; }

        /// <summary>
        /// The internal list of data.
        /// </summary>
        private ObservableCollection<TItem> Items { get; set; }

        /// <summary>
        /// Is used by the cynchronization logic for multithreading.
        /// </summary>
        private object ListLock { get; } = new object();

        #endregion
    }
}
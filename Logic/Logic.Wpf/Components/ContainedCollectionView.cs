namespace codingfreaks.cfUtils.Logic.Wpf.Components
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Windows.Data;

    using Annotations;

    /// <inheritdoc />
    /// <summary>
    /// Complete wrapper for <see cref="ListCollectionView"/> making it easy to connect internal data with bindable view representation.
    /// </summary>
    /// <typeparam name="TItem">The type of each element in <see cref="P:codingfreaks.cfUtils.Logic.Wpf.Components.ContainedCollectionView`1.Items" />.</typeparam>
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
            InitItems(items);
        }

        #endregion

        #region methods

        /// <summary>
        /// Resets the <see cref="Items" /> and <see cref="ItemsView" /> in one step.
        /// </summary>
        /// <param name="items">The items to take as the current data source.</param>
        public void InitItems(IEnumerable<TItem> items)
        {
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
        /// Gets/sets the current selected item.
        /// </summary>
        public TItem CurrentItem
        {
            get => (TItem)ItemsView.CurrentItem;
            set
            {
                ItemsView.MoveCurrentTo(value);
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The bindable view of the internal <see cref="Items" />.
        /// </summary>
        public ListCollectionView ItemsView { get; set; }

        /// <summary>
        /// The internal list of data.
        /// </summary>
        private ObservableCollection<TItem> Items { get; set; }

        #endregion
    }
}
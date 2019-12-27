namespace codingfreaks.cfUtils.Logic.Csv
{
    using System;
    using System.Linq;

    /// <summary>
    /// Is used as an argument for events which are related to an item of type <typeparamref name="TItem" />.
    /// </summary>
    /// <typeparam name="TItem">The type of the related item.</typeparam>
    public class ItemEventArgs<TItem> : EventArgs
        where TItem : new()
    {
        #region constructors and destructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="item">The item associated to the event.</param>
        public ItemEventArgs(TItem item)
        {
            Item = item;
        }

        #endregion

        #region properties

        /// <summary>
        /// The item associated to the event.
        /// </summary>
        public TItem Item { get; }

        #endregion
    }
}
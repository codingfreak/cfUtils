namespace codingfreaks.cfUtils.Logic.Azure
{
    using Microsoft.WindowsAzure.Storage.Table;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Is used by events which want to inform about a bunch of <typeparamref name="TTableItem"/>.
    /// </summary>
    /// <typeparam name="TTableItem">The type of the items in the table.</typeparam>
    public class TableEntityListEventArgs<TTableItem> : EventArgs where TTableItem : TableEntity, new()
    {
        #region constructors and destructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="entries">The entries.</param>
        public TableEntityListEventArgs(IEnumerable<TTableItem> entries)
        {
            Entries = entries;
        }

        #endregion

        #region properties

        /// <summary>
        /// The entries.
        /// </summary>
        public IEnumerable<TTableItem> Entries { get; private set; }

        #endregion
    }
}
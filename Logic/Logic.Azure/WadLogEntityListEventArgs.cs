namespace codingfreaks.cfUtils.Logic.Azure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Is used by events which want to inform about a bunch of <see cref="WadLogEntity"/>.
    /// </summary>
    public class WadLogEntityListEventArgs : EventArgs
    {
        #region constructors and destructors

        public WadLogEntityListEventArgs(IEnumerable<WadLogEntity> entries)
        {
            Entries = entries;
        }

        #endregion

        #region properties

        /// <summary>
        /// The entries.
        /// </summary>
        public IEnumerable<WadLogEntity> Entries { get; private set; }

        #endregion
    }
}
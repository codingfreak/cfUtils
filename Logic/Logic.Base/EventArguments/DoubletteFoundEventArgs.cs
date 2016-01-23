namespace codingfreaks.cfUtils.Logic.Base.EventArguments
{
    using System;
    using System.Collections.Generic;

    using codingfreaks.cfUtils.Logic.Base.Delegates;
    using codingfreaks.cfUtils.Logic.Base.Interfaces;

    /// <summary>
    /// Is used as an argument for <see cref="DoubletteFoundEventHandler{T}"/> to enable transport
    /// of doublettes to the caller and getting cancellation response back.
    /// </summary>
    /// <typeparam name="T">The type of entity where checking is performed.</typeparam>
    public class DoubletteFoundEventArgs<T> : EventArgs
        where T : class, IEntity
    {
        #region constructors and destructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="doublettes">The doublette elements found.</param>
        public DoubletteFoundEventArgs(IEnumerable<T> doublettes)
        {
            Doublettes = doublettes;
        }

        #endregion

        #region properties

        /// <summary>
        /// Caller can set this to <c>true</c> to stop further actions.
        /// </summary>
        public bool Cancel { get; set; }

        /// <summary>
        /// The doublette elements found.
        /// </summary>
        public IEnumerable<T> Doublettes { get; private set; }

        #endregion
    }
}
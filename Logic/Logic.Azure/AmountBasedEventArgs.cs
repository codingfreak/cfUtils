using System;
using System.Linq;

namespace s2.s2Utils.Logic.Azure
{
    /// <summary>
    /// Can be used for events which inform on the amont of items affected by an operation.
    /// </summary>
    public class AmountBasedEventArgs : EventArgs
    {
        #region constructors and destructors

        public AmountBasedEventArgs(long amount)
        {
            Amount = amount;
        }

        #endregion

        #region properties

        /// <summary>
        /// The amount of items affected.
        /// </summary>
        public long Amount { get; private set; }

        #endregion
    }
}
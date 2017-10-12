namespace codingfreaks.cfUtils.Logic.Standard.Structures
{
    using System;
    using System.Linq;

    /// <summary>
    /// Provides a key-value-pair in mutable style so it can be used in serialization scenarios.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public class MutableKeyValuePair<TKey, TValue>
    {
        #region properties

        /// <summary>
        /// The key.
        /// </summary>
        public TKey Key { get; set; }

        /// <summary>
        /// The value.
        /// </summary>
        public TValue Value { get; set; }

        #endregion
    }
}
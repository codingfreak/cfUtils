using System;
using System.Linq;

namespace codingfreaks.cfUtils.Logic.Wpf.MvvmLight
{
    using GalaSoft.MvvmLight.Messaging;

    /// <summary>
    /// An explicit implementation for informations on a changed property.
    /// </summary>
    public class PropertyChangedMessage : PropertyChangedMessageBase
    {
        #region constructors and destructors

        /// <summary>
        /// Initializes a new instance of the PropertyChangedMessage class.
        /// </summary>
        /// <param name="sender">The message's sender.</param>
        /// <param name="target">The message's intended target. This parameter can be used to give an indication as to whom the message was intended for. Of course this is only an indication, amd may be null.</param>
        /// <param name="newValue">The new value of the property.</param>
        /// <param name="propertyName">The name of the property that changed.</param>
        /// <param name="oldValue">The former value of the property.</param>
        public PropertyChangedMessage(object sender, object target, object oldValue, object newValue, string propertyName) : base(sender, target, propertyName)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }

        #endregion

        #region properties

        /// <summary>
        /// The new value of the property.
        /// </summary>
        public object NewValue { get; set; }

        /// <summary>
        /// The former value of the property.
        /// </summary>
        public object OldValue { get; set; }

        #endregion
    }
}
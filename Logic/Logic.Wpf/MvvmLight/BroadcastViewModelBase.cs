using System;
using System.Linq;

namespace s2.s2Utils.Logic.Wpf.MvvmLight
{
    using GalaSoft.MvvmLight;

    /// <summary>
    /// Abstract base class for all view models that will support auto-broadcasting property-changes
    /// to the messenger. 
    /// </summary>
    /// <remarks>
    /// This view model is important for the use of <see cref="AutoRelayCommand"/>.
    /// </remarks>
    public abstract class BroadcastViewModelBase : ViewModelBase
    {
        #region methods

        /// <summary>
        /// A new implementation of the <see cref="RaisePropertyChanged"/> method that will be used by 
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="oldValue">The former value of the property.</param>
        /// <param name="newValue">The new value of the property.</param>
        protected virtual void RaisePropertyChanged(string propertyName, object oldValue, object newValue)
        {
            if (propertyName == null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }
            RaisePropertyChanged(propertyName);
            Broadcast(oldValue, newValue, propertyName);
        }

        #endregion
    }
}
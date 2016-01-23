using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace codingfreaks.cfUtils.Logic.Base.Structures
{
    using System.Collections;
    using System.Collections.Concurrent;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    using Interfaces;

    /// <summary>
    /// Abstract base class for transport models bindable by MVVM and MVC.
    /// </summary>
    /// <remarks>
    /// It is assumed that you use something like Fody to generate your automatic <see cref="INotifyPropertyChanged"/> propagation
    /// calls. This class will implement <see cref="OnPropertyChanged"/> by itself so that <see cref="INotifyDataErrorInfo"/> can
    /// be wired up. This makes it compatible with both approaches.
    /// </remarks>
    public abstract class BaseTransportModel : IEntity, INotifyDataErrorInfo
    {
        #region member vars

        private readonly ConcurrentDictionary<string, List<string>> _errors = new ConcurrentDictionary<string, List<string>>();

        private readonly object _lock = new object();

        #endregion

        #region event declarations

        /// <summary>
        /// Occurs when the validation errors have changed for a property or for the entire entity. 
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region properties

        public bool HasErrors
        {
            get
            {
                return _errors.Any(kv => kv.Value != null && kv.Value.Count > 0);
            }
        }

        /// <summary>
        /// Identifier of values.
        /// </summary>
        [Display(AutoGenerateField = false)]
        public long Id { get; set; }

        #endregion

        #region methods

        /// <summary>
        /// Clone a transport model without id.
        /// </summary>
        /// <returns>A cloned transport model.</returns>
        public virtual object Clone()
        {
            var clone = MemberwiseClone();
            if (clone is IEntity)
            {
                (clone as IEntity).Id = 0;
            }
            return clone;
        }

        /// <summary>
        /// Gets the validation errors for a specified property or for the entire entity.
        /// </summary>
        /// <returns>
        /// The validation errors for the property or entity.
        /// </returns>
        /// <param name="propertyName">The name of the property to retrieve validation errors for; or null or <see cref="F:System.String.Empty"/>, to retrieve entity-level errors.</param>
        public IEnumerable GetErrors(string propertyName)
        {
            List<string> errorsForName;
            _errors.TryGetValue(propertyName, out errorsForName);
            return errorsForName;
        }

        /// <summary>
        ///  Wrapper for firing the <see cref="PropertyChanged"/> event if at least one listener is attached to it.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        public virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler == null)
            {
                return;
            }
            handler(this, new PropertyChangedEventArgs(propertyName));
            ValidateAsync();
        }

        /// <summary>
        /// Perfoms the internal validation of the model.
        /// </summary>
        /// <remarks>
        /// Is normally triggered by <see cref="OnPropertyChanged"/>.
        /// </remarks>
        public void Validate()
        {
            lock (_lock)
            {
                var validationContext = new ValidationContext(this, null, null);
                var validationResults = new List<ValidationResult>();
                Validator.TryValidateObject(this, validationContext, validationResults, true);
                foreach (var kv in _errors.ToList())
                {
                    if (validationResults.All(r => r.MemberNames.All(m => m != kv.Key)))
                    {
                        List<string> outLi;
                        _errors.TryRemove(kv.Key, out outLi);
                        OnErrorsChanged(kv.Key);
                    }
                }
                var q = from r in validationResults from m in r.MemberNames group r by m into g select g;
                foreach (var prop in q)
                {
                    var messages = prop.Select(r => r.ErrorMessage).ToList();

                    if (_errors.ContainsKey(prop.Key))
                    {
                        List<string> outLi;
                        _errors.TryRemove(prop.Key, out outLi);
                    }
                    _errors.TryAdd(prop.Key, messages);
                    OnErrorsChanged(prop.Key);
                }
            }
        }

        /// <summary>
        /// Async call wrapper for <see cref="Validate"/>.
        /// </summary>        
        public Task ValidateAsync()
        {
            return Task.Run(() => Validate());
        }

        /// <summary>
        /// Wrapper for firing the <see cref="ErrorsChanged"/> event if at least one listener is attached to it.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        protected void OnErrorsChanged(string propertyName)
        {
            var handler = ErrorsChanged;
            if (handler != null)
            {
                handler(this, new DataErrorsChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}

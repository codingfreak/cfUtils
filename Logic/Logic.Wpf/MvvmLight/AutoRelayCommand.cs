namespace codingfreaks.cfUtils.Logic.Wpf.MvvmLight
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Windows.Threading;

    using GalaSoft.MvvmLight.Command;
    using GalaSoft.MvvmLight.Messaging;

    /// <summary>
    /// A command that can be bound to dependend properties using <see cref="DependsOn{T}"/>.
    /// </summary>
    /// <remarks>
    /// After a property is bind via <see cref="DependsOn{T}"/> this command will fire 
    /// RaiseCanExecuteChanged automatically.
    /// </remarks>
    public class AutoRelayCommand : RelayCommand, IDisposable
    {
        #region member vars

        private bool _isDisposed;
        private ISet<string> _properties;

        #endregion

        #region constructors and destructors

        /// <summary>
        /// Initializes a new instance of the AutoRelayCommand class that can always execute.
        /// </summary>
        /// <param name="execute">The action of the command.</param>
        public AutoRelayCommand(Action execute) : base(execute)
        {
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the AutoRelayCommand class that can always execute.
        /// </summary>
        /// <param name="execute">The action of the command.</param>
        /// <param name = "canExecute" > The execution status logic.</param>
        public AutoRelayCommand(Action execute, Func<bool> canExecute) : base(execute, canExecute)
        {
            Initialize();
        }

        #endregion

        #region explicit interfaces

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region methods

        /// <summary>
        /// Attaches a messenger-based observation for the property inside the given <paramref name="propertyExpression"/>.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="propertyExpression">The expression resulting in a property.</param>
        public void DependsOn<T>(Expression<Func<T>> propertyExpression)
        {
            if (_properties == null)
            {
                _properties = new HashSet<string>();
            }
            _properties.Add(GetPropertyName(propertyExpression));
        }

        /// <summary>
        /// Is used to savely dispose all unmanaged resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> if this was called from the internal disposing method.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed)
            {
                return;
            }
            if (disposing)
            {
                Messenger.Default.Unregister(this);
            }
            _isDisposed = true;
        }

        /// <summary>
        /// Retrieves the name of a property out of a lambda expression.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="propertyExpression">The lambda expression.</param>
        /// <returns>The name of the property.</returns>
        private static string GetPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            if (propertyExpression == null)
            {
                throw new ArgumentNullException(nameof(propertyExpression));
            }
            var body = propertyExpression.Body as MemberExpression;
            if (body == null)
            {
                throw new ArgumentException("Invalid argument", nameof(propertyExpression));
            }
            var property = body.Member as PropertyInfo;
            if (property == null)
            {
                throw new ArgumentException("Argument is not a property", nameof(propertyExpression));
            }
            return property.Name;
        }

        /// <summary>
        /// Attaches this instance to the MVVM messenger appropriately.
        /// </summary>
        private void Initialize()
        {
            Messenger.Default.Register<PropertyChangedMessageBase>(
                this,
                true,
                property =>
                {
                    if (_properties != null && _properties.Contains(property.PropertyName))
                    {
                        Dispatcher.CurrentDispatcher.Invoke(RaiseCanExecuteChanged);
                    }
                });
        }

        #endregion
    }

    /// <summary>
    /// A command that can be bound to dependend properties using <see cref="DependsOn{T}"/>.
    /// </summary>
    /// <remarks>
    /// After a property is bind via <see cref="DependsOn{T}"/> this command will fire 
    /// RaiseCanExecuteChanged automatically.
    /// </remarks>
    public class AutoRelayCommand<TArg> : RelayCommand<TArg>, IDisposable
    {
        #region member vars

        private bool _isDisposed;
        private ISet<string> _properties;

        #endregion

        #region constructors and destructors

        /// <summary>
        /// Initializes a new instance of the AutoRelayCommand class that can always execute.
        /// </summary>
        /// <param name="execute">The action of the command.</param>
        public AutoRelayCommand(Action<TArg> execute) : base(execute)
        {
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the AutoRelayCommand class that can always execute.
        /// </summary>
        /// <param name="execute">The action of the command.</param>
        /// <param name = "canExecute" > The execution status logic.</param>
        public AutoRelayCommand(Action<TArg> execute, Func<TArg, bool> canExecute) : base(execute, canExecute)
        {
            Initialize();
        }

        #endregion

        #region explicit interfaces

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region methods

        /// <summary>
        /// Attaches a messenger-based observation for the property inside the given <paramref name="propertyExpression"/>.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="propertyExpression">The expression resulting in a property.</param>
        public void DependsOn<T>(Expression<Func<T>> propertyExpression)
        {
            if (_properties == null)
            {
                _properties = new HashSet<string>();
            }
            _properties.Add(GetPropertyName(propertyExpression));
        }

        /// <summary>
        /// Is used to savely dispose all unmanaged resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> if this was called from the internal disposing method.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed)
            {
                return;
            }
            if (disposing)
            {
                Messenger.Default.Unregister(this);
            }
            _isDisposed = true;
        }

        /// <summary>
        /// Retrieves the name of a property out of a lambda expression.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="propertyExpression">The lambda expression.</param>
        /// <returns>The name of the property.</returns>
        private static string GetPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            if (propertyExpression == null)
            {
                throw new ArgumentNullException(nameof(propertyExpression));
            }
            var body = propertyExpression.Body as MemberExpression;
            if (body == null)
            {
                throw new ArgumentException("Invalid argument", nameof(propertyExpression));
            }
            var property = body.Member as PropertyInfo;
            if (property == null)
            {
                throw new ArgumentException("Argument is not a property", nameof(propertyExpression));
            }
            return property.Name;
        }

        /// <summary>
        /// Attaches this instance to the MVVM messenger appropriately.
        /// </summary>
        private void Initialize()
        {
            Messenger.Default.Register<PropertyChangedMessageBase>(
                this,
                true,
                property =>
                {
                    if (_properties != null && _properties.Contains(property.PropertyName))
                    {
                        Dispatcher.CurrentDispatcher.Invoke(RaiseCanExecuteChanged);
                    }
                });
        }

        #endregion
    }
}
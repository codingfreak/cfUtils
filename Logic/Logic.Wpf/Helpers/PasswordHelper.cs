namespace s2.s2Utils.Logic.Wpf.Helpers
{
    using System;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Provides binding for <see cref="PasswordBox"/>.
    /// </summary>
    public static class PasswordHelper
    {
        #region methods

        /// <summary>
        /// Retrieves the <see cref="AttachProperty"/>.
        /// </summary>
        /// <param name="target">The target object.</param>
        /// <returns></returns>
        public static bool GetAttach(DependencyObject target)
        {
            return (bool)target.GetValue(AttachProperty);
        }

        /// <summary>
        /// Retrieves the current password value.
        /// </summary>
        /// <param name="target">The target object.</param>
        /// <returns></returns>
        public static string GetPassword(DependencyObject target)
        {
            return (string)target.GetValue(PasswordProperty);
        }

        /// <summary>
        /// Sets the value of <see cref="AttachProperty"/> to the given <paramref name="value"/>.
        /// </summary>
        /// <param name="target">The target object.</param>
        /// <param name="value">The new value for <see cref="AttachProperty"/>.</param>
        public static void SetAttach(DependencyObject target, bool value)
        {
            target.SetValue(AttachProperty, value);
        }

        /// <summary>
        /// Changes the password to the new <paramref name="value"/>.
        /// </summary>
        /// <param name="target">The target object.</param>
        /// <param name="value">The new password.</param>
        public static void SetPassword(DependencyObject target, string value)
        {
            target.SetValue(PasswordProperty, value);
        }

        private static void Attach(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            if (passwordBox == null)
            {
                return;
            }
            if ((bool)e.OldValue)
            {
                passwordBox.PasswordChanged -= PasswordChanged;
            }
            if ((bool)e.NewValue)
            {
                passwordBox.PasswordChanged += PasswordChanged;
            }
        }

        private static bool GetIsUpdating(DependencyObject dp)
        {
            return (bool)dp.GetValue(IsUpdatingProperty);
        }

        private static void OnPasswordPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            if (passwordBox == null)
            {
                throw new ArgumentException(nameof(sender));
            }            
            passwordBox.PasswordChanged -= PasswordChanged;

            if (!GetIsUpdating(passwordBox))
            {
                passwordBox.Password = (string)e.NewValue;
            }
            passwordBox.PasswordChanged += PasswordChanged;
        }

        private static void PasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            if (passwordBox == null)
            {
                throw new ArgumentException(nameof(sender));
            }
            SetIsUpdating(passwordBox, true);
            SetPassword(passwordBox, passwordBox.Password);
            SetIsUpdating(passwordBox, false);
        }

        /// <summary>
        /// Changes the state of <see cref="IsUpdatingProperty"/> to the <paramref name="value"/>.
        /// </summary>
        /// <param name="target">The target property.</param>
        /// <param name="value">The new value for <see cref="IsUpdatingProperty"/>.</param>
        private static void SetIsUpdating(DependencyObject target, bool value)
        {
            target.SetValue(IsUpdatingProperty, value);
        }

        #endregion

        /// <summary>
        /// The password string.
        /// </summary>
        public static readonly DependencyProperty PasswordProperty = DependencyProperty.RegisterAttached(
            "Password",
            typeof(string),
            typeof(PasswordHelper),
            new FrameworkPropertyMetadata(string.Empty, OnPasswordPropertyChanged));
        
        /// <summary>
        /// The property for attachings.
        /// </summary>
        public static readonly DependencyProperty AttachProperty = DependencyProperty.RegisterAttached("Attach", typeof(bool), typeof(PasswordHelper), new PropertyMetadata(false, Attach));

        private static readonly DependencyProperty IsUpdatingProperty = DependencyProperty.RegisterAttached("IsUpdating", typeof(bool), typeof(PasswordHelper));
    }
}
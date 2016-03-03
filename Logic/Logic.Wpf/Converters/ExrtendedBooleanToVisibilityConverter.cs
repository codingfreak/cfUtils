namespace codingfreaks.cfUtils.Logic.Wpf.Converters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    /// <summary>
    /// Convertes a boolean value to its opposite.
    /// </summary>
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class ExrtendedBooleanToVisibilityConverter : IValueConverter
    {
        #region explicit interfaces

        /// <summary>
        /// Converts a boolean <paramref name="value"/> into a WPF <see cref="Visibility"/> depending on the <paramref name="parameter"/> which can be of string "Normal" or "Inverted".
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var boolValue = (bool)value;
            var direction = (Parameters)Enum.Parse(typeof(Parameters), (string)parameter);
            if (direction == Parameters.Inverted)
            {
                return !boolValue ? Visibility.Visible : Visibility.Collapsed;
            }
            return boolValue ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value that is produced by the binding target.</param><param name="targetType">The type to convert to.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool))
            {
                value = false;
            }
            var direction = (Parameters)Enum.Parse(typeof(Parameters), (string)parameter);
            return direction == Parameters.Normal ? !((bool)value) : (bool)value;
        }

        #endregion

        enum Parameters
        {
            Normal,
            Inverted
        }
    }
}
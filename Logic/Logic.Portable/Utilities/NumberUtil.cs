namespace codingfreaks.cfUtils.Logic.Portable.Utilities
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Provides methods for eased number-dealing.
    /// </summary>
    public static class NumberUtil
    {
        #region methods

        /// <summary>
        /// Retrieves a string representing the given <paramref name="number"/> as a currency string if it is not <c>null</c>.
        /// </summary>
        /// <param name="number">The number to represent as a string or <c>null</c>.</param>
        /// <returns>The formatted <paramref name="number"/> or <see cref="string.Empty"/>.</returns>
        public static string ToCurrencyString(this decimal? number)
        {
            return number.HasValue ? number.Value.ToCurrencyString() : string.Empty;
        }

        /// <summary>
        /// Retrieves a string representing the given <paramref name="number"/> as a currency string.
        /// </summary>
        /// <param name="number">The number to represent as a string.</param>
        /// <returns>The formatted <paramref name="number"/>.</returns>
        public static string ToCurrencyString(this decimal number)
        {
            return number.ToString("C");
        }

        /// <summary>
        /// Retrieves a string representing the given <paramref name="number"/> as a formatted string.
        /// </summary>
        /// <param name="number">The number to represent as a string.</param>
        /// <param name="precision">The amount of chars after the comma.</param>
        /// <param name="useThousendSeparators"><c>true</c> if a culture-specific separator should be inserted after each 3rd char.</param>
        /// <param name="keepZeroBelowOne">If <c>true</c> 0.34232323 will be '0.34', otherwise it'll be '.34'.</param>
        /// <returns>The formatted <paramref name="number"/> or <see cref="string.Empty"/>.</returns>
        public static string ToFormattedString(this decimal? number, int precision = 2, bool useThousendSeparators = true, bool keepZeroBelowOne = true)
        {
            return number.HasValue ? number.Value.ToFormattedString(precision, useThousendSeparators, keepZeroBelowOne) : string.Empty;
        }

        /// <summary>
        /// Retrieves a string representing the given <paramref name="number"/> as a formatted string.
        /// </summary>
        /// <param name="number">The number to represent as a string.</param>
        /// <param name="precision">The amount of chars after the comma.</param>
        /// <param name="useThousendSeparators"><c>true</c> if a culture-specific separator should be inserted after each 3rd char.</param>
        /// <param name="keepZeroBelowOne">If <c>true</c> 0.34232323 will be '0.34', otherwise it'll be '.34'.</param>
        /// <returns>The formatted <paramref name="number"/>.</returns>
        public static string ToFormattedString(this decimal number, int precision = 2, bool useThousendSeparators = true, bool keepZeroBelowOne = true)
        {
            var zero = keepZeroBelowOne ? "0" : "#";
            var format = useThousendSeparators ? string.Format(CultureInfo.InvariantCulture, "#,#{0}", zero) : string.Format(CultureInfo.InvariantCulture, "#{0}", zero);
            if (precision <= 0)
            {
                // just return the value regardless of the precision
                return number.ToString(format);
            }
            // we have to take care on precision after comma
            var formatPrecision = new String('0', precision);
            format = useThousendSeparators
                ? string.Format(CultureInfo.InvariantCulture, "#,#{0}.{1}", zero, formatPrecision)
                : string.Format(CultureInfo.InvariantCulture, "{0}.{1}", zero, formatPrecision);
            return number.ToString(format);
        }

        #endregion
    }
}
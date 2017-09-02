namespace codingfreaks.cfUtils.Logic.Utils.Utilities
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using Base.Utilities;

    using Standard.Attributes;
    using Standard.Utilities;
    using System.Runtime.Serialization;

    /// <summary>
    /// Provides logic for easy access to <see cref="Enum"/>s.
    /// </summary>
    public static class EnumUtil
    {
        #region methods

        /// <summary>
        /// Converts a single value from an enum of type <typeparamref name="T"/> into a text.
        /// </summary>
        /// <typeparam name="T">The type of the enum.</typeparam>
        /// <param name="value">The specific enum value out of the type.</param>
        /// <returns>The <see cref="EnumMemberAttribute.Value"/> or the name of the <paramref name="value"/>.</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "We cannot make value a T because enums cannot be used for constraints.")]
        public static string EnumValueToText<T>(Enum value)
        {
            if (value == null)
            {
                return string.Empty;
            }
            var enumType = typeof(T);
            if (!enumType.IsEnum)
            {
                throw new InvalidOperationException("Type is no enumration.");
            }
            // get the source-code name of the enum-value
            var valueAsText = value.ToString("G");
            // try to get the EnumMemberAttribute for this enum-value            
            var enumMemberAttribute = ((EnumMemberAttribute[])enumType.GetField(valueAsText).GetCustomAttributes(typeof(EnumMemberAttribute), true)).SingleOrDefault();
            // return the attributes value of the name of the enum-value if no EnumMemberAttribute was found
            return (enumMemberAttribute != null) ? enumMemberAttribute.Value : valueAsText;
        }

        /// <summary>
        /// Converts a single value from an enum of type <typeparamref name="T"/> into a text using <see cref="ResourceUtil"/>.
        /// </summary>
        /// <typeparam name="T">The type of the enum.</typeparam>
        /// <param name="localeId">The locale id for passing to the resources.</param>
        /// <param name="value">The specific enum value out of the type.</param>
        /// <returns>The <see cref="EnumMemberAttribute.Value"/> or the name of the <paramref name="value"/>.</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "We cannot make value a T because enums cannot be used for constraints.")]
        public static string LocalizedEnumValueToText<T>(string localeId, Enum value)
        {
            if (value == null)
            {
                return string.Empty;
            }
            var enumType = typeof(T);
            if (!enumType.IsEnum)
            {
                throw new InvalidOperationException("Type is no enumration.");
            }
            // get the source-code name of the enum-value
            var valueAsText = value.ToString("G");
            // try to get the LocalizedEnumMemberAttribute for this enum-value            
            var enumMemberAttribute = ((LocalizedEnumMemberAttribute[])enumType.GetField(valueAsText).GetCustomAttributes(typeof(LocalizedEnumMemberAttribute), true)).SingleOrDefault();
            // return the attributes value of the name of the enum-value if no LocalizedEnumMemberAttribute was found
            return (enumMemberAttribute != null) ? PortableResourceUtil.Get<string>(localeId, enumMemberAttribute.ResourceKey, enumMemberAttribute.ResourceType) : valueAsText;
        }

        /// <summary>
        /// Tries to convert a given <paramref name="text"/> into the enum value of a given enum-type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the enum.</typeparam>
        /// <param name="text">The name of the enum member or the value of its <see cref="EnumMemberAttribute"/>.</param>
        /// <returns>The enum value.</returns>
        public static T TextToEnum<T>(string text) where T : struct
        {
            CheckUtil.ThrowIfNullOrEmpty(() => text);
            var enumType = typeof(T);
            if (!enumType.IsEnum)
            {
                throw new InvalidOperationException("Type is no enumration.");
            }
            foreach (var field in enumType.GetFields())
            {
                var attribute = Attribute.GetCustomAttribute(field, typeof(EnumMemberAttribute)) as EnumMemberAttribute;
                if (attribute != null && !string.IsNullOrEmpty(attribute.Value))
                {
                    // We found a EnumMemberAttribute with a given Value-property ...
                    if (attribute.Value == text)
                    {
                        // ... and we got a match on the value -> we will retrieve the name of the value.
                        return (T)field.GetValue(null);
                    }
                }
                else
                {
                    // We did not find a EnumMemberAttribute or it's Value-property was empty ...
                    if (string.Equals(text, field.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        // ... and we got a match on the value -> we will retrieve the name of the value.
                        return (T)field.GetValue(null);
                    }
                }
            }
            // If we reach this point, GetFields did not contain the field provided in value
            throw new InvalidOperationException("The provided value is not a member of the given Enum type.");
        }

        /// <summary>
        /// Tries to convert a given <paramref name="text"/> into the enum value of a given enum-type <typeparamref name="T"/> using <see cref="ResourceUtil"/>.
        /// </summary>
        /// <typeparam name="T">The type of the enum.</typeparam>
        /// <param name="localeId">The locale id for passing to the resources.</param>
        /// <param name="text">The name of the enum member or the value of its <see cref="EnumMemberAttribute"/>.</param>
        /// <returns>The enum value.</returns>
        public static T LocalizedTextToEnum<T>(string localeId, string text) where T : struct
        {
            CheckUtil.ThrowIfNullOrEmpty(() => text);
            var enumType = typeof(T);
            if (!enumType.IsEnum)
            {
                throw new InvalidOperationException("Type is no enumration.");
            }
            foreach (var field in enumType.GetFields())
            {
                var attribute = Attribute.GetCustomAttribute(field, typeof(LocalizedEnumMemberAttribute)) as LocalizedEnumMemberAttribute;
                if (attribute != null && !string.IsNullOrEmpty(attribute.ResourceKey))
                {
                    // We found a LocalizedEnumMemberAttribute with a given Value-property ...
                    var textToSearch = PortableResourceUtil.Get<string>(localeId, attribute.ResourceKey, attribute.ResourceType);                    
                    if (textToSearch == text)
                    {
                        // ... and we got a match on the value -> we will retrieve the name of the value.
                        return (T)field.GetValue(null);
                    }
                }
                else
                {
                    // We did not find a LocalizedEnumMemberAttribute or it's Value-property was empty ...
                    if (string.Equals(text, field.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        // ... and we got a match on the value -> we will retrieve the name of the value.
                        return (T)field.GetValue(null);
                    }
                }
            }
            // If we reach this point, GetFields did not contain the field provided in value
            throw new InvalidOperationException("The provided value is not a member of the given Enum type.");
        }

        /// <summary>
        /// Retrieves the enumeration value for a given number.
        /// </summary>
        /// <typeparam name="T">The type of the enum.</typeparam>
        /// <param name="value">The numeric value.</param>
        /// <returns>The enum value if possible.</returns>
        public static T ValueToEnum<T>(int value) where T : struct
        {
            var enumType = typeof(T);
            if (!enumType.IsEnum)
            {
                throw new InvalidOperationException("Type is no enumration.");
            }
            return (T)Enum.ToObject(enumType, value);
        }

        /// <summary>
        /// Retrieves the enumeration value for a given number provided as text.
        /// </summary>
        /// <typeparam name="T">The type of the enum.</typeparam>
        /// <param name="value">The numeric value in form of a text.</param>
        /// <returns>The enum value if possible.</returns>
        public static T ValueToEnum<T>(string value) where T : struct
        {
            CheckUtil.ThrowIfNullOrEmpty(() => value);
            var enumVal = -1;
            if (!int.TryParse(value, out enumVal))
            {
                throw new ArgumentException("The value is not a valid number.", nameof(value));
            }
            var enumType = typeof(T);
            if (!enumType.IsEnum)
            {
                throw new InvalidOperationException("Type is no enumration.");
            }
            return (T)Enum.ToObject(enumType, enumVal);
        }

        #endregion
    }
}
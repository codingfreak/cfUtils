namespace codingfreaks.cfUtils.Logic.Core.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Provides helper-methods for reflection.
    /// </summary>
    public static class ReflectionUtil
    {
        #region methods

        /// <summary>
        /// Wrapper to get the company-name out of an assembly.
        /// </summary>
        /// <param name="assemblyInfo">The assembly-information.</param>
        /// <returns>The name of the company.</returns>
        public static string GetAssemblyCompany(Assembly assemblyInfo)
        {
            var attr = assemblyInfo.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
            if (!attr.Any())
            {
                return string.Empty;
            }
            if (attr.First() is AssemblyCompanyAttribute converted)
            {
                return converted.Company;
            }
            return string.Empty;
        }

        /// <summary>
        /// Wrapper to get the copyright out of an assembly.
        /// </summary>
        /// <param name="assemblyInfo">The assembly-information.</param>
        /// <returns>The copyright-text of the assembly.</returns>
        public static string GetAssemblyCopyright(Assembly assemblyInfo)
        {
            var attr = assemblyInfo.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
            if (!attr.Any())
            {
                return string.Empty;
            }
            if (attr.First() is AssemblyCopyrightAttribute converted)
            {
                return converted.Copyright;
            }
            return string.Empty;
        }

        /// <summary>
        /// Wrapper to get the description out of an assembly.
        /// </summary>
        /// <param name="assemblyInfo">The assembly-information.</param>
        /// <returns>The description of the company.</returns>
        public static string GetAssemblyDescription(Assembly assemblyInfo)
        {
            var attr = assemblyInfo.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
            if (!attr.Any())
            {
                return string.Empty;
            }
            if (attr.First() is AssemblyDescriptionAttribute converted)
            {
                return converted.Description;
            }
            return string.Empty;
        }

        /// <summary>
        /// Wrapper to get the product-name out of an assembly.
        /// </summary>
        /// <param name="assemblyInfo">The assembly-information.</param>
        /// <returns>The name of the product.</returns>
        public static string GetAssemblyProduct(Assembly assemblyInfo)
        {
            var attr = assemblyInfo.GetCustomAttributes(typeof(AssemblyProductAttribute), false);
            if (!attr.Any())
            {
                return string.Empty;
            }
            if (attr.First() is AssemblyProductAttribute converted)
            {
                return converted.Product;
            }
            return string.Empty;
        }

        /// <summary>
        /// Wrapper to get the title out of an assembly.
        /// </summary>
        /// <param name="assemblyInfo">The assembly-information.</param>
        /// <returns>The title of the assembly.</returns>
        public static string GetAssemblyTitle(Assembly assemblyInfo)
        {
            var attr = assemblyInfo.GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
            if (!attr.Any())
            {
                return string.Empty;
            }
            if (attr.First() is AssemblyTitleAttribute converted)
            {
                return converted.Title;
            }
            return string.Empty;
        }

        /// <summary>
        /// Wrapper to get the version out of an assembly.
        /// </summary>
        /// <param name="assemblyInfo">The assembly-information.</param>
        /// <returns>The version of the assembly.</returns>
        public static string GetAssemblyVersion(Assembly assemblyInfo)
        {
            var attr = GetAssemblyVersionAttribute(assemblyInfo);
            return attr != null ? attr.Version : string.Empty;
        }

        /// <summary>
        /// Retrieves all assembly version information.
        /// </summary>
        /// <param name="assemblyInfo">The assembly-information.</param>
        /// <returns>The version of the assembly.</returns>
        public static AssemblyVersionAttribute GetAssemblyVersionAttribute(Assembly assemblyInfo)
        {
            var attr = assemblyInfo.GetCustomAttributes(typeof(AssemblyVersionAttribute), false);
            if (!attr.Any())
            {
                return null;
            }
            return attr.First() as AssemblyVersionAttribute;
        }

        /// <summary>
        /// Retrieves all public constants inside the given <paramref name="type" />.
        /// </summary>
        /// <param name="type">The type to extend.</param>
        /// <returns>All public constants of the type.</returns>
        public static IEnumerable<FieldInfo> GetPublicConstants(this Type type)
        {
            return type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy).Where(fi => fi.IsLiteral && !fi.IsInitOnly);
        }

        /// <summary>
        /// Checks if a given <paramref name="type" /> provides a property named <paramref name="propertyName" />.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="mustBeWritable">Indicates whether the property must be writable.</param>
        /// <returns><c>true</c> if the property exists, otherwise <c>false</c>.</returns>
        public static bool HasProperty(this Type type, string propertyName, bool mustBeWritable = false)
        {
            var prop = type.GetProperty(propertyName);
            if (prop == null)
            {
                return false;
            }
            return !mustBeWritable || prop.CanWrite;
        }

        /// <summary>
        /// Checks if a given <paramref name="type" /> provides a property named <paramref name="propertyName" /> and a given
        /// <paramref name="returnType" />.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="returnType">The type of the property.</param>
        /// <param name="mustBeWritable">Indicates whether the property must be writable.</param>
        /// <returns><c>true</c> if the property exists, otherwise <c>false</c>.</returns>
        public static bool HasProperty(this Type type, string propertyName, Type returnType, bool mustBeWritable = false)
        {
            var prop = type.GetProperty(propertyName, returnType);
            if (prop == null)
            {
                return false;
            }
            return !mustBeWritable || prop.CanWrite;
        }

        /// <summary>
        /// Changes the value of a <paramref name="propertyName" /> on a given <paramref name="instance" /> including type-checks.
        /// </summary>
        /// <typeparam name="TPropertyType">The type of the property.</typeparam>
        /// <param name="type">The type of the instance.</param>
        /// <param name="instance">The instance itself.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="value">The new value for the property.</param>
        public static void SetProperty<TPropertyType>(this Type type, object instance, string propertyName, TPropertyType value)
        {
            if (!type.HasProperty(propertyName, typeof(TPropertyType), true))
            {
                throw new InvalidOperationException("Could not find property on type or property isn't writable.");
            }
            var prop = type.GetProperty(propertyName, typeof(TPropertyType));
            prop?.SetValue(instance, value, null);
        }

        #endregion
    }
}
namespace s2.s2Utils.Logic.Base.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Provides helper-methods for reflection.
    /// </summary>
    public static class ReflectionUtil
    {
        #region methods

        /// <summary>
        /// Retrieves all currently visible assemblies from the AppDomain.
        /// </summary>
        /// <returns>The list of assemblies.</returns>
        public static IEnumerable<Assembly> GetAvailableAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies();
        }

        #endregion
    }
}
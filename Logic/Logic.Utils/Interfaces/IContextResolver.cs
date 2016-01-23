namespace s2.s2Utils.Logic.Utils.Interfaces
{
    using System;
    using System.Data.Entity;

    /// <summary>
    /// Must be implemented by each type which is able to resolve entity contexts.
    /// </summary>
    public interface IContextResolver
    {
        #region methods

        /// <summary>
        /// Will retrieve the special db context when a type is provided.
        /// </summary>
        /// <param name="targetType">The type which is known on caller side so far. (Should be something inherting from DbContext).</param>
        /// <returns>Either the result from the correct context util or <c>null</c> if the <paramref name="targetType"/> could not be mapped to a known DbContext.</returns>
        DbContext GetContext(Type targetType);

        #endregion
    }
}
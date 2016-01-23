namespace codingfreaks.cfUtils.Logic.Utils.Extensions
{
    using System;
    using System.Collections;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Data.Entity;

    /// <summary>
    /// Provides extension methods useful for LINQ.
    /// </summary>
    public static class LinqExtensions
    {
        #region methods

        /// <summary>
        /// Extends an queryable list by ordering.
        /// </summary>
        /// <typeparam name="T">The type of entity in the collection.</typeparam>
        /// <param name="source">The original query.</param>
        /// <param name="property">The name of the property.</param>
        /// <returns>The ordered query.</returns>
        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string property)
        {
            return ApplyOrder(source, property, "OrderBy");
        }

        /// <summary>
        /// Adds an Include expression to each navigation property of a query-result.
        /// </summary>
        /// <typeparam name="T">The type of entity contained in the expression.</typeparam>
        /// <param name="source">The original query.</param>
        /// <returns>The eager-loaded query.</returns>
        public static IQueryable<T> LoadEager<T>(this IQueryable<T> source)
        {
            var typesToIgnore = new[] { typeof(string), typeof(DateTime), typeof(DateTime?) };
            var properties = typeof(T).GetProperties().Where(p => !typesToIgnore.Contains(p.PropertyType)).ToList();
            return properties.Where(property => property.PropertyType.IsClass || typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
                .Aggregate(source, (current, property) => current.Include(property.Name).AsQueryable());
        }

        /// <summary>
        /// Extends an queryable list by ordering.
        /// </summary>
        /// <typeparam name="T">The type of entity in the collection.</typeparam>
        /// <param name="source">The original query.</param>
        /// <param name="property">The name of the property.</param>
        /// <returns>The ordered query.</returns>
        public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string property)
        {
            return ApplyOrder(source, property, "OrderByDescending");
        }

        /// <summary>
        /// Extends an queryable list by ordering.
        /// </summary>
        /// <typeparam name="T">The type of entity in the collection.</typeparam>
        /// <param name="source">The original query.</param>
        /// <param name="property">The name of the property.</param>
        /// <returns>The ordered query.</returns>
        public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> source, string property)
        {
            return ApplyOrder(source, property, "ThenBy");
        }

        /// <summary>
        /// Extends an queryable list by ordering.
        /// </summary>
        /// <typeparam name="T">The type of entity in the collection.</typeparam>
        /// <param name="source">The original query.</param>
        /// <param name="property">The name of the property.</param>
        /// <returns>The ordered query.</returns>
        public static IOrderedQueryable<T> ThenByDescending<T>(this IOrderedQueryable<T> source, string property)
        {
            return ApplyOrder(source, property, "ThenByDescending");
        }

        /// <summary>
        /// Uses expression-building to apply ordering to an IQueryable (server-side ordering).
        /// </summary>
        /// <typeparam name="T">The type of entity in the collection.</typeparam>
        /// <param name="source">The original query.</param>
        /// <param name="property">The name of the property.</param>
        /// <param name="methodName">The name of the LINQ-method to use for ordering.</param>
        /// <returns>The ordered query.</returns>
        private static IOrderedQueryable<T> ApplyOrder<T>(IQueryable<T> source, string property, string methodName)
        {
            var props = property.Split('.');
            var type = typeof(T);
            var arg = Expression.Parameter(type, "x");
            Expression expr = arg;
            foreach (var prop in props)
            {
                // use reflection (not ComponentModel) to mirror LINQ
                var pi = type.GetProperty(prop);
                expr = Expression.Property(expr, pi);
                type = pi.PropertyType;
            }
            var delegateType = typeof(Func<,>).MakeGenericType(typeof(T), type);
            var lambda = Expression.Lambda(delegateType, expr, arg);

            var result =
                typeof(Queryable).GetMethods()
                    .Single(method => method.Name == methodName && method.IsGenericMethodDefinition && method.GetGenericArguments().Length == 2 && method.GetParameters().Length == 2)
                    .MakeGenericMethod(typeof(T), type)
                    .Invoke(null, new object[] { source, lambda });
            return (IOrderedQueryable<T>)result;
        }

        #endregion
    }
}
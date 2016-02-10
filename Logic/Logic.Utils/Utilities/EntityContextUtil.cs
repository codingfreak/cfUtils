namespace codingfreaks.cfUtils.Logic.Utils.Utilities
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Data.Entity;

    using codingfreaks.cfUtils.Logic.Utils.Extensions;

    using System.Linq;
    using System.Threading.Tasks;

    using codingfreaks.cfUtils.Logic.Base.Interfaces;
    using codingfreaks.cfUtils.Logic.Base.Utilities;
    using codingfreaks.cfUtils.Logic.Utils.Interfaces;

    /// <summary>
    /// Utils available to the public.
    /// </summary>
    public static class EntityContextUtil
    {
        #region methods

        /// <summary>
        /// Allows to execute a given <paramref name="action"/> within an ensured <paramref name="ctx"/> including 
        /// disposing the <paramref name="ctx"/> if none was given.
        /// </summary>
        /// <param name="action">The action to execute including an EF context.</param>
        /// <param name="ctx">The context or <c>null</c> if a new should be created.</param>
        /// <param name="resolver">A type which knows how to get the correct DbContext for a given entity type.</param>
        /// <param name="logger">The logger to use inside the operation.</param>
        public static void ExecuteContextWrapped<TContext>(Action<TContext> action, TContext ctx, IContextResolver resolver, ILogger logger = null) where TContext : DbContext
        {
            if (action == null)
            {
                var ex = new ArgumentNullException(nameof(action));
                logger?.LogException("ECU-EX-01", ex);
                throw ex;
            }
            var dispose = false;
            if (ctx == null)
            {
                ctx = resolver.GetContext(typeof(TContext)) as TContext;
                if (ctx == null)
                {
                    throw new InvalidOperationException("Can not resolve database context.");
                }
                dispose = true;
            }
            action.Invoke(ctx);
            if (dispose)
            {
                ctx.Dispose();
            }
        }

        /// <summary>
        /// Allows to execute a given <paramref name="func"/> within an ensured <paramref name="ctx"/> including 
        /// disposing the <paramref name="ctx"/> if none was given.
        /// </summary>        
        /// <typeparam name="TResult">The result type of the <paramref name="func"/>.</typeparam>
        /// <typeparam name="TContext">The type of DbContext which should be used.</typeparam>
        /// <param name="func">The function to execute including an EF context.</param>
        /// <param name="ctx">The context or <c>null</c> if a new should be created.</param>
        /// <param name="resolver">A type which knows how to get the correct DbContext for a given entity type.</param>
        /// <param name="logger">The logger to use inside the operation.</param>
        /// <returns>The result of the <paramref name="func"/>.</returns>
        public static TResult ExecuteContextWrapped<TContext, TResult>(Func<TContext, TResult> func, TContext ctx, IContextResolver resolver, ILogger logger = null) where TContext : DbContext
        {
            if (func == null)
            {
                var ex = new ArgumentNullException(nameof(func));
                logger?.LogException("ECU-EX-02", ex);
                throw ex;
            }
            var dispose = false;
            if (ctx == null)
            {
                ctx = resolver.GetContext(typeof(TContext)) as TContext;
                if (ctx == null)
                {
                    throw new InvalidOperationException("Can not resolve database context.");
                }
                dispose = true;
            }
            var result = func.Invoke(ctx);
            if (dispose)
            {
                ctx.Dispose();
            }
            return result;
        }

        /// <summary>
        /// Allows to execute a given <paramref name="func"/> within an ensured <paramref name="ctx"/> including 
        /// disposing the <paramref name="ctx"/> if none was given.
        /// </summary>        
        /// <typeparam name="TResult">The result type of the <paramref name="func"/>.</typeparam>
        /// <typeparam name="TContext">The type of DbContext which should be used.</typeparam>
        /// <param name="func">The function to execute including an EF context.</param>
        /// <param name="ctx">The context or <c>null</c> if a new should be created.</param>
        /// <param name="resolver">A type which knows how to get the correct DbContext for a given entity type.</param>
        /// <param name="logger">The logger to use inside the operation.</param>
        /// <returns>The result of the <paramref name="func"/>.</returns>
        public static async Task<TResult> ExecuteContextWrappedAsync<TContext, TResult>(Func<TContext, Task<TResult>> func, TContext ctx, IContextResolver resolver, ILogger logger = null)
            where TContext : DbContext
        {
            if (func == null)
            {
                var ex = new ArgumentNullException(nameof(func));
                logger?.LogException("ECU-EX-03", ex);
                throw ex;
            }
            var dispose = false;
            if (ctx == null)
            {
                ctx = resolver.GetContext(typeof(TContext)) as TContext;
                if (ctx == null)
                {
                    throw new InvalidOperationException("Can not resolve database context.");
                }
                dispose = true;
            }
            var result = await func.Invoke(ctx);
            if (dispose)
            {
                ctx.Dispose();
            }
            return result;
        }
        
        #endregion
    }
}
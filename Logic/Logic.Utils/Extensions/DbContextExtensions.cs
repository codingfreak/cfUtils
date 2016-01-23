namespace s2.s2Utils.Logic.Utils.Extensions
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Core.Objects;
    using System.Data.Entity.Infrastructure;

    using Base.Utilities;

    using Base.Interfaces;

    /// <summary>
    /// Extends any <see cref="DbContext" />.
    /// </summary>
    public static class DbContextExtensions
    {
        #region methods

        /// <summary>
        /// Retrieves the <see cref="ObjectContext" /> from a given
        /// </summary>
        /// <param name="ctx">The context to extend.</param>
        /// <returns>The ObjectContext.</returns>
        public static ObjectContext ToObjectContext(this DbContext ctx)
        {
            return ((IObjectContextAdapter)ctx).ObjectContext;
        }

        /// <summary>
        /// Tries to call the Attach method on a set of <typeparamref name="TEntity" /> for a given <paramref name="entity" />.
        /// </summary>
        /// <typeparam name="TEntity">The type of the <paramref name="entity" />.</typeparam>
        /// <param name="ctx">The context to extend.</param>
        /// <param name="entity">The entity which should be attached.</param>
        /// <returns><c>true</c> if the operation succeeds, otherwise <c>false</c>.</returns>
        [Obsolete("Do not use this approach any longer! Instead of this reload the entity in the other context.")]
        public static bool TryAttachEntity<TEntity>(this DbContext ctx, TEntity entity) where TEntity : class, IEntity
        {
            try
            {
                if (ctx.Entry(entity).State == EntityState.Detached)
                {
                    ctx.Set<TEntity>().Attach(entity);
                    return true;
                }
            }
            catch (Exception ex)
            {
                TraceUtil.WriteTraceError(ex.Message);
            }
            return false;
        }

        #endregion
    }
}
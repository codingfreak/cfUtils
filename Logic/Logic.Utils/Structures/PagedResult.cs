namespace codingfreaks.cfUtils.Logic.Utils.Structures
{
    using System;
    using System.Data.Entity;

    using Extensions;

    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    using Base.Enumerations;
    using Base.Interfaces;
    using Base.Structures;

    using Enumerations;

    using Standard.Utilities;

    using Utilities;

    /// <summary>
    /// Encapsulates data and logic for handling paged results.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity each element of Items will be of.</typeparam>
    /// <typeparam name="TContext">The type of entity context to use to access the database.</typeparam>
    public class PagedResult<TEntity, TContext> : BasePagedResult<TEntity>
        where TEntity : class, IEntity where TContext : DbContext
    {
        #region member vars

        private BaseUtil<TEntity, TContext> _util;

        #endregion

        #region constructors and destructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="request">The request for generating the result.</param>
        public PagedResult(PagedRequest request) : base(request)
        {
        }

        /// <summary>
        /// Additional constructor for passing the <see cref="Util"/> in.
        /// </summary>
        /// <param name="request">The request for generating the result.</param>
        /// <param name="util">The util to use for database-related operations.</param>
        public PagedResult(PagedRequest request, BaseUtil<TEntity, TContext> util) : base(request)
        {
            _util = util;
        }

        #endregion

        #region methods

        /// <summary>
        /// Retrieves the result.
        /// </summary>
        /// <param name="ctx">The entity context to use.</param>
        /// <param name="query">An optional query on the entities to use (if <c>null</c>, GetValidEntities is used).</param>
        /// <param name="filter">An optional filter expression.</param>        
        public void Generate(TContext ctx, IQueryable<TEntity> query = null, Expression<Func<TEntity, bool>> filter = null)
        {
            CheckUtil.ThrowIfNull(() => Util);
            if (query == null)
            {
                query = Util.GetValidEntities(ctx, filter);
            }
            CompleteItemsCount = query.Count();
            if (Request.OrderByExpressions != null && Request.OrderByExpressions.Any())
            {
                // we habe to cycle through each desired order-expression and build up our query-
                var firstItem = true;
                foreach (var orderBy in Request.OrderByExpressions)
                {
                    if (firstItem)
                    {
                        // this is the first item so that ThenBy can not be applied
                        query = orderBy.OrderDirection == OrderByDirections.Ascending ? query.OrderBy(orderBy.PropertyName) : query.OrderByDescending(orderBy.PropertyName);
                        firstItem = false;
                    }
                    else
                    {
                        // this is not the first item so that each order has to be done using Then*
                        var ordered = query as IOrderedQueryable<TEntity>;
                        if (ordered == null)
                        {
                            throw new InvalidOperationException("Could not retrieve ordered elements from previous iteration.");
                        }
                        query = orderBy.OrderDirection == OrderByDirections.Ascending ? ordered.ThenBy(orderBy.PropertyName) : ordered.ThenByDescending(orderBy.PropertyName);
                    }
                }
            }
            else
            {
                // order by Id if no other order is defined
                query = query.OrderBy(e => e.Id);
            }
            // generate and return the result
            var itemsToSkip = Request.EntriesToSkip ?? (Request.PageToDeliver - 1) * Request.ItemsPerPage;
            Items = query.Skip(itemsToSkip).Take(Request.ItemsPerPage).ToList();
        }

        /// <summary>
        /// Retrieves the result asynchronously.
        /// </summary>
        /// <param name="ctx">The entity context to use.</param>
        /// <param name="query">An optional query on the entities to use (if <c>null</c>, GetValidEntities is used).</param>
        /// <param name="filter">An optional filter expression.</param>        
        public async Task GenerateAsync(TContext ctx, IQueryable<TEntity> query = null, Expression<Func<TEntity, bool>> filter = null)
        {
            await GenerateAsync(ctx, CancellationToken.None, query, filter).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves the result asynchronously.
        /// </summary>
        /// <param name="ctx">The entity context to use.</param>
        /// <param name="cancellationToken">A token for cancelling the opertation.</param>
        /// <param name="query">An optional query on the entities to use (if <c>null</c>, GetValidEntities is used).</param>
        /// <param name="filter">An optional filter expression.</param>     
        public async Task GenerateAsync(TContext ctx, CancellationToken cancellationToken, IQueryable<TEntity> query = null, Expression<Func<TEntity, bool>> filter = null)
        {            
            if (query == null)
            {
                CheckUtil.ThrowIfNull(() => Util);
                query = Util.GetValidEntities(ctx, filter);
            }
            CompleteItemsCount = await query.CountAsync(cancellationToken).ConfigureAwait(false);
            if (Request.OrderByExpressions != null && Request.OrderByExpressions.Any())
            {
                // we habe to cycle through each desired order-expression and build up our query-
                var firstItem = true;
                foreach (var orderBy in Request.OrderByExpressions)
                {
                    if (firstItem)
                    {
                        // this is the first item so that ThenBy can not be applied
                        query = orderBy.OrderDirection == OrderByDirections.Ascending ? query.OrderBy(orderBy.PropertyName) : query.OrderByDescending(orderBy.PropertyName);
                        firstItem = false;
                    }
                    else
                    {
                        // this is not the first item so that each order has to be done using Then*
                        var ordered = query as IOrderedQueryable<TEntity>;
                        if (ordered == null)
                        {
                            throw new InvalidOperationException("Could not retrieve ordered elements from previous iteration.");
                        }
                        query = orderBy.OrderDirection == OrderByDirections.Ascending ? ordered.ThenBy(orderBy.PropertyName) : ordered.ThenByDescending(orderBy.PropertyName);
                    }
                }
            }
            else
            {
                // order by Id if no other order is defined
                query = query.OrderBy(e => e.Id);
            }
            // generate and return the result
            var itemsToSkip = Request.EntriesToSkip ?? (Request.PageToDeliver - 1) * Request.ItemsPerPage;
            Items = await query.Skip(itemsToSkip).Take(Request.ItemsPerPage).ToListAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Allows easy access to a <see cref="BasePagedResult{TEntity}"/> using a static one-line-call.
        /// </summary>
        /// <param name="request">The request for generating the result.</param>
        /// <param name="ctx">The entity context to use.</param>
        /// <param name="query">An optional query on the entities to use (if <c>null</c>, GetValidEntities is used).</param>
        /// <param name="filter">An optional filter expression.</param>
        /// <param name="util">Optionally the util to use for database-related operations.</param>
        /// <returns>The paged result.</returns>
        public static Task<BasePagedResult<TEntity>> GetBaseResultAsync(
            PagedRequest request,
            TContext ctx,
            IQueryable<TEntity> query = null,
            Expression<Func<TEntity, bool>> filter = null,
            BaseUtil<TEntity, TContext> util = null)
        {
            var completionSource = new TaskCompletionSource<BasePagedResult<TEntity>>();
            Task.Run(
                async () =>
                {
                    try
                    {
                        var result = new PagedResult<TEntity, TContext>(request, util);
                        await result.GenerateAsync(ctx, query, filter).ConfigureAwait(false);
                        if (!completionSource.TrySetResult(result.ToBasePagedResult()))
                        {
                            completionSource.TrySetException(new InvalidOperationException("Could not retrieve result."));
                        }
                    }
                    catch (Exception ex)
                    {
                        completionSource.TrySetException(ex);
                    }
                });
            return completionSource.Task;
        }

        /// <summary>
        /// Allows easy access to a <see cref="PagedResult{TEntity,TContext}"/> using a static one-line-call.
        /// </summary>
        /// <param name="request">The request for generating the result.</param>
        /// <param name="ctx">The entity context to use.</param>
        /// <param name="query">An optional query on the entities to use (if <c>null</c>, GetValidEntities is used).</param>
        /// <param name="filter">An optional filter expression.</param>
        /// <param name="util">Optionally the util to use for database-related operations.</param>
        /// <returns>The paged result.</returns>
        public static PagedResult<TEntity, TContext> GetResult(
            PagedRequest request,
            TContext ctx,
            IQueryable<TEntity> query = null,
            Expression<Func<TEntity, bool>> filter = null,
            BaseUtil<TEntity, TContext> util = null)
        {
            var result = new PagedResult<TEntity, TContext>(request, util);
            result.Generate(ctx, query, filter);
            return result;
        }

        /// <summary>
        /// Allows easy access to a <see cref="PagedResult{TEntity,TContext}"/> using a static one-line-call.
        /// </summary>
        /// <param name="request">The request for generating the result.</param>
        /// <param name="ctx">The entity context to use.</param>
        /// <param name="query">An optional query on the entities to use (if <c>null</c>, GetValidEntities is used).</param>
        /// <param name="filter">An optional filter expression.</param>
        /// <param name="util">Optionally the util to use for database-related operations.</param>
        /// <returns>The paged result.</returns>
        public static Task<PagedResult<TEntity, TContext>> GetResultAsync(
            PagedRequest request,
            TContext ctx,
            IQueryable<TEntity> query = null,
            Expression<Func<TEntity, bool>> filter = null,
            BaseUtil<TEntity, TContext> util = null)
        {
            var completionSource = new TaskCompletionSource<PagedResult<TEntity, TContext>>();
            Task.Run(
                async () =>
                {
                    try
                    {
                        var result = new PagedResult<TEntity, TContext>(request, util);
                        await result.GenerateAsync(ctx, query, filter).ConfigureAwait(false);
                        if (!completionSource.TrySetResult(result))
                        {
                            completionSource.TrySetException(new InvalidOperationException("Could not retrieve result."));
                        }
                    }
                    catch (Exception ex)
                    {
                        completionSource.TrySetException(ex);
                    }
                });
            return completionSource.Task;
        }

        /// <summary>
        /// Casts this instance into a simpler form suitable for callers.
        /// </summary>
        /// <returns>The base instance of this result.</returns>
        public BasePagedResult<TEntity> ToBasePagedResult()
        {
            return new BasePagedResult<TEntity>(Request)
            {
                Items = Items,
                CompleteItemsCount = CompleteItemsCount
            };
        }

        #endregion

        #region properties

        /// <summary>
        /// The util to use for database-related operations.
        /// </summary>
        private BaseUtil<TEntity, TContext> Util
        {
            get
            {
                return _util ?? (_util = Utils.Get<TEntity, TContext>());
            }
        }

        #endregion
    }
}
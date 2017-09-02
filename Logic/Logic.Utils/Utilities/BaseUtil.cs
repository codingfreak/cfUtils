namespace codingfreaks.cfUtils.Logic.Utils.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.ModelConfiguration.Configuration;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;

    using Extensions;

    using System.Linq;
    using System.Linq.Expressions;
    using System.Runtime.Caching;
    using System.Threading.Tasks;

    using Base.Delegates;
    using Base.EventArguments;
    using Base.Interfaces;
    using Base.Structures;
    using Base.Utilities;

    using Interfaces;

    using Standard.Extensions;
    using Standard.Utilities;

    using Structures;

    /// <summary>
    /// Abstract base class for all utils in this namespace.
    /// </summary>
    /// <typeparam name="TEntity">A type implementing <see cref="IEntity" />.</typeparam>
    /// <typeparam name="TContext">A type which derives from <see cref="DbContext" />.</typeparam>
    [SuppressMessage("Microsoft.Design", "CA1012:AbstractTypesShouldNotHaveConstructors", Justification = "The constructor has to be accessible for Ninject.")]
    public abstract class BaseUtil<TEntity, TContext>
        where TEntity : class, IEntity where TContext : DbContext
    {
        #region events

        #region event declarations

        /// <summary>
        /// Occurs when a doublette is found before creation of a new item.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event DoubletteFoundEventHandler<TEntity> DoubletteFound;

        #endregion

        #endregion

        #region constructors and destructors

        /// <summary>
        /// Default constructor for a util.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="contextResolver">The context resolver to use.</param>
        public BaseUtil(ILogger logger, IContextResolver contextResolver)
        {
            Logger = logger;
            ContextResolver = contextResolver;
        }

        #endregion

        #region methods

        /// <summary>
        /// Is called to check, if a given <paramref name="instance" /> is valid currently.
        /// </summary>
        /// <param name="instance">The instance to check.</param>
        /// <returns><c>true</c> if the <paramref name="instance" /> is valid, otherwise <c>false</c>.</returns>
        public async Task<bool> CheckInstance(TEntity instance)
        {
            return await Task.Run(async () => await CheckInstanceInternal(instance)).ConfigureAwait(false);
        }

        /// <summary>
        /// Stores a new instance of <typeparamref name="TEntity" /> in the database and takes care of historization etc.
        /// </summary>
        /// <param name="newItem">The item to add to the database.</param>
        /// <param name="ctx">The EF context or <c>null</c> if a fresh one should be used.</param>
        /// <param name="checkForDoublettes">Set this to <c>true</c> if doublette-check should be performed prior to creation.</param>
        /// <returns><c>True</c> if all steps succeeded, otherwise <c>false</c>.</returns>
        public virtual bool Create(TEntity newItem, TContext ctx = null, bool checkForDoublettes = false)
        {
            return ExecuteContextWrapped(
                c =>
                {
                    var ok = true;
                    if (checkForDoublettes && DoubletteFound != null)
                    {
                        var doublettes = GetValidEntities(c).Where(DoubletteFindExpression(newItem));
                        if (doublettes.Any())
                        {
                            // found doublettes
                            var e = new DoubletteFoundEventArgs<TEntity>(doublettes);
                            // fire the event
                            DoubletteFound(newItem, ref e);
                            // check for cancellation
                            ok = !e.Cancel;
                        }
                    }
                    if (ok)
                    {
                        // no doublettes where found, doublettes where found but caller wants to add anyways or doublette check is off
                        GetEntitiesSet(c).Add(newItem);
                        try
                        {
                            c.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            Logger.LogException("BU-EX-01", ex);
                            return false;
                        }
                    }
                    return true;
                },
                ctx);
        }

        /// <summary>
        /// Stores a new instance of <typeparamref name="TEntity" /> in the database and takes care of historization etc.
        /// </summary>
        /// <param name="newItem">The item to add to the database.</param>
        /// <param name="ctx">The EF context or <c>null</c> if a fresh one should be used.</param>
        /// <param name="checkForDoublettes">Set this to <c>true</c> if doublette-check should be performed prior to creation.</param>
        /// <returns><c>True</c> if all steps succeeded, otherwise <c>false</c>.</returns>
        public virtual async Task<bool> CreateAsync(TEntity newItem, TContext ctx = null, bool checkForDoublettes = false)
        {
            return await ExecuteContextWrappedAsync(
                async c =>
                {
                    var ok = true;
                    if (checkForDoublettes && DoubletteFound != null)
                    {
                        var doublettes = (await GetValidEntitiesAsync(c).ConfigureAwait(false)).Where(DoubletteFindExpression(newItem));
                        if (doublettes.Any())
                        {
                            // found doublettes
                            var e = new DoubletteFoundEventArgs<TEntity>(doublettes);
                            // fire the event
                            DoubletteFound(newItem, ref e);
                            // check for cancellation
                            ok = !e.Cancel;
                        }
                    }
                    if (ok)
                    {
                        // no doublettes where found, doublettes where found but caller wants to add anyways or doublette check is off
                        GetEntitiesSet(c).Add(newItem);
                        try
                        {
                            await c.SaveChangesAsync().ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            Logger.LogException("BU-EX-02", ex);
                            return false;
                        }
                    }
                    return true;
                },
                ctx).ConfigureAwait(false);
        }

        /// <summary>
        /// Stores the <paramref name="entity" /> in  the <paramref name="ctx" /> and decides, if INSERT or UPDATE should be used.
        /// </summary>
        /// <param name="entity">The entity to update in database.</param>
        /// <param name="ctx">The context or <c>null</c> if a new should be created.</param>
        /// <param name="checkForDoublettes">Set this to <c>true</c> if doublette-check should be performed prior to creation.</param>
        // <returns><c>true</c> if no exceptions occured otherwise <c>false</c>.</returns>
        public bool CreateOrUpdate(TEntity entity, TContext ctx = null, bool checkForDoublettes = false)
        {
            return ExecuteContextWrapped(c => c.Entry(entity).State == EntityState.Added ? Create(entity, c, checkForDoublettes) : Update(entity, c), ctx);
        }

        /// <summary>
        /// Stores the <paramref name="entity" /> in  the <paramref name="ctx" /> and decides, if INSERT or UPDATE should be used.
        /// </summary>
        /// <param name="entity">The entity to update in database.</param>
        /// <param name="ctx">The context or <c>null</c> if a new should be created.</param>
        /// <param name="checkForDoublettes">Set this to <c>true</c> if doublette-check should be performed prior to creation.</param>
        // <returns><c>true</c> if no exceptions occured otherwise <c>false</c>.</returns>
        public async Task<bool> CreateOrUpdateAsync(TEntity entity, TContext ctx = null, bool checkForDoublettes = false)
        {
            return await ExecuteContextWrappedAsync(async c => c.Entry(entity).State == EntityState.Added ? await CreateAsync(entity, c, checkForDoublettes) : await UpdateAsync(entity, c), ctx).ConfigureAwait(false);
        }

        /// <summary>
        /// Deletes a given <typeparamref name="TEntity"/> which can be context-less.
        /// </summary>
        /// <param name="id">The id entity to update in database.</param>
        /// <param name="deletionTimestampProperty"> The name of a property of <typeparamref name="TEntity"/> where deletion time should be stored, if existing.</param>
        /// <param name="ctx">The context or <c>null</c> if a new should be created.</param>
        /// <returns><c>true</c> if no exceptions occured otherwise <c>false</c>.</returns>
        public async Task<bool> DeleteAsync(long id, string deletionTimestampProperty = "DateDeleted", TContext ctx = null)
        {
            return await ExecuteContextWrappedAsync(
                async c =>
                {
                    var user = await LoadAsync(id, c).ConfigureAwait(false);
                    if (user == null)
                    {
                        return false;
                    }
                    return await DeleteAsync(user, deletionTimestampProperty, c).ConfigureAwait(false);
                }).ConfigureAwait(false);
        }

        /// <summary>
        /// Deletes a given <paramref name="entity" /> which can be context-less.
        /// </summary>
        /// <param name="entity">The entity to update in database.</param>
        /// <param name="deletionTimestampProperty">
        /// The name of a property of <paramref name="entity" /> where deletion time should
        /// be stored, if existing.
        /// </param>
        /// <param name="ctx">The context or <c>null</c> if a new should be created.</param>
        /// <returns><c>true</c> if no exceptions occured otherwise <c>false</c>.</returns>
        public bool Delete(TEntity entity, string deletionTimestampProperty = "DateDeleted", TContext ctx = null)
        {
            return ExecuteContextWrapped(
                c =>
                {                    
                    if (!deletionTimestampProperty.IsNullOrEmpty() && typeof(TEntity).HasProperty(deletionTimestampProperty, typeof(DateTime?), true))
                    {
                        typeof(TEntity).SetProperty<DateTime?>(entity, deletionTimestampProperty, DateTime.UtcNow);
                        c.Entry(entity).State = EntityState.Modified;
                    }
                    else
                    {
                        c.Entry(entity).State = EntityState.Deleted;
                    }
                    try
                    {
                        c.SaveChanges();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Logger.LogException("BU-EX-03", ex);
                        return false;
                    }
                },
                ctx);
        }

        /// <summary>
        /// Deletes a given <paramref name="entity" /> which can be context-less.
        /// </summary>
        /// <param name="entity">The entity to update in database.</param>
        /// <param name="deletionTimestampProperty">
        /// The name of a property of <paramref name="entity" /> where deletion time should
        /// be stored, if existing.
        /// </param>
        /// <param name="ctx">The context or <c>null</c> if a new should be created.</param>
        /// <returns><c>true</c> if no exceptions occured otherwise <c>false</c>.</returns>
        public async Task<bool> DeleteAsync(TEntity entity, string deletionTimestampProperty = "DateDeleted", TContext ctx = null)
        {
            return await ExecuteContextWrappedAsync(
                async c =>
                {                    
                    if (!deletionTimestampProperty.IsNullOrEmpty() && typeof(TEntity).HasProperty(deletionTimestampProperty, typeof(DateTime?), true))
                    {
                        typeof(TEntity).SetProperty<DateTime?>(entity, deletionTimestampProperty, DateTime.UtcNow);
                        c.Entry(entity).State = EntityState.Modified;
                    }
                    else
                    {
                        c.Entry(entity).State = EntityState.Deleted;
                    }
                    try
                    {
                        await c.SaveChangesAsync().ConfigureAwait(false);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Logger.LogException("BU-EX-04", ex);
                        return false;
                    }
                },
                ctx).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves a base paged result on the current <see cref="GetValidEntities" /> asynchronously.
        /// </summary>
        /// <param name="request">The paged request to use.</param>
        /// <param name="ctx">The context or <c>null</c> if a new should be created.</param>
        /// <param name="query">An optional query on the entities to use (if <c>null</c>, GetValidEntities is used).</param>
        /// <param name="filter">An optional filter expression.</param>
        /// <returns>The base paged result.</returns>
        public async Task<BasePagedResult<TEntity>> GetBasePagedResultAsync(PagedRequest request, TContext ctx = null, IQueryable<TEntity> query = null, Expression<Func<TEntity, bool>> filter = null)
        {
            return await ExecuteContextWrappedAsync(async c => await PagedResult<TEntity, TContext>.GetBaseResultAsync(request, c, query, filter, this), ctx).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves all currently valid entites from the cache or builds up a new cache if necessary.
        /// </summary>
        /// <param name="loadEager">
        /// If set to <c>true</c> all resulting items will be loaded including their collections and
        /// references.
        /// </param>
        /// <param name="ctx">The EF context or <c>null</c> if a fresh context should be used.</param>
        /// <param name="skipCache">In some cases caller wants to skip cache items easy.</param>
        /// <returns>The list of valid items.</returns>
        public IEnumerable<TEntity> GetCachedItems(bool loadEager = false, TContext ctx = null, bool skipCache = false)
        {
            return GetCacheItem(
                typeof(TEntity).Name,
                () => ExecuteContextWrapped(
                    c =>
                    {
                        var query = GetValidEntities(c);
                        try
                        {
                            if (loadEager)
                            {
                                // now load all collections and references
                                query = query.LoadEager();
                                //var typesToIgnore = new[] { typeof(string), typeof(DateTime), typeof(DateTime?) };
                                //var properties = typeof(TEntity).GetProperties().Where(p => !typesToIgnore.Contains(p.PropertyType)).ToList();
                                //query = properties.Where(property => property.PropertyType.IsClass || typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
                                //    .Aggregate(query, (current, property) => current.Include(property.Name).AsQueryable());
                            }
                            return query.ToList();
                        }
                        catch (Exception ex)
                        {
                            Trace.TraceError("Error in GetCachedItemsAsync: {0}", ex.Message);
                        }
                        return null;
                    },
                    ctx),
                skipCache);
        }

        /// <summary>
        /// Retrieves all currently valid entites from the cache or builds up a new cache if necessary.
        /// </summary>
        /// <param name="loadEager">
        /// If set to <c>true</c> all resulting items will be loaded including their collections and
        /// references.
        /// </param>
        /// <param name="ctx">The EF context or <c>null</c> if a fresh context should be used.</param>
        /// <param name="skipCache">In some cases caller wants to skip cache items easy.</param>
        /// <returns>The list of valid items.</returns>
        public async Task<IEnumerable<TEntity>> GetCachedItemsAsync(bool loadEager = false, TContext ctx = null, bool skipCache = false)
        {
            return await GetCacheItemAsync(
                typeof(TEntity).Name,
                async () => await ExecuteContextWrappedAsync(
                    async c =>
                    {
                        var query = GetValidEntities(c);
                        try
                        {
                            if (loadEager)
                            {
                                // now load all collections and references
                                query = query.LoadEager();
                                //var typesToIgnore = new[] { typeof(string), typeof(DateTime), typeof(DateTime?) };
                                //var properties = typeof(TEntity).GetProperties().Where(p => !typesToIgnore.Contains(p.PropertyType)).ToList();
                                //query = properties.Where(property => property.PropertyType.IsClass || typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
                                //    .Aggregate(query, (current, property) => current.Include(property.Name).AsQueryable());
                            }
                            return await query.ToListAsync().ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            Trace.TraceError("Error in GetCachedItemsAsync: {0}", ex.Message);
                        }
                        return null;
                    },
                    ctx),
                skipCache).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves the <see cref="DbSet" /> from the given <paramref name="ctx" /> which holds all items of the current
        /// <typeparamref name="TEntity" />.
        /// </summary>
        /// <param name="ctx">The EF context to use.</param>
        /// <returns>The entity set from the EF context.</returns>
        public DbSet<TEntity> GetEntitiesSet(TContext ctx)
        {
            CheckUtil.ThrowIfNull(() => ctx);
            return ctx.Set<TEntity>();
        }

        /// <summary>
        /// Retrieves a paged result on the current <see cref="GetValidEntities" />.
        /// </summary>
        /// <param name="request">The paged request to use.</param>
        /// <param name="ctx">The context or <c>null</c> if a new should be created.</param>
        /// <param name="query">An optional query on the entities to use (if <c>null</c>, GetValidEntities is used).</param>
        /// <param name="filter">An optional filter expression.</param>
        /// <returns>The paged result.</returns>
        public PagedResult<TEntity, TContext> GetPagedResult(PagedRequest request, TContext ctx = null, IQueryable<TEntity> query = null, Expression<Func<TEntity, bool>> filter = null)
        {
            return ExecuteContextWrapped(c => PagedResult<TEntity, TContext>.GetResult(request, c, query, filter, this), ctx);
        }

        /// <summary>
        /// Retrieves a paged result on the current <see cref="GetValidEntities" /> asynchronously.
        /// </summary>
        /// <param name="request">The paged request to use.</param>
        /// <param name="ctx">The context or <c>null</c> if a new should be created.</param>
        /// <param name="query">An optional query on the entities to use (if <c>null</c>, GetValidEntities is used).</param>
        /// <param name="filter">An optional filter expression.</param>
        /// <returns>The paged result.</returns>
        public async Task<PagedResult<TEntity, TContext>> GetPagedResultAsync(
            PagedRequest request,
            TContext ctx = null,
            IQueryable<TEntity> query = null,
            Expression<Func<TEntity, bool>> filter = null)
        {
            return await ExecuteContextWrappedAsync(async c => await PagedResult<TEntity, TContext>.GetResultAsync(request, c, query, filter, this).ConfigureAwait(false), ctx).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves a queryable for all entities of <typeparamref name="TEntity" /> which are currently (UTC) valid.
        /// </summary>
        /// <param name="ctx">The database context to use.</param>
        /// <param name="filterExpression">A LINQ expression to filter the elements before retrieving the query.</param>
        /// <returns>The queryable collection of valid entites.</returns>
        public IQueryable<TEntity> GetValidEntities(TContext ctx, Expression<Func<TEntity, bool>> filterExpression = null)
        {
            IQueryable<TEntity> result;
            result = filterExpression != null ? GetValidityQuery(ctx).Where(filterExpression) : GetValidityQuery(ctx);
            return result;
        }

        /// <summary>
        /// Retrieves a queryable for all entities of <typeparamref name="TEntity" /> which are currently (UTC) valid.
        /// </summary>
        /// <param name="ctx">The database context to use.</param>
        /// <param name="filterExpression">A LINQ expression to filter the elements before retrieving the query.</param>
        /// <returns>The queryable collection of valid entites.</returns>
        public async Task<IQueryable<TEntity>> GetValidEntitiesAsync(TContext ctx, Expression<Func<TEntity, bool>> filterExpression = null)
        {
            return await Task.Run(() => GetValidEntities(ctx, filterExpression));
        }

        /// <summary>
        /// Checks if the given <paramref name="id"/> exists in database without loading the entity itself.
        /// </summary>
        /// <param name="id">The database id to search for.</param>
        /// <param name="ctx">The EF context or <c>null</c> if a fresh one should be used.</param>
        /// <returns><c>true</c> if the <paramref name="id"/> was found otherwise <c>false</c>.</returns>
        public async Task<bool> IdExistsAsync(long id, TContext ctx = null)
        {
            return await ExecuteContextWrappedAsync(
                async c =>
                {
                    return await GetEntitiesSet(c).AnyAsync(e => e.Id == id).ConfigureAwait(false);
                },
                ctx).ConfigureAwait(false);
        }

        /// <summary>
        /// Loads an instance of <typeparamref name="TEntity" /> from database using it's <paramref name="id" />.
        /// </summary>
        /// <param name="id">The database id of the voucher.</param>
        /// <param name="ctx">The EF context to use.</param>
        /// <returns>The instance or <c>null</c> if no item was found.</returns>
        public TEntity Load(long id, TContext ctx = null)
        {
            return ExecuteContextWrapped(c => GetEntitiesSet(c).Find(id), ctx);
        }

        /// <summary>
        /// Loads an instance of <typeparamref name="TEntity" /> from database using it's <paramref name="id" /> based on
        /// a query that might contain includes etc.
        /// </summary>
        /// <remarks>
        /// This is slightly slower than loading with <paramref name="baseQuery" /> because here we have to
        /// depend on SingleOrDefault instead on Find.
        /// </remarks>
        /// <param name="id">The database id of the voucher.</param>
        /// <param name="baseQuery">The query to use against the db.</param>
        /// <param name="ctx">The EF context to use.</param>
        /// <returns>The instance or <c>null</c> if no item was found.</returns>
        public TEntity Load(long id, DbQuery<TEntity> baseQuery, TContext ctx = null)
        {
            return ExecuteContextWrapped(c => baseQuery.SingleOrDefault(e => e.Id == id), ctx);
        }

        /// <summary>
        /// Loads an instance of <typeparamref name="TEntity" /> from database using it's <paramref name="key" /> based on
        /// a query that might contain includes etc.
        /// </summary>
        /// <param name="key">The key-value to search for.</param>
        /// <param name="baseQuery">The query to use against the db.</param>
        /// <param name="ctx">The EF context to use.</param>
        /// <returns>The instance or <c>null</c> if no item was found.</returns>
        public TEntity Load(string key, DbQuery<TEntity> baseQuery, TContext ctx = null)
        {
            var expr = KeyFindExpression(key);
            if (expr == null)
            {
                throw new InvalidOperationException("Can not load entities of this type because no KeyFindExpression is defined.");
            }
            return ExecuteContextWrapped(c => baseQuery.SingleOrDefault(expr), ctx);
        }

        /// <summary>
        /// Loads an instance of <typeparamref name="TEntity" /> from database using a unique value defined as it's
        /// <paramref name="key" />.
        /// </summary>
        /// <remarks>
        /// In order to make this method work <see cref="KeyFindExpression" /> must be defined.
        /// </remarks>
        /// <param name="key">The key-value to search for.</param>
        /// <param name="ctx">The EF context to use.</param>
        /// <returns>The instance or <c>null</c> if no item was found.</returns>
        public TEntity Load(string key, TContext ctx = null)
        {
            CheckUtil.ThrowIfNullOrEmpty(() => key);
            var expr = KeyFindExpression(key);
            if (expr == null)
            {
                throw new InvalidOperationException("Can not load entities of this type because no KeyFindExpression is defined.");
            }
            return ExecuteContextWrapped(c => GetValidEntities(c).SingleOrDefault(expr), ctx);
        }

        /// <summary>
        /// Loads an instance of <typeparamref name="TEntity" /> from database using it's <paramref name="id" /> based on
        /// a query that might contain includes etc.
        /// </summary>
        /// <remarks>
        /// This is slightly slower than loading with <paramref name="baseQuery" /> because here we have to
        /// depend on SingleOrDefault instead on Find.
        /// </remarks>
        /// <param name="id">The database id of the voucher.</param>
        /// <param name="baseQuery">The query to use against the db.</param>
        /// <param name="ctx">The EF context to use.</param>
        /// <returns>The instance or <c>null</c> if no item was found.</returns>
        public async Task<TEntity> LoadAsync(long id, DbQuery<TEntity> baseQuery, TContext ctx = null)
        {
            return await ExecuteContextWrappedAsync(async c => await baseQuery.SingleOrDefaultAsync(e => e.Id == id).ConfigureAwait(false), ctx).ConfigureAwait(false);
        }

        /// <summary>
        /// Loads an instance of <typeparamref name="TEntity" /> from database using the <paramref name="baseQuery" /> based on
        /// a query that might contain includes etc.
        /// </summary>
        /// <param name="key">The key-value to search for.</param>
        /// <param name="baseQuery">The query to use against the db.</param>
        /// <param name="ctx">The EF context to use.</param>
        /// <returns>The instance or <c>null</c> if no item was found.</returns>
        public async Task<TEntity> LoadAsync(string key, DbQuery<TEntity> baseQuery, TContext ctx = null)
        {
            var expr = KeyFindExpression(key);
            if (expr == null)
            {
                throw new InvalidOperationException("Can not load entities of this type because no KeyFindExpression is defined.");
            }
            return await ExecuteContextWrappedAsync(async c => await baseQuery.SingleOrDefaultAsync(expr).ConfigureAwait(false), ctx).ConfigureAwait(false);
        }        

        /// <summary>
        /// Loads an instance of <typeparamref name="TEntity" /> from database using a unique value defined as it's
        /// <paramref name="key" />.
        /// </summary>
        /// <remarks>
        /// In order to make this method work <see cref="KeyFindExpression" /> must be defined.
        /// </remarks>
        /// <param name="key">The key-value to search for.</param>
        /// <param name="ctx">The EF context to use.</param>
        /// <returns>The instance or <c>null</c> if no item was found.</returns>
        public async Task<TEntity> LoadAsync(string key, TContext ctx = null)
        {
            CheckUtil.ThrowIfNullOrEmpty(() => key);
            var expr = KeyFindExpression(key);
            if (expr == null)
            {
                throw new InvalidOperationException("Can not load entities of this type because no KeyFindExpression is defined.");
            }
            return await ExecuteContextWrappedAsync(async c => await GetValidEntities(c).SingleOrDefaultAsync(expr).ConfigureAwait(false), ctx).ConfigureAwait(false);
        }

        /// <summary>
        /// Loads an instance of <typeparamref name="TEntity" /> from database using it's <paramref name="id" />.
        /// </summary>
        /// <param name="id">The database id of the voucher.</param>
        /// <param name="ctx">The EF context to use.</param>
        /// <returns>The instance or <c>null</c> if no item was found.</returns>
        public async Task<TEntity> LoadAsync(long id, TContext ctx = null)
        {
            return await Task.Run(async () => await ExecuteContextWrappedAsync(async c => await GetEntitiesSet(c).FindAsync(id).ConfigureAwait(false), ctx)).ConfigureAwait(false);
        }

        /// <summary>
        /// Updates a given <paramref name="entity" /> which can be context-less.
        /// </summary>
        /// <param name="entity">The entity to update in database.</param>
        /// <param name="ctx">The context or <c>null</c> if a new should be created.</param>
        /// <returns><c>true</c> if no exceptions occured otherwise <c>false</c>.</returns>
        public bool Update(TEntity entity, TContext ctx = null)
        {
            return ExecuteContextWrapped(
                c =>
                {
                    c.TryAttachEntity(entity);
                    c.Entry(entity).State = EntityState.Modified;
                    try
                    {
                        c.SaveChanges();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Logger.LogException("BU-EX-05", ex);
                        return false;
                    }
                },
                ctx);
        }

        /// <summary>
        /// Updates a given <paramref name="entity" /> which can be context-less.
        /// </summary>
        /// <param name="entity">The entity to update in database.</param>
        /// <param name="ctx">The context or <c>null</c> if a new should be created.</param>
        /// <returns><c>true</c> if no exceptions occured otherwise <c>false</c>.</returns>
        public async Task<bool> UpdateAsync(TEntity entity, TContext ctx = null)
        {
            return await ExecuteContextWrappedAsync(
                async c =>
                {
                    c.TryAttachEntity(entity);
                    c.Entry(entity).State = EntityState.Modified;
                    try
                    {
                        await c.SaveChangesAsync().ConfigureAwait(false);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Logger.LogException("BU-EX-06", ex);
                        return false;
                    }
                },
                ctx).ConfigureAwait(false);
        }

        /// <summary>
        /// Is called by the public <see cref="CheckInstance" /> to do the check-work.
        /// </summary>
        /// <param name="instance">The instance to check.</param>
        /// <returns><c>true</c> if the <paramref name="instance" /> is valid, otherwise <c>false</c>.</returns>
        protected virtual async Task<bool> CheckInstanceInternal(TEntity instance)
        {
            await Task.Yield();
            return false;
        }

        /// <summary>
        /// An expression to define a LINQ-statement that will search doublettes of <paramref name="compareObject" /> in the
        /// database.
        /// </summary>
        /// <param name="compareObject">The entity to compare with.</param>
        protected virtual Expression<Func<TEntity, bool>> DoubletteFindExpression(TEntity compareObject)
        {
            return item => item.Id == compareObject.Id;
        }

        /// <summary>
        /// Allows to execute a given <paramref name="action" /> within an ensured <paramref name="ctx" /> including
        /// disposing the <paramref name="ctx" /> if none was given.
        /// </summary>
        /// <param name="action">The action to execute including an EF context.</param>
        /// <param name="ctx">The context or <c>null</c> if a new should be created.</param>
        protected void ExecuteContextWrapped(Action<TContext> action, TContext ctx = null)
        {
            EntityContextUtil.ExecuteContextWrapped(action, ctx, ContextResolver, Logger);
        }

        /// <summary>
        /// Allows to execute a given <paramref name="func" /> within an ensured <paramref name="ctx" /> including
        /// disposing the <paramref name="ctx" /> if none was given.
        /// </summary>
        /// <typeparam name="TResult">The result type of the <paramref name="func" />.</typeparam>
        /// <param name="func">The function to execute including an EF context.</param>
        /// <param name="ctx">The context or <c>null</c> if a new should be created.</param>
        /// <returns>The result of the <paramref name="func" />.</returns>
        protected TResult ExecuteContextWrapped<TResult>(Func<TContext, TResult> func, TContext ctx = null)
        {
            return EntityContextUtil.ExecuteContextWrapped(func, ctx, ContextResolver, Logger);
        }

        /// <summary>
        /// Allows to execute a given <paramref name="func" /> within an ensured <paramref name="ctx" /> including
        /// disposing the <paramref name="ctx" /> if none was given.
        /// </summary>
        /// <param name="func">The function to execute including an EF context.</param>
        /// <param name="ctx">The context or <c>null</c> if a new should be created.</param>
        /// <returns>The awaitable task.</returns>
        protected async Task ExecuteContextWrappedAsync(Func<TContext, Task> func, TContext ctx = null)
        {
            if (func == null)
            {
                var ex = new ArgumentNullException("func");
                Logger.LogException("BU-EX-07", ex);
                throw ex;
            }
            var dispose = false;
            if (ctx == null)
            {
                ctx = ContextResolver.GetContext(typeof(TContext)) as TContext;
                if (ctx == null)
                {
                    throw new InvalidOperationException("Can not resolve database context.");
                }
                dispose = true;
            }
            try
            {
                await func(ctx).ConfigureAwait(false);
            }
            catch (AggregateException aex)
            {
                Logger.LogException("BU-EX-003", aex);
                if (aex.InnerExceptions.Count > 0)
                {
                    Logger.LogException("BU-EX-004", aex.InnerExceptions[0]);
                }
            }
            if (dispose)
            {
                ctx.Dispose();
            }
        }

        /// <summary>
        /// Allows to execute a given <paramref name="func" /> within an ensured <paramref name="ctx" /> including
        /// disposing the <paramref name="ctx" /> if none was given.
        /// </summary>
        /// <typeparam name="TResult">The result type of the <paramref name="func" />.</typeparam>
        /// <param name="func">The function to execute including an EF context.</param>
        /// <param name="ctx">The context or <c>null</c> if a new should be created.</param>
        /// <returns>The result of the <paramref name="func" />.</returns>
        protected async Task<TResult> ExecuteContextWrappedAsync<TResult>(Func<TContext, Task<TResult>> func, TContext ctx = null)
        {
            if (func == null)
            {
                var ex = new ArgumentNullException("func");
                Logger.LogException("BU-EX-08", ex);
                throw ex;
            }
            var dispose = false;
            if (ctx == null)
            {
                ctx = ContextResolver.GetContext(typeof(TContext)) as TContext;
                if (ctx == null)
                {
                    throw new InvalidOperationException("Can not resolve database context.");
                }
                dispose = true;
            }
            var result = default(TResult);
            try
            {
                result = await func(ctx).ConfigureAwait(false);
            }
            catch (AggregateException aex)
            {
                Logger.LogException("BU-EX-001", aex);
                if (aex.InnerExceptions.Count > 0)
                {
                    // Loses the proper stack trace. Oops. For workarounds, see 
                    // See http://bradwilson.typepad.com/blog/2008/04/small-decisions.html 
                    Logger.LogException("BU-EX-002", aex.InnerExceptions[0]);
                }
            }
            if (dispose)
            {
                ctx.Dispose();
            }
            return result;
        }

        /// <summary>
        /// Checks the cache for a given <paramref name="key" /> and retrieves it's value if found. Otherwise
        /// the <paramref name="creationFunc" /> will be used to instantiate a new <typeparamref name="T" /> and
        /// store it in the cache.
        /// </summary>
        /// <typeparam name="T">The type of the item which is expected to be returned from cache.</typeparam>
        /// <param name="key">The key inside the cache.</param>
        /// <param name="creationFunc">
        /// A function which will return an instance of <typeparamref name="T" /> in case no cache-item
        /// is found.
        /// </param>
        /// <param name="skipCache">In some cases caller wants to skip cache items easy.</param>
        /// <returns>The item from the cache or the one that was just created and stored in the cache.</returns>
        protected T GetCacheItem<T>(string key, Func<T> creationFunc, bool skipCache = false)
        {
            if (skipCache)
            {
                return creationFunc();
            }
            MemoryCache cache = null;
            try
            {
                cache = MemoryCache.Default;
            }
            catch (Exception ex)
            {
                TraceUtil.WriteTraceError(ex.Message);
            }
            if (cache != null && cache.Contains(key))
            {
                // cache hit
                Trace.TraceInformation("Cache hit on key {0}", key);
                return (T)cache.Get(key);
            }
            if (creationFunc == null)
            {
                throw new ArgumentNullException("creationFunc");
            }
            var result = creationFunc();
            if (result == null || cache == null)
            {
                return result;
            }
            var policy = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTime.Now.AddHours(ConfigurationUtil.Get<double>("MemoryCache.AbsoluteExpirationHours", 2))
            };
            cache.Add(key, result, policy);
            return result;
        }

        /// <summary>
        /// Checks the cache for a given <paramref name="key" /> and retrieves it's value if found. Otherwise
        /// the <paramref name="creationFunc" /> will be used to instantiate a new <typeparamref name="T" /> and
        /// store it in the cache asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of the item which is expected to be returned from cache.</typeparam>
        /// <param name="key">The key inside the cache.</param>
        /// <param name="creationFunc">
        /// An async function which will return an instance of <typeparamref name="T" /> in case no
        /// cache-item is found.
        /// </param>
        /// <param name="skipCache">In some cases caller wants to skip cache items easy.</param>
        /// <returns>The item from the cache or the one that was just created and stored in the cache.</returns>
        protected async Task<T> GetCacheItemAsync<T>(string key, Func<Task<T>> creationFunc, bool skipCache = false)
        {
            if (skipCache)
            {
                return await creationFunc().ConfigureAwait(false);
            }
            var cache = MemoryCache.Default;
            if (cache.Contains(key))
            {
                // cache hit
                Trace.TraceInformation("Cache hit on key {0}", key);
                return (T)cache.Get(key);
            }
            var result = await creationFunc().ConfigureAwait(false);
            // ReSharper disable once CompareNonConstrainedGenericWithNull
            if (result != null)
            {
                var policy = new CacheItemPolicy
                {
                    AbsoluteExpiration = DateTime.Now.AddHours(ConfigurationUtil.Get("MemoryCache.AbsoluteExpirationHours", 2))
                };
                cache.Add(key, result, policy);
            }
            return result;
        }

        /// <summary>
        /// Retrieves a queryable for all entities of <typeparamref name="TEntity" /> which are currently (UTC) valid.
        /// </summary>
        /// <remarks>
        /// This method is used by <see cref="GetValidEntities" />.
        /// </remarks>
        /// <param name="ctx">The database context to use.</param>
        /// <returns>The queryable collection of valid entites.</returns>
        protected virtual IQueryable<TEntity> GetValidityQuery(TContext ctx)
        {
            return GetEntitiesSet(ctx);
        }

        /// <summary>
        /// An expression to define a LINQ-statement that will find a single entity using the <paramref name="key" /> (not the
        /// id!).
        /// </summary>
        /// <param name="key">The key-value to search for.</param>
        protected virtual Expression<Func<TEntity, bool>> KeyFindExpression(string key)
        {
            return null;
        }

        /// <summary>
        /// Removes all entries from the cache where the key contains <paramref name="keyPart" />.
        /// </summary>
        /// <param name="keyPart">A string which should be searched in the keys of the cache.</param>
        protected void RemoveFromCache(string keyPart)
        {
            var cache = MemoryCache.Default;
            cache.Where(c => c.Key.Contains(keyPart)).Select(c => c.Key).ToList().ForEach(key => cache.Remove(key));
        }

        #endregion

        #region properties

        /// <summary>
        /// Indicates, if the <see cref="Logger" /> is set to an instance and thus logging is possible.
        /// </summary>
        public bool CanLog
        {
            get
            {
                return Logger != null;
            }
        }

        /// <summary>
        /// A type which knows how to get the correct DbContext for a given entity type.
        /// </summary>
        public IContextResolver ContextResolver { get; }

        /// <summary>
        /// The logger-instance for use in derived types.
        /// </summary>
        public ILogger Logger { get; }

        #endregion
    }
}
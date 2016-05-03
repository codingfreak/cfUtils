namespace codingfreaks.cfUtils.Logic.Utils.Extensions
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Core.Mapping;
    using System.Data.Entity.Core.Metadata.Edm;
    using System.Data.Entity.Core.Objects;
    using System.Data.Entity.Infrastructure;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;

    using codingfreaks.cfUtils.Logic.Base.Interfaces;
    using codingfreaks.cfUtils.Logic.Utils.Utilities;

    /// <summary>
    /// Extention methods for <see cref="IEntity" />.
    /// </summary>
    public static class EntityExtentions
    {
        #region methods

        /// <summary>
        /// Tries to create a shallow copy of <paramref name="entity" /> and attach it to the given <paramref name="newContext" />.
        /// </summary>
        /// <param name="entity">The entity object.</param>
        /// <param name="newContext">The ef context to attach the result to.</param>
        /// <returns><c>true</c> if operation succeeds, otherwise <c>false</c>.</returns>
        public static T CloneToContext<T>(this T entity, DbContext newContext) where T : class, IEntity
        {
            var oldDynamicSetting = newContext.Configuration.ProxyCreationEnabled;
            newContext.Configuration.ProxyCreationEnabled = false;
            var clone = newContext.Set<T>().Create();
            var propertiesOriginal = entity.GetType().GetProperties().Where(p => (p.CanWrite && (p.PropertyType.IsValueType || p.PropertyType == typeof(string)))).ToList();
            var propertiesClone = clone.GetType().GetProperties().Where(p => (p.CanWrite && (p.PropertyType.IsValueType || p.PropertyType == typeof(string)))).ToList();
            propertiesOriginal.ForEach(
                po =>
                {
                    var pc = propertiesClone.SingleOrDefault(p => p.Name.Equals(po.Name, StringComparison.Ordinal));
                    if (pc != null)
                    {
                        pc.SetValue(clone, po.GetValue(entity));
                    }
                });
            clone.Id = 0;
            newContext.Configuration.ProxyCreationEnabled = oldDynamicSetting;
            return clone;
        }

        /// <summary>
        /// Stores the <paramref name="entity" /> in  the <paramref name="ctx" /> and decides, if INSERT or UPDATE should be used.
        /// </summary>
        /// <param name="entity">The entity to update in database.</param>
        /// <param name="ctx">The context or <c>null</c> if a new should be created.</param>
        /// <param name="checkForDoublettes">Set this to <c>true</c> if doublette-check should be performed prior to creation.</param>
        // <returns><c>true</c> if no exceptions occured otherwise <c>false</c>.</returns>
        public static bool CreateOrUpdate<TEntity, TContext>(this TEntity entity, TContext ctx = null, bool checkForDoublettes = false) where TEntity : class, IEntity where TContext : DbContext
        {
            return Utils.Get<TEntity, TContext>().CreateOrUpdate(entity, ctx, checkForDoublettes);
        }

        /// <summary>
        /// Stores the <paramref name="entity" /> in  the <paramref name="ctx" /> and decides, if INSERT or UPDATE should be used.
        /// </summary>
        /// <param name="entity">The entity to update in database.</param>
        /// <param name="ctx">The context or <c>null</c> if a new should be created.</param>
        /// <param name="checkForDoublettes">Set this to <c>true</c> if doublette-check should be performed prior to creation.</param>
        // <returns><c>true</c> if no exceptions occured otherwise <c>false</c>.</returns>
        public static async Task<bool> CreateOrUpdateAsync<TEntity, TContext>(this TEntity entity, TContext ctx = null, bool checkForDoublettes = false) where TEntity : class, IEntity
            where TContext : DbContext
        {
            return await Utils.Get<TEntity, TContext>().CreateOrUpdateAsync(entity, ctx, checkForDoublettes).ConfigureAwait(false);
        }

        /// <summary>
        /// Deletes an existing <paramref name="entity" />. If the entity has a DateDeleted-property this will be set to the
        /// current timestamp.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <param name="ctx">The EF context or <c>null</c> if a fresh context should be used.</param>
        /// <returns><c>true</c> if the operaton succeeds, otherwise <c>false</c>.</returns>
        public static bool Delete<TEntity, TContext>(this TEntity entity, TContext ctx = null) where TEntity : class, IEntity where TContext : DbContext
        {
            return Utils.Get<TEntity, TContext>().Delete(entity, "DateDeleted", ctx);
        }

        /// <summary>
        /// Deletes an existing <paramref name="entity" />. If the entity has a DateDeleted-property this will be set to the
        /// current timestamp.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <param name="ctx">The EF context or <c>null</c> if a fresh context should be used.</param>
        /// <returns><c>true</c> if the operaton succeeds, otherwise <c>false</c>.</returns>
        public static async Task<bool> DeleteAsync<TEntity, TContext>(this TEntity entity, TContext ctx = null) where TEntity : class, IEntity where TContext : DbContext
        {
            return await Utils.Get<TEntity, TContext>().DeleteAsync(entity, "DateDeleted", ctx);
        }

        /// <summary>
        /// Retrieves the name of the database table of an <typeparamref name="TEntity" />.
        /// </summary>
        /// <remarks>
        /// Taken from http://romiller.com/2014/04/08/ef6-1-mapping-between-types-tables/.
        /// </remarks>
        /// <typeparam name="TEntity">The type of entity in the context.</typeparam>
        /// <typeparam name="TContext">The context type which is a <see cref="DbContext" />.</typeparam>
        /// <param name="entity">The entity to extend.</param>
        /// <param name="ctx">The context to work on.</param>
        /// <returns>The name of the database table or <c>null</c> if no name could be retrieved.</returns>
        public static string GetDatabaseTableName<TEntity, TContext>(this TEntity entity, TContext ctx = null) where TEntity : class, IEntity where TContext : DbContext
        {
            var type = typeof(TEntity);
            return type.GetDatabaseTableName(ctx);
        }

        /// <summary>
        /// Retrieves the name of the database table of an <paramref name="type" />.
        /// </summary>
        /// <typeparam name="TContext">The context type which is a <see cref="DbContext" />.</typeparam>
        /// <param name="type">The type of entity to extend.</param>
        /// <param name="ctx">The context to work on.</param>
        /// <returns>The name of the database table or <c>null</c> if no name could be retrieved.</returns>
        public static string GetDatabaseTableName<TContext>(this Type type, TContext ctx = null) where TContext : DbContext
        {
            var metadata = ((IObjectContextAdapter)ctx).ObjectContext.MetadataWorkspace;
            // Get the part of the model that contains info about the actual CLR types
            var objectItemCollection = ((ObjectItemCollection)metadata.GetItemCollection(DataSpace.OSpace));
            // Get the entity type from the model that maps to the CLR type
            var entityType = metadata.GetItems<EntityType>(DataSpace.OSpace).Single(e => objectItemCollection.GetClrType(e) == type);
            // Get the entity set that uses this entity type
            var entitySet = metadata.GetItems<EntityContainer>(DataSpace.CSpace).Single().EntitySets.Single(s => s.ElementType.Name == entityType.Name);
            // Find the mapping between conceptual and storage model for this entity set
            var mapping = metadata.GetItems<EntityContainerMapping>(DataSpace.CSSpace).Single().EntitySetMappings.Single(s => s.EntitySet == entitySet);
            // Find the storage entity set (table) that the entity is mapped
            var table = mapping.EntityTypeMappings.Single().Fragments.Single().StoreEntitySet;            
            // Return the table name from the storage entity set
            var tableName = (string)table.MetadataProperties["Table"].Value ?? table.Name;
            var schemaName = (string)table.MetadataProperties["Schema"].Value ?? entitySet.Schema;
            return string.Format(CultureInfo.InvariantCulture, "{0}.{1}", schemaName, tableName);
        }

        /// <summary>
        /// Touches all reference-type properties of an entity so that they are loaded.
        /// </summary>
        /// <remarks>
        /// Call this inside the EF context which loaded the entity initially!
        /// </remarks>
        /// <typeparam name="T">The type of the entitity to touch.</typeparam>
        /// <param name="entity">The entity object.</param>
        /// <param name="onlyEntityTypes">
        /// Indicates if only properties containing types implementing <see cref="IEntity" /> should
        /// be used.
        /// </param>
        /// <param name="propertyNames">A list of property names this mehod should search for to touch.</param>
        /// <param name="ignorePropertyNames">A list of property names this mehod should NOT touch.</param>
        public static void LoadEager<T>(this T entity, bool onlyEntityTypes = false, string[] propertyNames = null, string[] ignorePropertyNames = null) where T : IEntity
        {
            var typesToIgnore = new[] { typeof(string), typeof(DateTime), typeof(DateTime?) };
            var properties = typeof(T).GetProperties().Where(p => !typesToIgnore.Contains(p.PropertyType)).ToList();
            if (propertyNames != null && propertyNames.Any())
            {
                properties = properties.Where(p => propertyNames.Contains(p.Name)).ToList();
            }
            if (ignorePropertyNames != null && ignorePropertyNames.Any())
            {
                properties = properties.Where(p => !ignorePropertyNames.Contains(p.Name)).ToList();
            }
            var propertiesToTouch =
                properties.Where(
                    property =>
                        (!onlyEntityTypes || typeof(IEntity).IsAssignableFrom(property.PropertyType))
                        || typeof(IEnumerable<IEntity>).IsAssignableFrom(property.PropertyType) && (property.PropertyType.IsClass || typeof(IEnumerable).IsAssignableFrom(property.PropertyType)))
                    .ToList();
            propertiesToTouch.ForEach(
                p =>
                {
                    Trace.TraceInformation("Eager loading property {0}", p.Name);
                    try
                    {
                        var x = p.GetValue(entity);
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceError("Error on eager loading property {0}: {1}", p.Name, ex.Message);
                    }
                });
        }

        /// <summary>
        /// Reloads a single <paramref name="entity" /> from  the <paramref name="ctx" />.
        /// </summary>
        /// <typeparam name="TEntity">The entity type</typeparam>
        /// <typeparam name="TContext">The type of the database context.</typeparam>
        /// <param name="entity">The entity to perform the operation for.</param>
        /// <param name="ctx">
        /// The context to do the operation on. Mandatory, as creating a new context would not make sense in this
        /// scenario.
        /// </param>
        public static void Reload<TEntity, TContext>(this TEntity entity, TContext ctx) where TEntity : class, IEntity where TContext : DbContext
        {
            var objectContext = ctx.ToObjectContext();
            objectContext.Refresh(RefreshMode.StoreWins, entity);
        }

        /// <summary>
        /// Reloads the contents of a navigation property of an entity that already has been loaded from the database. Useful to
        /// just reload the references without reloading the whole entity object.
        /// </summary>
        /// <typeparam name="TEntityToReload">The entity type to reload from database.</typeparam>
        /// <typeparam name="TEntity">The entity type</typeparam>
        /// <typeparam name="TContext">The type of the database context.</typeparam>
        /// <param name="entity">The entity to perform the operation for.</param>
        /// <param name="ctx">
        /// The context to do the operation on. Mandatory, as creating a new context would not make sense in this
        /// scenario.
        /// </param>
        public static void ReloadNavigationPropertyFromDatabase<TEntity, TEntityToReload, TContext>(this TEntity entity, TContext ctx) where TEntity : class, IEntity where TEntityToReload : class
            where TContext : DbContext
        {
            // From "Entity Framework 6 Recipes 2nd Edition, Chapter 5 - Loading entities and navigation properties, Page 172:
            // Currently, there isn't an easy way to refresh entities with the DbContext API.
            // Instead, drop down into the ObjectContext and perform the following actions
            var objectContext = ctx.ToObjectContext();
            var objectSet = objectContext.CreateObjectSet<TEntityToReload>();
            objectSet.MergeOption = MergeOption.OverwriteChanges;
            objectSet.Load();
        }

        /// <summary>
        /// Removes all entries from a set of entries.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity in the context.</typeparam>
        /// <typeparam name="TContext">The context type which is a <see cref="DbContext" />.</typeparam>
        /// <param name="entities">The set of entities to extend.</param>
        /// <param name="ctx">The context to work on.</param>
        /// <returns>The amount of entries removed, -1 if no table could be retrieved and -2 if an error occured.</returns>
        public static int RemoveAllEntries<TEntity, TContext>(this DbSet<TEntity> entities, TContext ctx = null) where TEntity : class, IEntity where TContext : DbContext
        {
            var tableName = (typeof(TEntity)).GetDatabaseTableName(ctx);
            if (string.IsNullOrEmpty(tableName))
            {
                return -1;
            }
            var sql = string.Format(CultureInfo.InvariantCulture, "DELETE FROM {0};", tableName);
            try
            {
                return ctx.Database.ExecuteSqlCommand(sql);
            }
            catch (Exception)
            {
                return -2;
            }
        }

        /// <summary>
        /// Removes all entries from a set of entries.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity in the context.</typeparam>
        /// <typeparam name="TContext">The context type which is a <see cref="DbContext" />.</typeparam>
        /// <param name="entities">The set of entities to extend.</param>
        /// <param name="ctx">The context to work on.</param>
        /// <returns>The amount of entries removed, -1 if no table could be retrieved and -2 if an error occured.</returns>
        public static async Task<int> RemoveAllEntriesAsync<TEntity, TContext>(this DbSet<TEntity> entities, TContext ctx = null) where TEntity : class, IEntity where TContext : DbContext
        {
            var tableName = (typeof(TEntity)).GetDatabaseTableName(ctx);
            if (string.IsNullOrEmpty(tableName))
            {
                return -1;
            }
            var sql = string.Format(CultureInfo.InvariantCulture, "DELETE FROM {0};", tableName);
            try
            {
                return await ctx.Database.ExecuteSqlCommandAsync(sql).ConfigureAwait(false);
            }
            catch (Exception)
            {
                return -2;
            }
        }

        /// <summary>
        /// Updates an existing <paramref name="entity" /> even if it has lost it's context to the database.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <param name="ctx">The EF context or <c>null</c> if a fresh context should be used.</param>
        /// <returns><c>true</c> if the operaton succeeds, otherwise <c>false</c>.</returns>
        public static bool Update<TEntity, TContext>(this TEntity entity, TContext ctx = null) where TEntity : class, IEntity where TContext : DbContext
        {
            return Utils.Get<TEntity, TContext>().Update(entity, ctx);
        }

        /// <summary>
        /// Updates an existing <paramref name="entity" /> asynchronously even if it has lost it's context to the database.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <param name="ctx">The EF context or <c>null</c> if a fresh context should be used.</param>
        /// <returns><c>true</c> if the operaton succeeds, otherwise <c>false</c>.</returns>
        public static async Task<bool> UpdateAsync<TEntity, TContext>(this TEntity entity, TContext ctx = null) where TEntity : class, IEntity where TContext : DbContext
        {
            return await Utils.Get<TEntity, TContext>().UpdateAsync(entity, ctx).ConfigureAwait(false);
        }

        #endregion
    }
}
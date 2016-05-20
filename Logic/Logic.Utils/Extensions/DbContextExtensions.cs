namespace codingfreaks.cfUtils.Logic.Utils.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Core.Metadata.Edm;
    using System.Data.Entity.Core.Objects;
    using System.Data.Entity.Infrastructure;
    using System.Linq;

    using codingfreaks.cfUtils.Logic.Base.Interfaces;
    using codingfreaks.cfUtils.Logic.Base.Utilities;

    /// <summary>
    /// Extends any <see cref="DbContext" />.
    /// </summary>
    public static class DbContextExtensions
    {
        #region methods

        /// <summary>
        /// Retrieves a dictionary containing the name of each table in a DbContext as the key
        /// and the amount of entries as the value.
        /// </summary>        
        /// <param name="ctx">The context to extend.</param>
        /// <param name="isSqlServer"><c>true</c> if the context is targetting SQL Server.</param>
        /// <returns>A dictionary containing all table names and their amount of rows or <c>null</c> if query fails.</returns>
        public static IDictionary<string, int?> GetTableFillGrade(this DbContext ctx, bool isSqlServer = true)
        {
            var result = new Dictionary<string, int?>();
            ctx.GetTableNames(isSqlServer).ToList().ForEach(
                table =>
                {
                    int? amount = null;
                    try
                    {
                        var query = ctx.Database.SqlQuery<int>($"SELECT COUNT(*) FROM {table};");
                        amount = query.FirstAsync().Result;
                    }
                    catch (Exception ex)
                    {
                        TraceUtil.WriteTraceError(ex.Message);
                    }
                    result.Add(table, amount);
                });
            return result;
        }

        /// <summary>
        /// Retrieves a list of all table names (including schemas) of the <paramref name="ctx"/>.
        /// </summary>
        /// <param name="ctx">The context to extend.</param>
        /// <param name="isSqlServer"><c>true</c> if the context is targetting SQL Server or SQL Azure so that names
        /// will be surrounded by [] instead of ''.</param>
        /// <returns>The list of table names.</returns>
        public static IEnumerable<string> GetTableNames(this DbContext ctx, bool isSqlServer = true)
        {
            var metadata = ctx.ToObjectContext().MetadataWorkspace;
            var opener = isSqlServer ? "[" : "'";
            var closer = isSqlServer ? "]" : "'";
            return
                metadata.GetItemCollection(DataSpace.SSpace)
                    .GetItems<EntityContainer>()
                    .Single()
                    .BaseEntitySets.OfType<EntitySet>()
                    .Where(s => !s.MetadataProperties.Contains("Type") || s.MetadataProperties["Type"].ToString() == "Tables")
                    .Select(
                        table =>
                        {
                            var tableName = table.MetadataProperties.Contains("Table") && table.MetadataProperties["Table"].Value != null
                                ? table.MetadataProperties["Table"].Value.ToString()
                                : table.Name;
                            var tableSchema = table.MetadataProperties["Schema"].Value?.ToString();
                            return string.IsNullOrEmpty(tableSchema) ? null : $"{opener}{tableSchema}{closer}.{opener}{tableName}{closer}";
                        }).Where(e => e != null).ToList();
        }

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
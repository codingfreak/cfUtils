using System;
using System.Linq;

namespace codingfreaks.cfUtils.Logic.Utils.Misc
{
    using System.Data.Entity;

    using Base.Utilities;

    /// <summary>
    /// A reusable database configuration for SQL Azure
    /// </summary>
    public class SqlAzureDatabaseConfiguration : DbConfiguration
    {
        #region constructors and destructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SqlAzureDatabaseConfiguration()
        {
            TraceUtil.WriteTraceDebug("Using custom database configuration.");
            SetExecutionStrategy("System.Data.SqlClient", () => new CustomExecutionStrategy(100, TimeSpan.FromSeconds(3)));
            SetDatabaseLogFormatter((ctx, logAction) => new OneLineDatabaseLogFormatter(ctx, logAction));
        }

        #endregion
    }
}
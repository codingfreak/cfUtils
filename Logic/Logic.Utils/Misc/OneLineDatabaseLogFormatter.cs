using System;
using System.Linq;

namespace s2.s2Utils.Logic.Utils.Misc
{
    using System.Data.Common;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure.Interception;
    using System.Globalization;

    /// <summary>
    /// Can be configured inside a <see cref="DbConfiguration"/> to tell EF to log events in one line.
    /// </summary>
    public class OneLineDatabaseLogFormatter : DatabaseLogFormatter
    {
        #region member vars

        private readonly CultureInfo _cultureInfoToUse = CultureInfo.InvariantCulture;

        #endregion

        #region constructors and destructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="context">The context passed by EF automatically.</param>
        /// <param name="writeAction">The action to perform when the result is written.</param>
        public OneLineDatabaseLogFormatter(DbContext context, Action<string> writeAction) : base(context, writeAction)
        {
        }

        #endregion

        #region methods

        /// <summary>
        /// Is called when the <paramref name="connection"/> switches to the closed state.
        /// </summary>
        /// <param name="connection">The database connection.</param>
        /// <param name="interceptionContext">The interception context.</param>
        public override void Closed(DbConnection connection, DbConnectionInterceptionContext interceptionContext)
        {
            Write(string.Format(_cultureInfoToUse, "Closed db connection at {0}.", DateTime.UtcNow));
        }

        /// <summary>
        /// Is called when the <paramref name="command"/> is executing.
        /// </summary>
        /// <param name="command">The database command.</param>
        /// <param name="interceptionContext">The interception context.</param>
        public override void LogCommand<TResult>(DbCommand command, DbCommandInterceptionContext<TResult> interceptionContext)
        {
            var commandText = command.CommandText.Replace(Environment.NewLine, "");
            Write(string.Format(_cultureInfoToUse, "Context '{0}' is executing command '{1}'", Context.GetType().Name, commandText));
        }

        /// <summary>
        /// Is called when the <paramref name="command"/> was executed.
        /// </summary>
        /// <param name="command">The database command.</param>
        /// <param name="interceptionContext">The interception context.</param>
        public override void LogResult<TResult>(DbCommand command, DbCommandInterceptionContext<TResult> interceptionContext)
        {
            Write(string.Format(_cultureInfoToUse, "Result of {0} operation: {1}", interceptionContext.IsAsync ? "ASYNC" : "SYNC", interceptionContext.Result));
        }

        /// <summary>
        /// Is called when the <paramref name="connection"/> switches to the open state.
        /// </summary>
        /// <param name="connection">The database connection.</param>
        /// <param name="interceptionContext">The interception context.</param>
        public override void Opened(DbConnection connection, DbConnectionInterceptionContext interceptionContext)
        {
            Write(string.Format(_cultureInfoToUse, "Opened db connection at {0}.", DateTime.UtcNow));
        }

        #endregion
    }
}
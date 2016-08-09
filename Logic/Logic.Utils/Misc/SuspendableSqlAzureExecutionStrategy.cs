namespace codingfreaks.cfUtils.Logic.Utils.Misc
{
    using System;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.SqlServer;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Runtime.Remoting.Messaging;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// http://stackoverflow.com/questions/31899382/how-to-use-sqlazureexecutionstrategy-and-nolock/32102380#32102380
    /// </summary>
    public class SuspendableSqlAzureExecutionStrategy : IDbExecutionStrategy
    {
        #region member vars

        private readonly IDbExecutionStrategy _azureExecutionStrategy;

        #endregion

        #region constructors and destructors

        public SuspendableSqlAzureExecutionStrategy()
        {
            _azureExecutionStrategy = new SqlAzureExecutionStrategy();
        }

        #endregion

        #region explicit interfaces

        public virtual void Execute(Action operation)
        {
            if (!RetriesOnFailure)
            {
                operation();
                return;
            }

            try
            {
                Suspend = true;
                _azureExecutionStrategy.Execute(operation);
            }
            finally
            {
                Suspend = false;
            }
        }

        public virtual TResult Execute<TResult>(Func<TResult> operation)
        {
            if (!RetriesOnFailure)
            {
                return operation();
            }

            try
            {
                Suspend = true;
                return _azureExecutionStrategy.Execute(operation);
            }
            finally
            {
                Suspend = false;
            }
        }

        public virtual async Task ExecuteAsync(Func<Task> operation, CancellationToken cancellationToken)
        {
            if (!RetriesOnFailure)
            {
                await operation();
                return;
            }

            try
            {
                Suspend = true;
                await _azureExecutionStrategy.ExecuteAsync(operation, cancellationToken);
            }
            finally
            {
                Suspend = false;
            }
        }

        public virtual async Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> operation, CancellationToken cancellationToken)
        {
            if (!RetriesOnFailure)
            {
                return await operation();
            }

            try
            {
                Suspend = true;
                return await _azureExecutionStrategy.ExecuteAsync(operation, cancellationToken);
            }
            finally
            {
                Suspend = false;
            }
        }

        public bool RetriesOnFailure
        {
            get
            {
                return !Suspend;
            }
        }

        protected virtual bool ShouldRetryOn(Exception exception)
        {
            var shouldRetry = false;
            var sqlException = exception as SqlException;
            if (sqlException == null)
            {
                return false;
            }
            SqlConnection.ClearAllPools();
            foreach (SqlError error in sqlException.Errors)
            {
                if (error.Number == -2 || error.Number == 19)
                {
                    shouldRetry = true;
                }
            }
            return shouldRetry;
        }

        #endregion

        #region properties

        private static bool Suspend
        {
            get
            {
                return (bool?)CallContext.LogicalGetData("SuspendExecutionStrategy") ?? false;
            }
            set
            {
                CallContext.LogicalSetData("SuspendExecutionStrategy", value);
            }
        }

        #endregion
    }
}
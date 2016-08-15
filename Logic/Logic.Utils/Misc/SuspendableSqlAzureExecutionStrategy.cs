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
    /// An execution strategy for scenarios in which transactional handling is important.
    /// </summary>
    /// <remarks>
    /// This code is mainly taken from suggestions at
    /// http://stackoverflow.com/questions/31899382/how-to-use-sqlazureexecutionstrategy-and-nolock/32102380#32102380
    /// </remarks>
    public class SuspendableSqlAzureExecutionStrategy : IDbExecutionStrategy
    {
        #region member vars

        private readonly IDbExecutionStrategy _azureExecutionStrategy;

        #endregion

        #region constructors and destructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SuspendableSqlAzureExecutionStrategy()
        {
            _azureExecutionStrategy = new SqlAzureExecutionStrategy();
        }

        #endregion

        #region explicit interfaces

        /// <summary>
        /// Executes the specified operation.
        /// </summary>
        /// <param name="operation">A delegate representing an executable operation that doesn't return any results.</param>
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

        /// <summary>
        /// Executes the specified operation and returns the result.
        /// </summary>
        /// <typeparam name="TResult"> The return type of <paramref name="operation" />. </typeparam>
        /// <param name="operation">
        /// A delegate representing an executable operation that returns the result of type
        /// <typeparamref name="TResult" />.
        /// </param>
        /// <returns>The result from the operation.</returns>
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

        /// <summary>
        /// Executes the specified asynchronous operation.
        /// </summary>
        /// <param name="operation">A function that returns a started task.</param>
        /// <param name="cancellationToken">
        /// A cancellation token used to cancel the retry operation, but not operations that are already in flight
        /// or that already completed successfully.
        /// </param>
        /// <returns>
        /// A task that will run to completion if the original task completes successfully (either the
        /// first time or after retrying transient failures). If the task fails with a non-transient error or
        /// the retry limit is reached, the returned task will become faulted and the exception must be observed.
        /// </returns>
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

        /// <summary>
        /// Executes the specified asynchronous operation and returns the result.
        /// </summary>
        /// <typeparam name="TResult">
        /// The result type of the <see cref="T:System.Threading.Tasks.Task`1" /> returned by
        /// <paramref name="operation" />.
        /// </typeparam>
        /// <param name="operation">A function that returns a started task of type <typeparamref name="TResult" />. </param>
        /// <param name="cancellationToken">
        /// A cancellation token used to cancel the retry operation, but not operations that are already in flight
        /// or that already completed successfully.
        /// </param>
        /// <returns>
        /// A task that will run to completion if the original task completes successfully (either the
        /// first time or after retrying transient failures). If the task fails with a non-transient error or
        /// the retry limit is reached, the returned task will become faulted and the exception must be observed.
        /// </returns>
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

        /// <summary>
        /// Indicates whether this <see cref="T:System.Data.Entity.Infrastructure.IDbExecutionStrategy" /> might retry the
        /// execution after a failure.
        /// </summary>
        public bool RetriesOnFailure => !Suspend;

        #endregion

        #region methods

        /// <summary>
        /// Defines the retry behavior of this strategy.
        /// </summary>
        /// <param name="exception">Any exception that occured on the context.</param>
        /// <returns><c>true</c> if a retry should by performed otherwise <c>false</c>.</returns>
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

        /// <summary>
        /// Gets or sets the strategy name from/in the current call context.
        /// </summary>
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
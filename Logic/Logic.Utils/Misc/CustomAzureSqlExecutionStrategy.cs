using System;
using System.Linq;

namespace codingfreaks.cfUtils.Logic.Utils.Misc
{
    using System.Data.Entity.SqlServer;
    using System.Data.SqlClient;
    using System.Diagnostics;

    /// <summary>
    /// A custom strategy for failures on transient SQL Azure database connections.
    /// </summary>
    /// <remarks>
    /// Taken from http://ritzlgrmft.blogspot.de/2014/03/working-with-sqlazureexecutionstrategy.html
    /// </remarks>
    public class CustomExecutionStrategy : SqlAzureExecutionStrategy
    {
        #region constructors and destructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public CustomExecutionStrategy() : base(10, TimeSpan.FromMinutes(10))
        {            
        }

        /// <summary>
        /// Constructor allowing definition of main properties.
        /// </summary>
        /// <param name="maxRetryCount">The maximum retry count on failures.</param>
        /// <param name="maxDelay">The delay to wait between retries.</param>
        public CustomExecutionStrategy(int maxRetryCount, TimeSpan maxDelay) : base(maxRetryCount, maxDelay)
        {
        }

        #endregion

        #region methods

        /// <inheritdoc/>
        protected override bool ShouldRetryOn(Exception exception)
        {
            var shouldRetry = false;
            var sqlException = exception as SqlException;
            if (sqlException != null)
            {
                SqlConnection.ClearAllPools();
                foreach (SqlError error in sqlException.Errors)
                {
                    if (error.Number == -2 || error.Number == 19)
                    {                        
                        shouldRetry = true;
                    }
                }
            }            
            shouldRetry = shouldRetry || base.ShouldRetryOn(exception);                 
            return shouldRetry;
        }

        //public int MaxRetryCount { get; set; }

        //public TimeSpan MaxDelay { get; set; }

        #endregion
    }
}
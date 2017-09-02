namespace codingfreaks.cfUtils.Logic.Base.Utilities
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Threading.Tasks;

    using Standard.Utilities;

    /// <summary>
    /// Contains logic for accessing relational databases.
    /// </summary>
    public static class DatabaseUtils
    {
        #region properties

        /// <summary>
        /// A callback method to call when exceptions occur.
        /// </summary>
        public static Action<Exception> ExceptionCallback { get; set; }

        #endregion

        #region methods

        /// <summary>
        /// Executes a given <paramref name="commandText" /> on the <paramref name="connection" />.
        /// </summary>
        /// <param name="commandText">The command text to execute.</param>
        /// <param name="connection">The connection to use for the query.</param>
        /// <param name="commandTimeout">The command timeout in seconds.</param>
        /// <returns>The amount of affected rows or -1 if an error occured.</returns>
        /// <exception cref="ArgumentException">
        /// Is thrown if the <paramref name="commandText" /> or <paramref name="connection" /> are invalid.
        /// </exception>
        [Obsolete("This implementation should not be used anymore because it uses deprecated calls.")]
        public static int ExecuteCommand(string commandText, IDbConnection connection, int commandTimeout = 30)
        {
            return AsyncUtil.CallSync(ExecuteCommandAsync, commandText, connection, commandTimeout);
        }

        /// <summary>
        /// Executes a given <paramref name="commandText" /> on the <paramref name="connection" />.
        /// </summary>
        /// <param name="commandText">The command text to execute.</param>
        /// <param name="connection">The connection to use for the query.</param>
        /// <param name="commandTimeout">The command timeout in seconds.</param>
        /// <returns>The amount of affected rows or -1 if an error occured.</returns>
        /// <exception cref="ArgumentException">
        /// Is thrown if the <paramref name="commandText" /> or <paramref name="connection" /> are invalid.
        /// </exception>
        public static async Task<int> ExecuteCommandAsync(string commandText, IDbConnection connection, int commandTimeout = 30)
        {
            CheckUtil.ThrowIfNull(() => connection);
            CheckUtil.ThrowIfNullOrEmpty(() => commandText);
            try
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandTimeout = commandTimeout;
                    var commandToUse = command as DbCommand;
                    CheckUtil.ThrowIfNull(() => commandToUse);
                    // ReSharper disable once PossibleNullReferenceException
                    commandToUse.CommandText = commandText;
                    return await commandToUse.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                ExceptionCallback?.Invoke(ex);
                return -1;
            }
        }

        /// <summary>
        /// Retrieves an opened connection to a SQL server.
        /// </summary>
        /// <param name="connectionStringOrKey">The connection string or the key to a connection-string-setting in the config.</param>
        /// <param name="providerName">
        /// The name of the provider. (Leave this null, if you set
        /// <paramref name="connectionStringIsConfigKey" /> to <c>true</c>.
        /// </param>
        /// <param name="connectionStringIsConfigKey">
        /// Indicates whether  <paramref name="connectionStringOrKey" /> should be
        /// interpreted as a config-key.
        /// </param>
        /// <returns>The opened connection of <c>null</c> if an error occured.</returns>
        /// <exception cref="ArgumentException">
        /// Is thrown if the <paramref name="connectionStringOrKey" /> is empty or if no key
        /// was found in the config.
        /// </exception>
        [Obsolete("This implementation should not be used anymore because it uses deprecated calls.")]
        public static IDbConnection GetConnection(string connectionStringOrKey, bool connectionStringIsConfigKey = false, string providerName = null)
        {
            return AsyncUtil.CallSync(GetConnectionAsync, connectionStringOrKey, connectionStringIsConfigKey, providerName);
        }

        /// <summary>
        /// Retrieves an opened connection to a SQL server.
        /// </summary>
        /// <param name="connectionStringOrKey">The connection string or the key to a connection-string-setting in the config.</param>
        /// <param name="providerName">
        /// The name of the provider. (Leave this null, if you set
        /// <paramref name="connectionStringIsConfigKey" /> to <c>true</c>.
        /// </param>
        /// <param name="connectionStringIsConfigKey">
        /// Indicates whether  <paramref name="connectionStringOrKey" /> should be
        /// interpreted as a config-key.
        /// </param>
        /// <returns>The opened connection of <c>null</c> if an error occured.</returns>
        /// <exception cref="ArgumentException">
        /// Is thrown if the <paramref name="connectionStringOrKey" /> is empty or if no key
        /// was found in the config.
        /// </exception>
        public static async Task<IDbConnection> GetConnectionAsync(string connectionStringOrKey, bool connectionStringIsConfigKey = false, string providerName = null)
        {
            CheckUtil.ThrowIfNullOrEmpty(() => connectionStringOrKey);
            var connectionString = connectionStringOrKey;
            if (connectionStringIsConfigKey)
            {
                // try to get connection string from config
                connectionString = ConfigurationUtil.GetConnectionString(connectionStringOrKey);
                providerName = ConfigurationUtil.GetProviderName(connectionStringOrKey);
            }
            CheckUtil.ThrowIfNullOrEmpty(() => connectionString);
            CheckUtil.ThrowIfNullOrEmpty(() => providerName);
            // connection-string is valid            
            // ReSharper disable once AssignNullToNotNullAttribute
            var factory = DbProviderFactories.GetFactory(providerName);
            var connection = factory.CreateConnection();
            if (connection == null)
            {
                return null;
            }
            connection.ConnectionString = connectionString;
            try
            {
                await connection.OpenAsync().ConfigureAwait(false);
                return connection;
            }
            catch (Exception ex)
            {
                ExceptionCallback?.Invoke(ex);
                return null;
            }
        }

        /// <summary>
        /// Retrieves a reader for a given <paramref name="commandText" /> on the <paramref name="connection" />.
        /// </summary>
        /// <param name="commandText">The command text to execute.</param>
        /// <param name="connection">The connection to use for the query.</param>
        /// <param name="behavior">The command beavior for the reader.</param>
        /// <param name="commandTimeout">The command timeout in seconds.</param>
        /// <returns>The reader or <c>null</c> if an error occurs.</returns>
        /// <exception cref="ArgumentException">
        /// Is thrown if the <paramref name="commandText" /> or <paramref name="connection" /> are invalid.
        /// </exception>
        [Obsolete("This implementation should not be used anymore because it uses deprecated calls.")]
        public static IDataReader GetReader(string commandText, IDbConnection connection, CommandBehavior behavior = CommandBehavior.CloseConnection, int commandTimeout = 30)
        {
            return AsyncUtil.CallSync(GetReaderAsync, commandText, connection, behavior, commandTimeout);
        }

        /// <summary>
        /// Retrieves a reader for a given <paramref name="commandText" /> on the <paramref name="connection" />.
        /// </summary>
        /// <param name="commandText">The command text to execute.</param>
        /// <param name="connection">The connection to use for the query.</param>
        /// <param name="behavior">The command beavior for the reader.</param>
        /// <param name="commandTimeout">The command timeout in seconds.</param>
        /// <returns>The reader or <c>null</c> if an error occurs.</returns>
        /// <exception cref="ArgumentException">
        /// Is thrown if the <paramref name="commandText" /> or <paramref name="connection" /> are invalid.
        /// </exception>
        public static async Task<IDataReader> GetReaderAsync(string commandText, IDbConnection connection, CommandBehavior behavior = CommandBehavior.CloseConnection, int commandTimeout = 30)
        {
            CheckUtil.ThrowIfNull(() => connection);
            CheckUtil.ThrowIfNullOrEmpty(() => commandText);
            try
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandTimeout = commandTimeout;
                    var commandToUse = command as DbCommand;
                    CheckUtil.ThrowIfNull(() => commandToUse);
                    // ReSharper disable once PossibleNullReferenceException
                    commandToUse.CommandText = commandText;
                    return await commandToUse.ExecuteReaderAsync(behavior).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                ExceptionCallback?.Invoke(ex);
                return null;
            }
        }

        /// <summary>
        /// Uses a <see cref="IDbCommand" /> to retrieve a scalar value using the given <paramref name="commandText" />.
        /// </summary>
        /// <typeparam name="TResult">The type of the expected result.</typeparam>
        /// <param name="commandText">The command text to execute.</param>
        /// <param name="connection">The connection to use for the query.</param>
        /// <param name="commandTimeout">The command timeout in seconds.</param>
        /// <returns>The result or the default value of <typeparamref name="TResult" /> if an error occurs.</returns>
        /// <exception cref="ArgumentException">
        /// Is thrown if the <paramref name="commandText" /> or <paramref name="connection" /> are invalid.
        /// </exception>
        [Obsolete("This implementation should not be used anymore because it uses deprecated calls.")]
        public static TResult GetScalarResult<TResult>(string commandText, IDbConnection connection, int commandTimeout = 30)
        {
            return AsyncUtil.CallSync(GetScalarResultAsync<TResult>, commandText, connection, commandTimeout);
        }

        /// <summary>
        /// Uses a <see cref="IDbCommand" /> to retrieve a scalar value using the given <paramref name="commandText" />.
        /// </summary>
        /// <typeparam name="TResult">The type of the expected result.</typeparam>
        /// <param name="commandText">The command text to execute.</param>
        /// <param name="connection">The connection to use for the query.</param>
        /// <param name="commandTimeout">The command timeout in seconds.</param>
        /// <returns>The result or the default value of <typeparamref name="TResult" /> if an error occurs.</returns>
        /// <exception cref="ArgumentException">
        /// Is thrown if the <paramref name="commandText" /> or <paramref name="connection" /> are invalid.
        /// </exception>
        public static async Task<TResult> GetScalarResultAsync<TResult>(string commandText, IDbConnection connection, int commandTimeout = 30)
        {
            CheckUtil.ThrowIfNull(() => connection);
            CheckUtil.ThrowIfNullOrEmpty(() => commandText);
            try
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandTimeout = commandTimeout;
                    var commandToUse = command as DbCommand;
                    CheckUtil.ThrowIfNull(() => commandToUse);
                    // ReSharper disable once PossibleNullReferenceException
                    commandToUse.CommandText = commandText;
                    var result = await commandToUse.ExecuteScalarAsync().ConfigureAwait(false);
                    if (result == null)
                    {
                        return default(TResult);
                    }
                    return (TResult)result;
                }
            }
            catch (Exception ex)
            {
                ExceptionCallback?.Invoke(ex);
                return default(TResult);
            }
        }

        #endregion
    }
}
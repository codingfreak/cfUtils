namespace codingfreaks.cfUtils.Logic.Base.Interfaces
{
    using System;

    using codingfreaks.cfUtils.Logic.Base.EventArguments;
    using codingfreaks.cfUtils.Logic.Standard.Structures;

    /// <summary>
    /// Must be implemented by all logging-components.
    /// </summary>
    public interface ILogger
    {
        #region events

        /// <summary>
        /// Occurs when the logger finished creating a log entry.
        /// </summary>
        event EventHandler<LoggerEventArgs> LogCreated;

        #endregion

        #region methods

        /// <summary>
        /// Can be used to pass a log entry of type <see cref="LogEntryType.Debug" />.
        /// </summary>
        /// <param name="message">A message that should be written to the log.</param>
        /// <param name="args">Arguments to match to parameters from <paramref name="message" /> if needed.</param>
        void LogDebug(string message, params object[] args);

        /// <summary>
        /// Can be used to pass a log entry of type <see cref="LogEntryType.Debug" />.
        /// </summary>
        /// <param name="eventId">The unique id for this type of log.</param>
        /// <param name="message">A message that should be written to the log.</param>
        /// <param name="args">Arguments to match to parameters from <paramref name="message" /> if needed.</param>
        void LogDebug(string eventId, string message, params object[] args);

        /// <summary>
        /// Can be used to pass a log entry of type <see cref="LogEntryType.Error" />.
        /// </summary>
        /// <param name="message">A message that should be written to the log.</param>
        /// <param name="args">Arguments to match to parameters from <paramref name="message" /> if needed.</param>
        void LogError(string message, params object[] args);

        /// <summary>
        /// Can be used to pass a log entry of type <see cref="LogEntryType.Error" />.
        /// </summary>
        /// <param name="eventId">The unique id for this type of log.</param>
        /// <param name="message">A message that should be written to the log.</param>
        /// <param name="args">Arguments to match to parameters from <paramref name="message" /> if needed.</param>
        void LogError(string eventId, string message, params object[] args);

        /// <summary>
        /// Can be used to pass in an <paramref name="exception" /> which will be logged as an error.
        /// </summary>
        /// <param name="exception">The exception to log.</param>
        void LogException(Exception exception);

        /// <summary>
        /// Can be used to pass in an <paramref name="exception" /> which will be logged as an error.
        /// </summary>
        /// <param name="eventId">The unique id for this type of log.</param>
        /// <param name="exception">The exception to log.</param>
        void LogException(string eventId, Exception exception);

        /// <summary>
        /// Can be used to pass a log entry.
        /// </summary>
        /// <param name="entryType">Informations about the type of log the caller wants to add.</param>
        /// <param name="message">A message that should be written to the log.</param>
        /// <param name="args">Arguments to match to parameters from <paramref name="message" /> if needed.</param>
        void LogMessage(LogEntryType entryType, string message, params object[] args);

        /// <summary>
        /// Can be used to pass a log entry.
        /// </summary>
        /// <param name="entryType">Informations about the type of log the caller wants to add.</param>
        /// <param name="eventId">The unique id for this type of log.</param>
        /// <param name="message">A message that should be written to the log.</param>
        /// <param name="args">Arguments to match to parameters from <paramref name="message" /> if needed.</param>
        void LogMessage(LogEntryType entryType, string eventId, string message, params object[] args);

        /// <summary>
        /// Can be used to pass a log entry of type <see cref="LogEntryType.Success" />.
        /// </summary>
        /// <param name="message">A message that should be written to the log.</param>
        /// <param name="args">Arguments to match to parameters from <paramref name="message" /> if needed.</param>
        void LogSuccess(string message, params object[] args);

        /// <summary>
        /// Can be used to pass a log entry of type <see cref="LogEntryType.Success" />.
        /// </summary>
        /// <param name="eventId">The unique id for this type of log.</param>
        /// <param name="message">A message that should be written to the log.</param>
        /// <param name="args">Arguments to match to parameters from <paramref name="message" /> if needed.</param>
        void LogSuccess(string eventId, string message, params object[] args);

        /// <summary>
        /// Can be used to pass a log entry of type <see cref="LogEntryType.Warning" />.
        /// </summary>
        /// <param name="message">A message that should be written to the log.</param>
        /// <param name="args">Arguments to match to parameters from <paramref name="message" /> if needed.</param>
        void LogWarning(string message, params object[] args);

        /// <summary>
        /// Can be used to pass a log entry of type <see cref="LogEntryType.Warning" />.
        /// </summary>
        /// <param name="eventId">The unique id for this type of log.</param>
        /// <param name="message">A message that should be written to the log.</param>
        /// <param name="args">Arguments to match to parameters from <paramref name="message" /> if needed.</param>
        void LogWarning(string eventId, string message, params object[] args);

        #endregion
    }
}
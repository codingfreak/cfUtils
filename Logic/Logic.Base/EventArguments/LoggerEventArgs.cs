namespace codingfreaks.cfUtils.Logic.Base.EventArguments
{
    using System;

    using Interfaces;

    using Portable.Structures;

    /// <summary>
    /// Will be passed by the <see cref="ILogger.LogCreated"/> event.
    /// </summary>
    public class LoggerEventArgs : EventArgs
    {
        #region constructors and destructors

        /// <summary>
        /// Initializes a new instance of this type.
        /// </summary>
        /// <param name="entryType">The type of the log-entry.</param>
        /// <param name="eventId">A unique ID for this event.</param>
        /// <param name="message">The complete message which was logged.</param>
        /// <param name="sourceFile">The file from which the log was triggered.</param>
        /// <param name="sourceLineNumber">The line-number inside the <paramref name="sourceFile"/> from which the log was triggered.</param>
        /// <param name="sourceMethodName">The method-name inside the <paramref name="sourceFile"/> from which the log was triggered.</param>
        public LoggerEventArgs(LogEntryType entryType, string eventId, string message, string sourceFile, int sourceLineNumber, string sourceMethodName)
        {
            EntryType = entryType;
            Message = message;
            SourceFile = sourceFile;
            SourceLineNumber = sourceLineNumber;
            SourceMethodName = sourceMethodName;
            EventId = eventId;
        }

        #endregion

        #region properties

        /// <summary>
        /// The type of the log-entry.
        /// </summary>
        public LogEntryType EntryType { get; private set; }

        /// <summary>
        /// A unique ID for this event.
        /// </summary>
        public string EventId { get; private set; }

        /// <summary>
        /// The complete message which was logged.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// The file from which the log was triggered.
        /// </summary>
        public string SourceFile { get; private set; }

        /// <summary>
        /// The line-number inside the <see cref="SourceFile"/> from which the log was triggered.
        /// </summary>
        public int SourceLineNumber { get; private set; }

        /// <summary>
        /// The method-name inside the <see cref="SourceFile"/> from which the log was triggered.
        /// </summary>
        public string SourceMethodName { get; private set; }

        #endregion
    }
}
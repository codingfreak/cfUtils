namespace codingfreaks.cfUtils.Logic.Base.Utilities
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Provides easy access to <see cref="Trace"/> based on the TRACE conditional compiler attribute.
    /// </summary>
    public static class TraceUtil
    {
        #region methods

        /// <summary>
        /// Writes a trace-debug message if TRACE conditional compiler attribute is set.
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="parameters">Optional parameters to insert to <paramref name="message"/>.</param>
        public static void WriteTraceDebug(string message, params object[] parameters)
        {
            WriteTraceDebugIfTrace(string.Format(CultureInfo.InvariantCulture, message, parameters));
        }

        /// <summary>
        /// Writes a trace-error message if TRACE conditional compiler attribute is set.
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="parameters">Optional parameters to insert to <paramref name="message"/>.</param>
        public static void WriteTraceError(string message, params object[] parameters)
        {
            WriteTraceErrorIfTrace(string.Format(CultureInfo.InvariantCulture, message, parameters));
        }

        /// <summary>
        /// Writes a trace error line including caller informations if TRACE conditional compiler attribute is set.
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="callerName">The class name of the caller (passed in autmatically).</param>
        /// <param name="callerFilePath">The file name of the caller (passed in autmatically).</param>
        /// <param name="callerLineNumber">The line number inside the caller (passed in autmatically).</param>
        public static void WriteTraceErrorWithContext(
            string message,
            [CallerMemberName] string callerName = "",
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLineNumber = 0)
        {
            var messageText = $"{callerName} in {callerFilePath} at line {callerLineNumber}: {message}";
            WriteTraceErrorIfTrace(messageText);
        }

        /// <summary>
        /// Writes a trace-info message if TRACE conditional compiler attribute is set.
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="parameters">Optional parameters to insert to <paramref name="message"/>.</param>
        public static void WriteTraceInfo(string message, params object[] parameters)
        {
            WriteTraceInfoIfTrace(string.Format(CultureInfo.InvariantCulture, message, parameters));
        }

        /// <summary>
        /// Writes a trace info line including caller informations if TRACE conditional compiler attribute is set.
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="callerName">The class name of the caller (passed in autmatically).</param>
        /// <param name="callerFilePath">The file name of the caller (passed in autmatically).</param>
        /// <param name="callerLineNumber">The line number inside the caller (passed in autmatically).</param>
        public static void WriteTraceInfoWithContext(
            string message,
            [CallerMemberName] string callerName = "",
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLineNumber = 0)
        {
            var messageText = $"{callerName} in {callerFilePath} at line {callerLineNumber}: {message}";
            WriteTraceInfoIfTrace(messageText);
        }

        /// <summary>
        /// Writes a trace-warning message if TRACE conditional compiler attribute is set.
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="parameters">Optional parameters to insert to <paramref name="message"/>.</param>
        public static void WriteTraceWarning(string message, params object[] parameters)
        {
            WriteTraceWarningIfTrace(string.Format(CultureInfo.InvariantCulture, message, parameters));
        }

        /// <summary>
        /// Writes a trace warning line including caller informations if TRACE conditional compiler attribute is set.
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="callerName">The class name of the caller (passed in autmatically).</param>
        /// <param name="callerFilePath">The file name of the caller (passed in autmatically).</param>
        /// <param name="callerLineNumber">The line number inside the caller (passed in autmatically).</param>
        public static void WriteTraceWarningWithContext(
            string message,
            [CallerMemberName] string callerName = "",
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLineNumber = 0)
        {
            var messageText = $"{callerName} in {callerFilePath} at line {callerLineNumber}: {message}";
            WriteTraceWarningIfTrace(messageText);
        }

        /// <summary>
        /// Writes a trace-debug message if TRACE conditional compiler attribute is set.
        /// </summary>
        /// <param name="message">The message to write to trace.</param>
        [Conditional("TRACE")]
        private static void WriteTraceDebugIfTrace(string message)
        {
            Trace.WriteLine(message);
        }

        /// <summary>
        /// Writes a trace-error message if TRACE conditional compiler attribute is set.
        /// </summary>
        /// <param name="message">The message to write to trace.</param>
        [Conditional("TRACE")]
        private static void WriteTraceErrorIfTrace(string message)
        {
            Trace.TraceError(message);
        }

        /// <summary>
        /// Writes a trace-info message if TRACE conditional compiler attribute is set.
        /// </summary>
        /// <param name="message">The message to write to trace.</param>
        [Conditional("TRACE")]
        private static void WriteTraceInfoIfTrace(string message)
        {
            Trace.TraceInformation(message);
        }

        /// <summary>
        /// Writes a trace-warning message if TRACE conditional compiler attribute is set.
        /// </summary>
        /// <param name="message">The message to write to trace.</param>
        [Conditional("TRACE")]
        private static void WriteTraceWarningIfTrace(string message)
        {
            Trace.TraceWarning(message);
        }

        #endregion
    }
}
namespace codingfreaks.cfUtils.Logic.Portable.Structures
{
    /// <summary>
    /// Defines possible values for types of logging entries.
    /// </summary>
    public enum LogEntryType
    {
        /// <summary>
        /// Unknown type of log entry.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Log entry should only by valueable for developers.
        /// </summary>
        Debug = 1,

        /// <summary>
        /// Entry indicates a succeeded operation.
        /// </summary>
        Success = 2,

        /// <summary>
        /// Entry indicates that something has to be checked but worked in principle.
        /// </summary>
        Warning = 3,

        /// <summary>
        /// An operation failed.
        /// </summary>
        Error = 4
    }
}
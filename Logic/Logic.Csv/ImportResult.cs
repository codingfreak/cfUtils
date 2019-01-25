namespace codingfreaks.cfUtils.Logic.Csv
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Wrapper used as a result for <see cref="Importer{T}.ImportAsync" />.
    /// </summary>
    /// <typeparam name="TResult">The type of one of the items in <see cref="Items" />.</typeparam>
    public class ImportResult<TResult>
    {
        #region constructors and destructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="succeeded">Shows if the operation succeeded.</param>
        /// <param name="items">The resulting items if any.</param>
        /// <param name="started">The timestamp when the operation started.</param>
        /// <param name="finished">The timestamp when the complete operation was finished.</param>
        /// <param name="skippedLines">The amount of CSV-file-lines skipped by the importer.</param>
        public ImportResult(bool succeeded, IEnumerable<TResult> items, DateTimeOffset started, DateTimeOffset finished, long skippedLines = 0)
        {
            Succeeded = succeeded;
            Items = items;
            Started = started;
            Finished = finished;
            SkippedLines = skippedLines;
        }

        #endregion

        #region properties

        /// <summary>
        /// Shows if the operation failed.
        /// </summary>
        public bool Failed => !Succeeded;

        /// <summary>
        /// The timestamp when the complete operation was finished.
        /// </summary>
        public DateTimeOffset Finished { get; }

        /// <summary>
        /// The resulting items if any.
        /// </summary>
        public IEnumerable<TResult> Items { get; }

        /// <summary>
        /// The amount of <see cref="Items" />.
        /// </summary>
        public long ItemsCount => Items?.Count() ?? 0;

        /// <summary>
        /// The amount of CSV-file-lines skipped by the importer.
        /// </summary>
        public long SkippedLines { get; }

        /// <summary>
        /// The timestamp when the operation started.
        /// </summary>
        public DateTimeOffset Started { get; }

        /// <summary>
        /// Shows if the operation succeeded.
        /// </summary>
        public bool Succeeded { get; }

        #endregion
    }
}
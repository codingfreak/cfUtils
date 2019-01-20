namespace codingfreaks.cfUtils.Logic.Csv
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ImportResult<TResult>
    {
        #region constructors and destructors

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

        public bool Failed => !Succeeded;

        public DateTimeOffset Finished { get; }

        public IEnumerable<TResult> Items { get; }

        public DateTimeOffset Started { get; }

        public bool Succeeded { get; }

        public long ItemsCount => Items?.Count() ?? 0;

        public long SkippedLines { get; }

        #endregion
    }
}
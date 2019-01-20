namespace codingfreaks.cfUtils.Logic.Csv
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ImportResult<TResult>
    {
        #region constructors and destructors

        public ImportResult(bool succeeded, IEnumerable<TResult> items, DateTimeOffset started, DateTimeOffset finished)
        {
            Succeeded = succeeded;
            Items = items;
            Started = started;
            Finished = finished;
        }

        #endregion

        #region properties

        public bool Failed => !Succeeded;

        public DateTimeOffset Finished { get; }

        public IEnumerable<TResult> Items { get; }

        public DateTimeOffset Started { get; }

        public bool Succeeded { get; }

        #endregion
    }
}
namespace codingfreaks.cfUtils.Logic.Azure
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;

    using Microsoft.WindowsAzure.Storage.Table;

    using codingfreaks.cfUtils.Logic.Portable.Extensions;

    /// <summary>
    /// Defines the structure of one line in the WADLogTable in Azure.
    /// </summary>
    public class WadLogEntity : TableEntity
    {
        #region constructors and destructors

        public WadLogEntity()
        {
            PartitionKey = "a";
            RowKey = $"{DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks:10}_{Guid.NewGuid()}";
        }

        #endregion

        #region properties

        /// <summary>
        /// The calculate datetime using the partition key which contains the event-time ticks.
        /// </summary>
        public DateTime EventDateTime => new DateTime(long.Parse(PartitionKey.Substring(1)));

        /// <summary>
        /// The id of the event.
        /// </summary>
        public int EventId { get; set; }

        /// <summary>
        /// The log level.
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// The message text.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Extractx the content of the Message attribute inside of the <see cref="Message"/> or
        /// retrieves the original <see cref="Message"/> if there is no such attribute.
        /// </summary>
        public string MessageCleaned
        {
            get
            {
                var regex = @"Message=(\\?"")(.*?)\1";
                var result = Regex.Match(Message, regex);
                return result.Success && result.Groups.Count == 3 ? result.Groups[2].Captures[0].Value : Message;
            }
        }

        /// <summary>
        /// The process id of the trigger process.
        /// </summary>
        public int Pid { get; set; }

        /// <summary>
        /// The role name.
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// The role instance name.
        /// </summary>
        public string RoleInstance { get; set; }

        /// <summary>
        /// The offset text value for this row.
        /// </summary>
        public string RowIndex { get; set; }

        /// <summary>
        /// The numeric value of <see cref="RowIndex"/>.
        /// </summary>
        public long RowIndexValue => RowIndex.IsNullOrEmpty() ? 0 : long.Parse(RowIndex);

        /// <summary>
        /// The TID.
        /// </summary>
        public int Tid { get; set; }

        #endregion
    }
}
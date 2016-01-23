namespace codingfreaks.cfUtils.Logic.Azure
{
    using System;

    using Microsoft.WindowsAzure.Storage.Queue;

    /// <summary>
    /// Is used by events wich will pass cloud queue messages.
    /// </summary>
    public class CloudQueueMessageEventArgs : EventArgs
    {
        #region constructors and destructors

        /// <summary>
        /// Instantiates a new instance of this type.
        /// </summary>
        /// <param name="message">The cloud message.</param>
        /// <param name="queueName">The name of the queue.</param>
        public CloudQueueMessageEventArgs(CloudQueueMessage message, string queueName)
        {
            Message = message;
            QueueName = queueName;
        }

        #endregion

        #region properties

        /// <summary>
        /// The queue message.
        /// </summary>
        public CloudQueueMessage Message { get; private set; }

        /// <summary>
        /// The name of the queue.
        /// </summary>
        public string QueueName { get; private set; }

        #endregion
    }
}
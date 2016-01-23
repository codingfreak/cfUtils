namespace s2.s2Utils.Logic.Azure
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Azure;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;
    using Microsoft.WindowsAzure.Storage.Queue;
    using Microsoft.WindowsAzure.Storage.Table;
    using Microsoft.WindowsAzure.Storage.Table.Protocol;
    using Microsoft.WindowsAzure.Storage.Table.Queryable;

    using s2.s2Utils.Logic.Base.Extensions;
    using s2.s2Utils.Logic.Base.Utilities;
    using s2.s2Utils.Logic.Portable.Extensions;
    using s2.s2Utils.Logic.Portable.Utilities;

    /// <summary>
    /// Provides easy access to Azure storage accounts.
    /// </summary>
    public static class StorageHelper
    {
        #region constants

        #region static fields

        private static CancellationTokenSource _tokenSource;

        #endregion

        #endregion

        #region events

        #region event declarations

        #region events

        #region event declarations

        /// <summary>
        /// Occurs when a new cloud queue message was detected.
        /// </summary>
        public static event EventHandler<CloudQueueMessageEventArgs> OnMessageReceived;

        #endregion

        #endregion

        #endregion

        /// <summary>
        /// Occurs when <see cref="ClearAsync"/> removed a chunk of entries.
        /// </summary>
        public static event EventHandler<AmountBasedEventArgs> TableItemsRemoved;

        #endregion

        #region constructors and destructors

        static StorageHelper()
        {
            StorageConnectionStringKey = "CloudStorageConnectionString";
        }

        #endregion

        #region methods

        /// <summary>
        /// Adds a new message with <paramref name="messageContent"/> to a <paramref name="queueName"/>.
        /// </summary>
        /// <param name="queueName">The name of the queue.</param>
        /// <param name="messageContent">The content to pass to the queue.</param>
        /// <param name="accountConnectionString">
        /// The connection string to connect to Azure or <c>null</c> if logic should search in configuration for 
        /// an app-setting defined in <see cref="StorageConnectionStringKey"/>.
        /// </param>
        /// <returns><c>true</c> if the message was added otherwise <c>false</c>.</returns>
        public static async Task<bool> AddQueueMessageAsync(string queueName, string messageContent, string accountConnectionString = null)
        {
            var queue = await GetQueueAsync(queueName, accountConnectionString);
            var message = new CloudQueueMessage(messageContent);
            try
            {
                await queue.AddMessageAsync(message);
                return true;
            }
            catch (Exception ex)
            {
                TraceUtil.WriteTraceError(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Removes all entries from a given Azure Storage Table.
        /// </summary>
        /// <param name="table">The table to clear.</param>
        /// <param name="parallelity">The amount of threads to use in parallel</param>
        /// <param name="raiseEvents">If <c>true</c> the <see cref="TableItemsRemoved"/> event will be raised regulary to indicate progress.</param>        
        public static async Task ClearAsync(this CloudTable table, int parallelity = 0, bool raiseEvents = true)
        {
            var query = new TableQuery<WadLogEntity>();
            TableContinuationToken continuationToken = null;
            if (parallelity <= 0)
            {
                parallelity = Environment.ProcessorCount * 4;
            }
            parallelity = Math.Max(parallelity, 48);
            var tasks = new List<Task>();
            do
            {
                var tableQueryResult = await table.ExecuteQuerySegmentedAsync(query, continuationToken);
                continuationToken = tableQueryResult.ContinuationToken;
                var elements = tableQueryResult.OrderBy(ele => ele.PartitionKey).ToList();
                while (tasks.Count == parallelity)
                {
                    Task.WaitAny(tasks.ToArray());
                    tasks.RemoveAll(t => t.IsCompleted || t.IsCanceled);
                }
                tasks.Add(
                    Task.Run(
                        async () =>
                        {
                            var lastPartKey = string.Empty;
                            var operations = new TableBatchOperation();
                            foreach (var item in elements)
                            {
                                if (!lastPartKey.IsNullOrEmpty() && !item.PartitionKey.Equals(lastPartKey))
                                {
                                    // we have to start the batch now because the current item will have a new 
                                    // partition key and batches are only allowed within the same partition key
                                    await table.ExecuteBatchAsync(operations, raiseEvents);
                                }
                                operations.Add(TableOperation.Delete(item));
                                lastPartKey = item.PartitionKey;
                                if (operations.Count == 100)
                                {
                                    // take care that the maximum amount of items for a batch is used
                                    await table.ExecuteBatchAsync(operations, raiseEvents);
                                }
                            }
                            if (operations.Any())
                            {
                                // take care of the rest of the operations
                                await table.ExecuteBatchAsync(operations, raiseEvents);
                            }
                        }));
            }
            while (continuationToken != null);
            Task.WaitAll(tasks.ToArray());
        }

        /// <summary>
        /// Deletes all blobs inside a storage container.
        /// </summary>
        /// <param name="containerName">The name of the container inside the storage account.</param>
        /// <param name="accountConnectionString">
        /// The connection string to connect to Azure or <c>null</c> if logic should search in configuration for 
        /// an app-setting defined in <see cref="StorageConnectionStringKey"/>.
        /// </param>
        /// <returns>The amount of items that where deleted.</returns>
        public static async Task<int> DeleteAllBlobsAsync(string containerName, string accountConnectionString = null)
        {
            var container = await GetContainerAsync(containerName, accountConnectionString);
            return await DeleteAllBlobsAsync(container);
        }

        /// <summary>
        /// Deletes all blobs inside a storage container.
        /// </summary>
        /// <param name="container">The storage container to use.</param>
        /// <returns>The amount of items that where deleted.</returns>
        public static async Task<int> DeleteAllBlobsAsync(CloudBlobContainer container)
        {
            var blobs = await ListBlockBlobsAsync(container);
            var deletes = 0;
            foreach (var blob in blobs)
            {
                await blob.DeleteAsync();
                deletes++;
            }
            return deletes;
        }

        /// <summary>
        /// Extends <see cref="CloudTable"/> to enable batched deleting of all entries in it.
        /// </summary>
        /// <param name="table">The Cloud Table where to delete all entries.</param>
        /// <param name="filter">A filter for selecting the entities.</param>
        public static void DeleteAllEntitiesInBatches(this CloudTable table, Expression<Func<DynamicTableEntity, bool>> filter)
        {
            DeleteAllEntitiesInBatches<DynamicTableEntity>(table, filter);
        }

        /// <summary>
        /// Extends <see cref="CloudTable"/> to enable batched deleting of all entries in it.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entities in the <paramref name="table"/>.</typeparam>
        /// <param name="table">The Cloud Table where to delete all entries.</param>
        /// <param name="filter">A filter for selecting the entities.</param>
        public static void DeleteAllEntitiesInBatches<TEntity>(this CloudTable table, Expression<Func<TEntity, bool>> filter) where TEntity : ITableEntity, new()
        {
            Action<IEnumerable<TEntity>> processor = entities =>
            {
                var batches = new Dictionary<string, TableBatchOperation>();

                foreach (var entity in entities)
                {
                    TableBatchOperation batch = null;
                    if (batches.TryGetValue(entity.PartitionKey, out batch) == false)
                    {
                        batches[entity.PartitionKey] = batch = new TableBatchOperation();
                    }
                    batch.Add(TableOperation.Delete(entity));
                    if (batch.Count != 100)
                    {
                        continue;
                    }
                    table.ExecuteBatch(batch);
                    batches[entity.PartitionKey] = new TableBatchOperation();
                }
                foreach (var batch in batches.Values.Where(batch => batch.Count > 0))
                {
                    table.ExecuteBatch(batch);
                }
            };
            table.ProcessEntities(processor, filter);
        }

        /// <summary>
        /// Executes a batch operation on a Azure Storage table securely and raises the <see cref="TableItemsRemoved"/> event.
        /// </summary>
        /// <remarks>
        /// Be ware to supply only operations that are working on the same partition key. Otherwise an exception will occur in 
        /// Azure Storage.
        /// </remarks>
        /// <param name="table">The table on which to execute the batch.</param>
        /// <param name="operations">The batch containing the operations.</param>
        /// <param name="raiseEvents">If <c>true</c> the <see cref="TableItemsRemoved"/> event will be raised if the btach completed.</param>           
        public static async Task ExecuteBatchAsync(this CloudTable table, TableBatchOperation operations, bool raiseEvents)
        {
            if (!operations.Any())
            {
                return;
            }
            try
            {
                await table.ExecuteBatchAsync(operations);
                if (raiseEvents)
                {
                    TableItemsRemoved?.Invoke(null, new AmountBasedEventArgs(operations.Count));
                }
                operations.Clear();
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
            }
        }

        /// <summary>
        /// Retrieves a reference to a blob container referenced by the <paramref name="containerName"/>.
        /// </summary>
        /// <remarks>
        /// If <paramref name="accountConnectionString"/> is null or empty this code assumes that the calling
        /// assembly is defining an app.config App-setting named <see cref="StorageConnectionStringKey"/>.
        /// </remarks>
        /// <param name="containerName">The name of the container inside the storage account.</param>
        /// <param name="accountConnectionString">
        /// The connection string to connect to Azure or <c>null</c> if logic should search in configuration for 
        /// an app-setting defined in <see cref="StorageConnectionStringKey"/>.
        /// </param>
        /// <returns>The ready-to-use container.</returns>
        public static async Task<CloudBlobContainer> GetContainerAsync(string containerName, string accountConnectionString = null)
        {
            if (string.IsNullOrEmpty(containerName))
            {
                throw new ArgumentNullException("containerName");
            }
            if (string.IsNullOrEmpty(accountConnectionString))
            {
                // try to get the connection string from config
                accountConnectionString = CloudConfigurationManager.GetSetting(StorageConnectionStringKey);
            }
            if (string.IsNullOrEmpty(accountConnectionString))
            {
                throw new InvalidOperationException("Could not retrieve connection string!");
            }
            CloudStorageAccount account = null;
            account = CloudStorageAccount.Parse(accountConnectionString);
            if (account == null)
            {
                throw new InvalidOperationException("Could not generate account.");
            }
            var blobClient = account.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(containerName);
            var options = new BlobRequestOptions();
            try
            {
                await container.CreateIfNotExistsAsync(BlobContainerPublicAccessType.Container, options, null);
                var permissions = await container.GetPermissionsAsync();
                if (permissions.PublicAccess == BlobContainerPublicAccessType.Off)
                {
                    permissions.PublicAccess = BlobContainerPublicAccessType.Blob;
                    await container.SetPermissionsAsync(permissions);
                }
            }
            catch (SocketException sockex)
            {
                throw new InvalidOperationException("Could not establish connection to server. Ensure that storage simulator runs, if you are running in dev-mode!", sockex);
            }
            return container;
        }

        /// <summary>
        /// Retrieves the next available message from a Azure storage queue.
        /// </summary>
        /// <param name="queueName">The name of the queue in the cloud.</param>
        /// <param name="accountConnectionString">
        /// The connection string to connect to Azure or <c>null</c> if logic should search in configuration for 
        /// an app-setting defined in <see cref="StorageConnectionStringKey"/>.
        /// </param>
        /// <param name="deleteMessage">Indicates whether the message should be deleted after retrieving it.</param>
        /// <returns>The message or <c>null</c> if no message could be retrieved.</returns>
        public static async Task<CloudQueueMessage> GetNextQueueMessageAsync(string queueName, string accountConnectionString = null, bool deleteMessage = true)
        {
            var queue = await GetQueueAsync(queueName, accountConnectionString);
            await queue.FetchAttributesAsync();
            if (queue.ApproximateMessageCount <= 0)
            {
                return null;
            }
            var message = await queue.GetMessageAsync();
            if (deleteMessage)
            {
                await queue.DeleteMessageAsync(message);
            }
            return message;
        }

        /// <summary>
        /// Retrieves an Azure CloudQueue by it's name.
        /// </summary>
        /// <param name="queueName">The name of the queue.</param>
        /// <param name="accountConnectionString">
        /// The connection string to connect to Azure or <c>null</c> if logic should search in configuration for 
        /// an app-setting defined in <see cref="StorageConnectionStringKey"/>.
        /// </param>
        /// <returns>The queue or <c>null</c> if no queue was found.</returns>
        public static async Task<CloudQueue> GetQueueAsync(string queueName, string accountConnectionString = null)
        {
            if (string.IsNullOrEmpty(queueName))
            {
                throw new ArgumentNullException("queueName");
            }
            if (string.IsNullOrEmpty(accountConnectionString))
            {
                // try to get the connection string from config
                accountConnectionString = CloudConfigurationManager.GetSetting(StorageConnectionStringKey);
            }
            if (string.IsNullOrEmpty(accountConnectionString))
            {
                throw new InvalidOperationException("Could not retrieve connection string!");
            }
            CloudStorageAccount account = null;
            account = CloudStorageAccount.Parse(accountConnectionString);
            if (account == null)
            {
                throw new InvalidOperationException("Could not generate account.");
            }
            var queueClient = account.CreateCloudQueueClient();
            var queue = queueClient.GetQueueReference(queueName);
            await queue.CreateIfNotExistsAsync();
            return queue;
        }

        /// <summary>
        /// Retrieves a reference to a single table in Azure Storage.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="accountConnectionString">
        /// The connection string to connect to Azure or <c>null</c> if logic should search in configuration for 
        /// an app-setting defined in <see cref="StorageConnectionStringKey"/>.
        /// </param>
        /// <returns></returns>
        public static CloudTable GetTableReference(string tableName, string accountConnectionString = null)
        {
            if (string.IsNullOrEmpty(accountConnectionString))
            {
                // try to get the connection string from config
                accountConnectionString = CloudConfigurationManager.GetSetting(StorageConnectionStringKey);
            }
            CloudStorageAccount storageAccount;
            try
            {
                storageAccount = CloudStorageAccount.Parse(accountConnectionString);
            }
            catch
            {
                return null;
            }
            var tableClient = storageAccount.CreateCloudTableClient();
            return tableClient.GetTableReference(tableName);
        }

        /// <summary>
        /// Retrieves a list of all block-blobs inside the <paramref name="containerName"/>.
        /// </summary>
        /// <remarks>
        /// If <paramref name="accountConnectionString"/> is null or empty this code assumes that the calling
        /// assembly is defining an app.config App-setting named <see cref="StorageConnectionStringKey"/>.
        /// </remarks>
        /// <param name="containerName">The name of the container inside the storage account.</param>
        /// <param name="accountConnectionString">
        /// The connection string to connect to Azure or <c>null</c> if logic should search in configuration for 
        /// an app-setting defined in <see cref="StorageConnectionStringKey"/>.
        /// </param>
        /// <returns>The list if block blobs found inside the <paramref name="containerName"/>.</returns>
        public static IEnumerable<CloudBlockBlob> ListBlockBlobs(string containerName, string accountConnectionString = null)
        {
            IEnumerable<CloudBlockBlob> result = null;
            var task = Task.Run(
                async () =>
                {
                    result = await ListBlockBlobsAsync(containerName, accountConnectionString);
                });
            task.Wait();
            return result;
        }

        /// <summary>
        /// Retrieves a list of all block-blobs inside the <paramref name="containerName"/>.
        /// </summary>
        /// <remarks>
        /// If <paramref name="accountConnectionString"/> is null or empty this code assumes that the calling
        /// assembly is defining an app.config App-setting named <see cref="StorageConnectionStringKey"/>.
        /// </remarks>
        /// <param name="containerName">The name of the container inside the storage account.</param>
        /// <param name="accountConnectionString">
        /// The connection string to connect to Azure or <c>null</c> if logic should search in configuration for 
        /// an app-setting defined in <see cref="StorageConnectionStringKey"/>.
        /// </param>
        /// <returns>The list if block blobs found inside the <paramref name="containerName"/>.</returns>
        public static async Task<IEnumerable<CloudBlockBlob>> ListBlockBlobsAsync(string containerName, string accountConnectionString = null)
        {
            var container = await GetContainerAsync(containerName, accountConnectionString);
            return await ListBlockBlobsAsync(container);
        }

        /// <summary>
        /// Retrieves a list of all block-blobs inside the <paramref name="container"/>.
        /// </summary>
        /// <param name="container">The storage container to use.</param>
        /// <returns>The list if block blobs found inside the <paramref name="container"/>.</returns>
        public static Task<IEnumerable<CloudBlockBlob>> ListBlockBlobsAsync(CloudBlobContainer container)
        {
            return Task.Run(() => container.ListBlobs().OfType<CloudBlockBlob>());
        }

        /// <summary>
        /// Performs the <paramref name="action"/> on a <paramref name="table"/> filtered by <paramref name="filter"/> for <see cref="DynamicTableEntity"/>.
        /// </summary>        
        /// <param name="table">The cloud table.</param>
        /// <param name="action">The action to perform on the entities.</param>
        /// <param name="filter">A filter for selecting the entities.</param>
        public static void ProcessEntities(this CloudTable table, Action<IEnumerable<DynamicTableEntity>> action, Expression<Func<DynamicTableEntity, bool>> filter)
        {
            ProcessEntities(table, action, filter);
        }

        /// <summary>
        /// Performs the <paramref name="action"/> on a <paramref name="table"/> filtered by <paramref name="filter"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entities in the <paramref name="table"/>.</typeparam>
        /// <param name="table">The cloud table.</param>
        /// <param name="action">The action to perform on the entities.</param>
        /// <param name="filter">A filter for selecting the entities.</param>
        public static void ProcessEntities<TEntity>(this CloudTable table, Action<IEnumerable<TEntity>> action, Expression<Func<TEntity, bool>> filter) where TEntity : ITableEntity, new()
        {
            TableQuerySegment<TEntity> segment = null;
            while (segment == null || segment.ContinuationToken != null)
            {
                if (filter == null)
                {
                    segment = table.ExecuteQuerySegmented(new TableQuery<TEntity>().Take(100), segment?.ContinuationToken);
                }
                else
                {
                    var query = table.CreateQuery<TEntity>().Where(filter).Take(100).AsTableQuery();
                    segment = query.ExecuteSegmented(segment?.ContinuationToken);
                }
                action(segment.Results);
            }
        }

        /// <summary>
        /// Allows to safely try to create a <paramref name="table"/> in the cloud and respects the fact that Azure will block creation after
        /// deleting a table for an amount of time.
        /// </summary>
        /// <param name="table">The cloud table to create.</param>
        /// <param name="maxSecondsToWait">An optional amount of seconds to wait for the operation to complete.</param>
        /// <param name="requestOptions">The options for the request.</param>
        /// <param name="operationContext">An optional operation context.</param>
        /// <returns><c>true</c> if creation succeeded otherwise <c>false</c>.</returns>
        public static bool SafeCreateIfNotExists(this CloudTable table, int maxSecondsToWait = 0, TableRequestOptions requestOptions = null, OperationContext operationContext = null)
        {
            var watch = new Stopwatch();
            watch.Start();
            do
            {
                try
                {
                    return table.CreateIfNotExists(requestOptions, operationContext);
                }
                catch (StorageException e)
                {
                    if ((e.RequestInformation.HttpStatusCode == 409) && (e.RequestInformation.ExtendedErrorInformation.ErrorCode.Equals(TableErrorCodeStrings.TableBeingDeleted)))
                    {
                        // The table is currently being deleted. Try again until it works.
                        Thread.Sleep(1000);
                    }
                    else
                    {
                        throw;
                    }
                }
                if (maxSecondsToWait > 0 && watch.Elapsed.TotalSeconds >= maxSecondsToWait)
                {
                    // exit the method because wait time exceeded.
                    return false;
                }
            }
            while (true);
        }

        /// <summary>
        /// Starts watching of a queue with a <paramref name="queueName"/>.
        /// </summary>
        /// <param name="queueName">The name of the Azure storage queue.</param>
        /// <param name="accountConnectionString">
        /// The connection string to connect to Azure or <c>null</c> if logic should search in configuration for 
        /// an app-setting defined in <see cref="StorageConnectionStringKey"/>.
        /// </param>
        public static void StartQueueWatcher(string queueName, string accountConnectionString = null)
        {
            if (OnMessageReceived == null)
            {
                throw new InvalidOperationException("Ensure listening to the OnMessageReceived event!");
            }
            var queue = GetQueueAsync(queueName, accountConnectionString).Result;
            var token = TokenSource.Token;
            Task.Run(
                async () =>
                {
                    while (!token.IsCancellationRequested)
                    {
                        try
                        {
                            var newMessage = await GetNextQueueMessageAsync(queueName, accountConnectionString);
                            if (newMessage != null)
                            {
                                OnMessageReceived(null, new CloudQueueMessageEventArgs(newMessage, queueName));
                            }
                            Task.Delay(100, token).Wait(token);
                        }
                        catch
                        {
                        }
                    }
                },
                token);
        }

        /// <summary>
        /// Stops the watching of a queue that was started before.
        /// </summary>
        public static void StopQueueWatcher()
        {
            TokenSource.Cancel();
        }

        /// <summary>
        /// Uploads a single file from local file system to Azure storage as a block blob.
        /// </summary>
        /// <param name="containerName">The name of the container inside the storage account.</param>
        /// <param name="localFileUri">The path to the local file.</param>
        /// <param name="accountConnectionString">
        /// The connection string to connect to Azure or <c>null</c> if logic should search in configuration for 
        /// an app-setting defined in <see cref="StorageConnectionStringKey"/>.
        /// </param>
        /// <param name="remoteFileName">The name the file should have in the storage container.</param>
        /// <param name="remoteFolder">A folder name to use or create in the storage container.</param>
        /// <param name="contentType">An optional content-type in MIME format to set for the new BLOB.</param>
        /// <returns><c>true</c> if the upload succeeds otherwise <c>false</c>.</returns>
        public static async Task<bool> UploadFileAsync(
            string containerName,
            string localFileUri,
            string accountConnectionString = null,
            string remoteFileName = null,
            string remoteFolder = null,
            string contentType = null)
        {
            var container = await GetContainerAsync(containerName, accountConnectionString);
            return await UploadFileAsync(container, localFileUri, remoteFileName, remoteFolder, contentType);
        }

        /// <summary>
        /// Uploads a local file to a storage container.
        /// </summary>
        /// <param name="container">The storage container to use.</param>
        /// <param name="localFileUri">The path to the local file.</param>
        /// <param name="remoteFileName">The name the file should have in the storage container.</param>
        /// <param name="remoteFolder">A folder name to use or create in the storage container.</param>
        /// <param name="contentType">An optional content-type in MIME format to set for the new BLOB.</param>
        /// <returns><c>true</c> if the upload succeeds otherwise <c>false</c>.</returns>
        public static async Task<bool> UploadFileAsync(CloudBlobContainer container, string localFileUri, string remoteFileName = null, string remoteFolder = null, string contentType = null)
        {
            if (!File.Exists(localFileUri))
            {
                throw new FileNotFoundException();
            }
            var localFile = new FileInfo(localFileUri);
            if (remoteFileName.IsNullOrEmpty())
            {
                remoteFileName = localFile.Name;
            }
            if (!remoteFolder.IsNullOrEmpty())
            {
                remoteFileName = Path.Combine(remoteFolder, remoteFileName);
            }
            // we know how to store the file in Azure
            var newBlob = container.GetBlockBlobReference(remoteFileName);
            if (!contentType.IsNullOrEmpty())
            {
                newBlob.Properties.ContentType = contentType;
            }
            await newBlob.UploadFromFileAsync(localFileUri, FileMode.Open);
            return true;
        }

        /// <summary>
        /// Uploads a single image to an Azure blob.
        /// </summary>
        /// <param name="container">The storage container to use.</param>
        /// <param name="image">The image to upload,</param>
        /// <param name="remoteFileName">The name the file should have in the storage container.</param>
        /// <param name="remoteFolder">A folder name to use or create in the storage container.</param>
        /// <returns><c>true</c> if the upload succeeds otherwise <c>false</c>.</returns>
        public static async Task<bool> UploadImageAsync(CloudBlobContainer container, Image image, string remoteFileName, string remoteFolder = null)
        {
            var name = remoteFileName;
            CheckUtil.ThrowIfNullOrEmpty(() => name);
            CheckUtil.ThrowIfNull(() => image);
            if (!remoteFolder.IsNullOrEmpty())
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                remoteFileName = Path.Combine(remoteFolder, remoteFileName);
            }
            var newBlob = container.GetBlockBlobReference(remoteFileName);
            newBlob.Properties.ContentType = image.GetMimeContentType();
            var bytes = image.GetByteArrayFromImage(image.RawFormat);
            await newBlob.UploadFromByteArrayAsync(bytes, 0, bytes.Length);
            return true;
        }

        /// <summary>
        /// Uploads a local file to a storage container.
        /// </summary>
        /// <param name="containerName">The name of the container inside the storage account.</param>
        /// <param name="file">The extended FileInfo instance.</param>
        /// <param name="accountConnectionString">
        /// The connection string to connect to Azure or <c>null</c> if logic should search in configuration for 
        /// an app-setting defined in <see cref="StorageConnectionStringKey"/>.
        /// </param>
        /// <param name="remoteFolder">A folder name to use or create in the storage container.</param>
        /// <param name="contentType">An optional content-type in MIME format to set for the new BLOB.</param>
        /// <returns><c>true</c> if the upload succeeds otherwise <c>false</c>.</returns>
        public static async Task<bool> UploadToAzureAsync(this FileInfo file, string containerName, string accountConnectionString = null, string remoteFolder = null, string contentType = null)
        {
            return await UploadFileAsync(containerName, file.FullName, accountConnectionString, file.Name, remoteFolder, contentType);
        }

        /// <summary>
        /// Uploads a local file to a storage container.
        /// </summary>
        /// <param name="container">The storage container to use.</param>
        /// <param name="file">The extended FileInfo instance.</param>
        /// <param name="remoteFolder">A folder name to use or create in the storage container.</param>
        /// <param name="contentType">An optional content-type in MIME format to set for the new BLOB.</param>
        /// <returns><c>true</c> if the upload succeeds otherwise <c>false</c>.</returns>
        public static async Task<bool> UploadToAzureAsync(this FileInfo file, CloudBlobContainer container, string remoteFolder = null, string contentType = null)
        {
            return await UploadFileAsync(container, file.FullName, file.Name, remoteFolder, contentType);
        }

        #endregion

        #region properties

        /// <summary>
        /// The key inside the calling app.config where the Azure storage connection
        /// string is defined. Default value is 'CloudStorageConnectionString'.
        /// </summary>
        public static string StorageConnectionStringKey { get; set; }

        /// <summary>
        /// The cancellation token source.
        /// </summary>
        private static CancellationTokenSource TokenSource => _tokenSource ?? (_tokenSource = new CancellationTokenSource());

        #endregion
    }
}
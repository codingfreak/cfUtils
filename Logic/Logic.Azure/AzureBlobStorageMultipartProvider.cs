namespace codingfreaks.cfUtils.Logic.Azure
{
    using System;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Base.Utilities;

    using Microsoft.WindowsAzure.Storage.Blob;

    /// <summary>
    /// Specialized provider which will store multipart-formdata in Azure storage accounts.
    /// </summary>
    public class AzureBlobStorageMultipartProvider : MultipartFileStreamProvider
    {
        #region member vars

        private CloudBlobContainer _container;

        #endregion

        #region constructors and destructors

        /// <summary>
        /// Initializes a new instance of the AzureBlobStorageMultipartProvider class.
        /// </summary>
        /// <param name="container">The cloud blob container to use.</param>
        /// <param name="rootPath">The root path where the content of MIME multipart body parts are written to.</param>
        public AzureBlobStorageMultipartProvider(CloudBlobContainer container, string rootPath) : base(rootPath)
        {
            Initialize(container);
        }

        /// <summary>
        /// Initializes a new instance of the AzureBlobStorageMultipartProvider class.
        /// </summary>
        /// <param name="container">The cloud blob container to use.</param>
        /// <param name="rootPath">The root path where the content of MIME multipart body parts are written to.</param>
        /// <param name="bufferSize">The number of bytes buffered for writes to the file.</param>
        public AzureBlobStorageMultipartProvider(CloudBlobContainer container, string rootPath, int bufferSize) : base(rootPath, bufferSize)
        {
            Initialize(container);
        }

        #endregion

        #region properties

        /// <summary>
        /// A collection of all blobs inside the current container.
        /// </summary>
        public Collection<AzureBlobInfo> AzureBlobs { get; private set; }

        #endregion

        #region methods

        /// <summary>
        /// Executes the post processing operation for this <see cref="T:System.Net.Http.MultipartStreamProvider"/>.
        /// </summary>
        /// <returns>
        /// The asynchronous task for this operation.
        /// </returns>
        public override async Task ExecutePostProcessingAsync()
        {
            // Upload the files asynchronously to azure blob storage and remove them from local disk when done
            foreach (var fileData in FileData)
            {
                // Get the blob name from the Content-Disposition header if present
                var blobName = GetBlobName(fileData);
                // Retrieve reference to a blob
                var blob = _container.GetBlockBlobReference(blobName);
                // Pick content type if present
                blob.Properties.ContentType = fileData.Headers.ContentType != null ? fileData.Headers.ContentType.ToString() : "application/octet-stream";
                // Upload content to blob storage
                using (var fStream = new FileStream(fileData.LocalFileName, FileMode.Open, FileAccess.Read, FileShare.Read, BufferSize, true))
                {
                    await Task.Factory.FromAsync(blob.BeginUploadFromStream, blob.EndUploadFromStream, fStream, null);
                }
                // Delete local file
                File.Delete(fileData.LocalFileName);
                AzureBlobs.Add(
                    new AzureBlobInfo
                    {
                        ContentType = blob.Properties.ContentType,
                        Name = blob.Name,
                        Size = blob.Properties.Length,
                        Location = blob.Uri.AbsoluteUri
                    });
            }

            await base.ExecutePostProcessingAsync();
        }

        /// <summary>
        /// Tries to extract the name of the blob out of the content disposition.
        /// </summary>
        /// <param name="fileData">A single element of the multipart form-data.</param>
        /// <returns>The file name or <c>null</c> if none was retrieved.</returns>
        private static string GetBlobName(MultipartFileData fileData)
        {
            string blobName = null;
            var contentDisposition = fileData.Headers.ContentDisposition;
            if (contentDisposition == null)
            {
                return Path.GetFileName(fileData.LocalFileName);
            }
            try
            {
                blobName = Path.GetFileName(contentDisposition.FileName.Trim('"'));
            }
            catch (Exception ex)
            {
                TraceUtil.WriteTraceError(ex.Message);
            }
            return blobName ?? Path.GetFileName(fileData.LocalFileName);
        }

        /// <summary>
        /// Is used to initialize cloud resources.
        /// </summary>
        /// <param name="container">The blob container to use.</param>
        private void Initialize(CloudBlobContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }
            _container = container;
            AzureBlobs = new Collection<AzureBlobInfo>();
        }

        #endregion
    }
}
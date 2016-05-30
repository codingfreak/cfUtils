namespace codingfreaks.cfUtils.Logic.Azure
{
    using System;

    using Microsoft.WindowsAzure.Storage.Blob;

    /// <summary>
    /// The <see cref="AzureBlobInfo" /> class contains information about the blobs found in
    /// the blob store.
    /// </summary>
    public class AzureBlobInfo
    {
        #region methods

        /// <summary>
        /// Creates a new instance from a ICloudBlob.
        /// </summary>
        /// <param name="original">The original cloud blob.</param>
        /// <returns></returns>
        public static AzureBlobInfo Create(ICloudBlob original)
        {
            var file = original as CloudBlockBlob;
            if (file != null)
            {
                var modified = DateTimeOffset.MinValue;
                var modifiedOk = false;
                if (file.Metadata.ContainsKey("CbModifiedTime"))
                {
                    modifiedOk = DateTimeOffset.TryParse(file.Metadata["CbModifiedTime"], out modified);
                }
                return new AzureBlobInfo
                {
                    BlobElement = file,
                    ContentType = file.Properties.ContentType,
                    Name = file.Name,
                    Size = file.Properties.Length,
                    Location = file.Uri.AbsoluteUri,
                    ModifiedTime = modifiedOk ? modified : default(DateTimeOffset?)
                };
            }
            var folder = original as CloudBlobDirectory;
            if (folder == null)
            {
                return null;
            }
            return new AzureBlobInfo
            {                
                Name = folder.Prefix,
                Location = folder.Uri.AbsoluteUri
            };
        }

        #endregion

        #region properties

        /// <summary>
        /// The content type of the blob.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// The path of the parent directory of this element or an empty string if it lays in the root.
        /// </summary>
        public string DirectoryPath
        {
            get
            {
                if (!Name.Contains("/"))
                {
                    return string.Empty;
                }
                var pos = Name.LastIndexOf("/", StringComparison.OrdinalIgnoreCase);
                return Name.Substring(0, pos);
            }
        }

        /// <summary>
        /// The BLOB itself.
        /// </summary>
        public CloudBlob BlobElement { get; set; }

        /// <summary>
        /// Indicates whether this is a folder.
        /// </summary>
        public bool IsDirectory { get; set; }

        /// <summary>
        /// The absolute URI by which the blob can be retrieved from the store.
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Modification timestamp in UTC.
        /// </summary>
        public DateTimeOffset? ModifiedTime { get; set; }

        /// <summary>
        /// The name without path of the file stored in the blob store.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The size of the blob.
        /// </summary>
        public long Size { get; set; }

        #endregion
    }
}
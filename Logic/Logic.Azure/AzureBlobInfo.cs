namespace s2.s2Utils.Logic.Azure
{
    /// <summary>
    /// The <see cref="AzureBlobInfo"/> class contains information about the blobs found in
    /// the blob store.
    /// </summary>
    public class AzureBlobInfo
    {
        #region properties

        /// <summary>
        /// The content type of the blob.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// The absolute URI by which the blob can be retrieved from the store.
        /// </summary>
        public string Location { get; set; }

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
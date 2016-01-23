namespace s2.s2Utils.Logic.WebUtils.Models
{
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Web;

    /// <summary>
    /// Provides HTTP content based on a given file on the local file system.
    /// </summary>
    /// <remarks>
    /// Remember to call <see cref="AddContentDisposition"/> before you pass this to an <see cref="MultipartFormDataContent"/>.
    /// </remarks>
    public class FileContent : ByteArrayContent
    {
        #region constructors and destructors

        /// <summary>
        /// Creates a new instance of the System.Net.Http.FileContent class.
        /// </summary>
        /// <param name="content">The binary content.</param>
        public FileContent(byte[] content) : base(content)
        {
        }

        /// <summary>
        /// Creates a new instance of the System.Net.Http.FileContent class.
        /// </summary>
        /// <param name="content">The binary content.</param>
        /// <param name="offset">The offset from which to read the <paramref name="content"/>.</param>
        /// <param name="count">The amount of bytes to take from <paramref name="offset"/> on.</param>
        public FileContent(byte[] content, int offset, int count) : base(content, offset, count)
        {
        }

        /// <summary>
        /// Creates a new instance of the System.Net.Http.FileContent class.
        /// </summary>
        /// <param name="fileName">The complete URI to the file locally.</param>
        /// <exception cref="InvalidOperationException"></exception>
        public FileContent(string fileName) : base(GetContentByteArray(fileName))
        {
            if (!CheckFile(fileName))
            {
                throw new InvalidOperationException("Checking the file failed.");
            }
            Headers.ContentType = new MediaTypeHeaderValue(MimeMapping.GetMimeMapping(fileName));
        }

        #endregion

        #region methods

        /// <summary>
        /// When called add an <see cref="ContentDispositionHeaderValue"/> using the given informations.
        /// </summary>
        /// <param name="name">The name of the disposition header.</param>
        /// <param name="fileName">The file name of the disposition header.</param>
        public void AddContentDisposition(string name, string fileName)
        {
            Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = name,
                FileName = fileName,
                FileNameStar = fileName
            };
        }

        /// <summary>
        /// Is used by the constructor to retrieve all bytes of the <paramref name="fileName"/>.
        /// </summary>
        /// <param name="fileName">The complete URI to the file locally.</param>
        /// <returns>The array of bytes or <c>null</c> if an error occurs.</returns>
        public static byte[] GetContentByteArray(string fileName)
        {
            if (!File.Exists(fileName))
            {
                return null;
            }
            try
            {
                return File.ReadAllBytes(fileName);
            }
            catch
            {
            }
            return null;
        }

        /// <summary>
        /// Is used to check existance and validity of a file.
        /// </summary>
        /// <param name="fileName">The complete URI to the file locally.</param>
        /// <returns><c>true</c> if the file is valid, otherwise <c>false</c>.</returns>
        protected virtual bool CheckFile(string fileName)
        {
            return File.Exists(fileName);
        }

        #endregion
    }
}
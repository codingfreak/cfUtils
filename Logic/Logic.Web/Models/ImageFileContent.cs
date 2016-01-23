namespace codingfreaks.cfUtils.Logic.WebUtils.Models
{
    using System;
    using System.Drawing;

    using Base.Utilities;

    /// <summary>
    /// Provides HTTP content based on a given image file on the local file system.
    /// </summary>
    public class ImageFileContent : FileContent
    {
        #region constructors and destructors

        /// <summary>
        /// Creates a new instance of the System.Net.Http.FileContent class.
        /// </summary>
        /// <param name="content">The binary content.</param>
        public ImageFileContent(byte[] content) : base(content)
        {
        }

        /// <summary>
        /// Creates a new instance of the System.Net.Http.FileContent class.
        /// </summary>
        /// <param name="content">The binary content.</param>
        /// <param name="offset">The offset from which to read the <paramref name="content"/>.</param>
        /// <param name="count">The amount of bytes to take from <paramref name="offset"/> on.</param>
        public ImageFileContent(byte[] content, int offset, int count) : base(content, offset, count)
        {
        }

        /// <summary>
        /// Creates a new instance of the System.Net.Http.FileContent class.
        /// </summary>
        /// <param name="fileName">The complete URI to the file locally.</param>
        /// <exception cref="InvalidOperationException"></exception>
        public ImageFileContent(string fileName) : base(fileName)
        {
        }

        #endregion

        #region methods

        /// <summary>
        /// Is used to check existance and validity of a file.
        /// </summary>
        /// <param name="fileName">The complete URI to the file locally.</param>
        /// <returns><c>true</c> if the file is valid, otherwise <c>false</c>.</returns>
        protected override bool CheckFile(string fileName)
        {
            if (!base.CheckFile(fileName))
            {
                return false;
            }
            try
            {
                var image = Image.FromFile(fileName);
                return true;
            }
            catch (Exception ex)
            {
                TraceUtil.WriteTraceError(ex.Message);
            }
            return false;
        }

        #endregion
    }
}
namespace codingfreaks.cfUtils.Logic.Core.Utilities
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Provides helper methods regarding files.
    /// </summary>
    public static class FileUtils
    {
        #region methods

        /// <summary>
        /// Uses <see cref="StreamReader" /> to automatically determine the encoding used for a given <paramref name="fileUri" />
        /// using the BOM.
        /// </summary>
        /// <param name="fileUri">The absolute path to the file.</param>
        /// <exception cref="ArgumentException">Is thrown if <paramref name="fileUri" /> is invalid.</exception>
        /// <exception cref="FileNotFoundException">
        /// Is thrown if the provided <paramref name="fileUri" /> is not found in file
        /// system.
        /// </exception>
        /// <exception cref="InvalidOperationException">Is thrown if any exception occurs during the operation.</exception>
        /// <returns>The detected encoding.</returns>
        public static Encoding GetEncoding(string fileUri)
        {
            CheckUtil.ThrowIfNullOrEmpty(() => fileUri);
            if (!File.Exists(fileUri))
            {
                throw new FileNotFoundException("Provided file not found.", fileUri);
            }
            try
            {
                using (var reader = new StreamReader(fileUri, true))
                {
                    return reader.CurrentEncoding;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Could not determine encoding. See inner exception for details.", ex);
            }
        }

        #endregion
    }
}
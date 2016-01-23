namespace codingfreaks.cfUtils.Logic.Base.Structures
{
    using Utilities;

    /// <summary>
    /// Defines informations on files read via <see cref="FtpUtil"/>.
    /// </summary>
    public class FtpFileInfo
    {
        #region constructors and destructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="fullName">The path to the file.</param>
        public FtpFileInfo(string fullName)
        {
            FullName = fullName;
        }

        #endregion

        #region properties

        /// <summary>
        /// The path to the file.
        /// </summary>
        public string FullName { get; }

        /// <summary>
        /// The name of the file without any path-informations.
        /// </summary>
        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(FullName) || !FullName.Contains("/") || FullName.EndsWith("/"))
                {
                    return FullName;
                }
                return FullName.Substring(FullName.LastIndexOf('/') + 1);
            }
        }

        #endregion
    }
}
namespace s2.s2Utils.Logic.Base.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Security;
    using System.Text;
    using System.Threading.Tasks;

    using Portable.Extensions;
    using Portable.Utilities;

    using Structures;

    /// <summary>
    /// Provides FTP functionallity.
    /// </summary>
    public class FtpUtil
    {
        #region member vars

        private bool _initialized;
        private SecureString _pass;
        private int _port;
        private string _server;
        private SecureString _user;

        #endregion

        #region methods

        /// <summary>
        /// Deletes a single file on the FTP server.
        /// </summary>
        /// <param name="remoteFolder">The folder on the FTP site.</param>
        /// <param name="remoteFilename">The file name on the FTP site.</param>
        /// <returns><c>true</c> if the operation succeeds, otherwise <c>false</c>.</returns>
        [Obsolete("This implementation should not be used anymore because it uses deprecated calls.")]
        public bool DeleteFile(string remoteFolder, string remoteFilename)
        {
            return AsyncUtil.CallSync(DeleteFileAsync, remoteFolder, remoteFilename);
        }

        /// <summary>
        /// Deletes a single file on the FTP server.
        /// </summary>
        /// <param name="remotefolder">The folder on the FTP site.</param>
        /// <param name="remoteFilename">The file name on the FTP site.</param>
        /// <returns><c>true</c> if the operation succeeds, otherwise <c>false</c>.</returns>
        public async Task<bool> DeleteFileAsync(string remotefolder, string remoteFilename)
        {
            EnsureInitialized();
            var request = GetRequest(WebRequestMethods.Ftp.DeleteFile, remotefolder, remoteFilename);
            using (var response = (FtpWebResponse)(await request.GetResponseAsync()))
            {
                LastStatusDescription = response.StatusDescription;
                return response.StatusCode == FtpStatusCode.FileActionOK;
            }
        }

        /// <summary>
        /// Downloads a file from FTP to a <paramref name="localPath"/>.
        /// </summary>
        /// <param name="remoteFolder">The folder on the FTP site.</param>
        /// <param name="remoteFilename">The file name on the FTP site.</param>
        /// <param name="localPath">The local directory to store the file in.</param>
        /// <returns><c>true</c> if the operation succeeds, otherwise <c>false</c>.</returns>
        [Obsolete("This implementation should not be used anymore because it uses deprecated calls.")]
        public bool DownloadFile(string remoteFolder, string remoteFilename, string localPath = "")
        {
            return AsyncUtil.CallSync(DownloadFileAsync, remoteFolder, remoteFilename, localPath);
        }

        /// <summary>
        /// Downloads a file from FTP to a <paramref name="localPath"/>.
        /// </summary>
        /// <param name="remotefolder">The folder on the FTP site.</param>
        /// <param name="remoteFilename">The file name on the FTP site.</param>
        /// <param name="localPath">The local directory to store the file in.</param>
        /// <returns><c>true</c> if the operation succeeds, otherwise <c>false</c>.</returns>
        public async Task<bool> DownloadFileAsync(string remotefolder, string remoteFilename, string localPath = "")
        {
            EnsureInitialized();
            var localInfo = new FileInfo(localPath);
            if (localInfo.Exists)
            {
                throw new InvalidOperationException("File already exists.");
            }
            var request = GetRequest(WebRequestMethods.Ftp.DownloadFile, remotefolder, remoteFilename);
            string content;
            bool result;
            using (var response = (FtpWebResponse)(await request.GetResponseAsync()))
            {
                using (var remoteReader = new StreamReader(response.GetResponseStream() ?? Stream.Null))
                {
                    content = await remoteReader.ReadToEndAsync();
                }
                LastStatusDescription = response.StatusDescription;
                result = response.StatusCode == FtpStatusCode.ClosingData;
            }
            if (content.IsNullOrEmpty())
            {
                return false;
            }
            try
            {
                File.WriteAllText(localPath, content);
            }
            catch
            {
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Has to be called before any other operation in this type.
        /// </summary>
        /// <param name="server">The server dns name without any prefix and port.</param>
        /// <param name="user">The user name.</param>
        /// <param name="pass">The password of the <paramref name="user"/>.</param>
        /// <param name="port">The port number (defaults to 21).</param>
        /// <returns></returns>
        [Obsolete("This implementation should not be used anymore because it uses deprecated calls.")]
        public bool Initialize(string server, string user, string pass, int port = 21)
        {
            return AsyncUtil.CallSync(InitializeAsync, server, user, pass, port);
        }

        /// <summary>
        /// Has to be called before any other operation in this type.
        /// </summary>
        /// <param name="server">The server dns name without any prefix and port.</param>
        /// <param name="user">The user name.</param>
        /// <param name="pass">The password of the <paramref name="user"/>.</param>
        /// <param name="port">The port number (defaults to 21).</param>
        /// <returns></returns>
        public Task<bool> InitializeAsync(string server, string user, string pass, int port = 21)
        {
            return Task.Run(
                () =>
                {
                    _initialized = false;
                    _server = server;
                    _user = user.ToSecureString();
                    _pass = pass.ToSecureString();
                    _port = port;
                    try
                    {
                        var request = GetRequest();
                        _initialized = request != null;
                    }
                    catch(Exception ex)
                    {
                        TraceUtil.WriteTraceError(ex.Message);
                    }
                    return _initialized;
                });
        }

        /// <summary>
        /// Retrieves a list of all files in a specific FTP-<paramref name="folder"/>.
        /// </summary>
        /// <param name="folder">The folder on the FTP site.</param>
        /// <returns>A list of file informations.</returns>
        [Obsolete("This implementation should not be used anymore because it uses deprecated calls.")]
        public IEnumerable<FtpFileInfo> ListFiles(string folder = "")
        {
            return AsyncUtil.CallSync(ListFilesAsync, folder);
        }

        /// <summary>
        /// Retrieves a list of all files in a specific FTP-<paramref name="folder"/>.
        /// </summary>
        /// <param name="folder">The folder on the FTP site.</param>
        /// <returns>A list of file informations.</returns>
        public async Task<IEnumerable<FtpFileInfo>> ListFilesAsync(string folder = "")
        {
            EnsureInitialized();
            var request = GetRequest(WebRequestMethods.Ftp.ListDirectory, folder);
            using (var response = (FtpWebResponse)(await request.GetResponseAsync()))
            {
                try
                {
                    var responseStream = response.GetResponseStream();
                    if (responseStream != null)
                    {
                        using (var reader = new StreamReader(responseStream))
                        {
                            var filesText = reader.ReadToEnd();
                            if (!filesText.IsNullOrEmpty())
                            {
                                var result = new List<FtpFileInfo>();
                                filesText.Split('\n').ToList().ForEach(name => result.Add(new FtpFileInfo(name.TrimEnd('\r'))));
                                return result;
                            }
                        }
                        responseStream.Dispose();
                    }
                }
                catch
                {
                }
            }
            return Enumerable.Empty<FtpFileInfo>();
        }

        /// <summary>
        /// Uploads a single file from the <paramref name="localPath"/> to a <paramref name="folder"/> on the FTP site.
        /// </summary>
        /// <param name="localPath">The complete URI to the local file.</param>
        /// <param name="folder">The folder on the FTP site.</param>
        /// <returns><c>true</c> if the operation succeeds, otherwise <c>false</c>.</returns>
        [Obsolete("This implementation should not be used anymore because it uses deprecated calls.")]
        public bool UploadFile(string localPath, string folder = "")
        {
            return AsyncUtil.CallSync(UploadFileAsync, localPath, folder);
        }

        /// <summary>
        /// Uploads a single file from the <paramref name="localPath"/> to a <paramref name="folder"/> on the FTP site.
        /// </summary>
        /// <param name="localPath">The complete URI to the local file.</param>
        /// <param name="folder">The folder on the FTP site.</param>
        /// <returns><c>true</c> if the operation succeeds, otherwise <c>false</c>.</returns>
        public async Task<bool> UploadFileAsync(string localPath, string folder = "")
        {
            EnsureInitialized();
            var localInfo = new FileInfo(localPath);
            if (!localInfo.Exists)
            {
                throw new FileNotFoundException("File not found.", localPath);
            }
            try
            {
                var request = GetRequest(WebRequestMethods.Ftp.UploadFile, folder, localInfo.Name);
                var content = File.ReadAllBytes(localInfo.FullName);
                request.ContentLength = content.Length;
                using (var stream = await request.GetRequestStreamAsync())
                {
                    stream.Write(content, 0, content.Length);
                }
                var response = (FtpWebResponse)(await request.GetResponseAsync());
                LastStatusDescription = response.StatusDescription;
                return response.StatusCode == FtpStatusCode.ClosingData;
            }
            catch
            {
            }
            return false;
        }

        /// <summary>
        /// Is used to ensure that Initialize was called before.
        /// </summary>
        private void EnsureInitialized()
        {
            if (!_initialized)
            {
                throw new InvalidOperationException("Instance not initialized. Call Initialize first!");
            }
        }

        /// <summary>
        /// Retrieves a ready-to-use ftp web request.
        /// </summary>
        /// <param name="method">The method to use on the request.</param>
        /// <param name="folder">An optional folder name including subfolders separated by /.</param>
        /// <param name="file">An optional file name on which to operate.</param>
        /// <returns>The ready-to-use request.</returns>
        private FtpWebRequest GetRequest(string method = WebRequestMethods.Ftp.ListDirectory, string folder = "", string file = "")
        {
            var builder = new StringBuilder("ftp://");
            builder.AppendFormat("{0}:{1}", _server, _port);
            if (!string.IsNullOrEmpty(folder))
            {
                builder.AppendFormat("/{0}", folder);
            }
            if (!string.IsNullOrEmpty(file))
            {
                builder.AppendFormat("/{0}", file);
            }
            var path = new Uri(builder.ToString());
#if TRACE
            Trace.TraceInformation("FTP-request to {0}", path);
#endif
            var request = (FtpWebRequest)WebRequest.Create(path);
            request.Credentials = new NetworkCredential(_user.ToUnsecureString(), _pass.ToUnsecureString());
            request.Method = method;
            return request;
        }

        #endregion

        #region properties

        /// <summary>
        /// The description of the last FTP status.
        /// </summary>
        public string LastStatusDescription { get; private set; }

        #endregion
    }
}
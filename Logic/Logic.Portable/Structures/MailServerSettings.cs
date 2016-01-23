namespace s2.s2Utils.Logic.Portable.Structures
{
    /// <summary>
    /// Model for settings for the mail server.
    /// </summary>
    public class MailServerSettings
    {
        #region properties

        /// <summary>
        /// The domain of the mail server.
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// The password for logins.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// The network port on which the mail service is available.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// The dmonain name of the server.
        /// </summary>
        public string ServerAddress { get; set; }

        /// <summary>
        /// If set tot <c>true</c> the windows logon will be used.
        /// </summary>        
        public bool UseDefaultCredentials { get; set; }
        
        /// <summary>
        /// The username of the mail server.
        /// </summary>
        public string Username { get; set; }

        #endregion
    }
}
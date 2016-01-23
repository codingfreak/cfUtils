namespace s2.s2Utils.Logic.Utils.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Mail;
    using System.Threading.Tasks;
    using System.Web.UI.WebControls;

    using Base.Utilities;

    using Enumerations;

    using Portable.Extensions;
    using Portable.Structures;
    using Portable.Utilities;

    /// <summary>
    /// Provides helper methods for mailing-features.
    /// </summary>
    public static class MailUtil
    {
        #region methods

        /// <summary>
        /// Retrieves a mail-definition which can be used to generate a HTML mail.
        /// </summary>
        /// <param name="fromAddress">Mail-address of the sender.</param>
        /// <param name="subject">Subject of the mail.</param>
        /// <returns>The defintion already set to HTML.</returns>
        public static MailDefinition GenerateHtmlMailDefinition(string fromAddress, string subject)
        {
            return new MailDefinition
            {
                From = fromAddress,
                Subject = subject,
                IsBodyHtml = true
            };
        }

        /// <summary>
        /// Openes a prepared mail-sending-window using the installed Outlook.
        /// </summary>
        /// <param name="senderAddress">The sender.</param>
        /// <param name="receipient">The receipient.</param>
        /// <param name="replyToAddress">Give in an SMTP mail if you want to reply to another mail.</param>
        /// <param name="subject">The subject of the mail.</param>
        /// <param name="body">The body-text.</param>
        /// <param name="bcc">One or many BCC receipients seprated by ";".</param>
        /// <param name="attachements">A list of strings representing file-URIs to attach.</param>
        /// <param name="format">The format of the body.</param>
        /// <returns><c>true</c>, if the process succeeded.</returns>
        public static bool OpenOutlookMail(
            string senderAddress,
            string receipient,
            string replyToAddress,
            string subject,
            string body = "",
            string bcc = "",
            IEnumerable<string> attachements = null,
            OutlookMailBodyFormat format = OutlookMailBodyFormat.Plain)
        {
            return OpenOutlookMail(senderAddress, new[] { receipient }, replyToAddress, subject, body, bcc, attachements, format);
        }

        /// <summary>
        /// Openes a prepared mail-sending-window using the installed Outlook.
        /// </summary>
        /// <param name="senderAddress">The sender.</param>
        /// <param name="receipients">The receipients.</param>
        /// <param name="replyToAddress">Give in an SMTP mail if you want to reply to another mail.</param>
        /// <param name="subject">The subject of the mail.</param>
        /// <param name="body">The body-text.</param>
        /// <param name="bcc">One or many BCC receipients seprated by ";".</param>
        /// <param name="attachements">A list of strings representing file-URIs to attach.</param>
        /// <param name="format">The format of the body.</param>
        /// <returns><c>true</c>, if the process succeeded.</returns>
        public static bool OpenOutlookMail(
            string senderAddress,
            IEnumerable<string> receipients,
            string replyToAddress,
            string subject,
            string body,
            string bcc = "",
            IEnumerable<string> attachements = null,
            OutlookMailBodyFormat format = OutlookMailBodyFormat.Plain)
        {
            var result = false;
            var outlookMailItem = GetMailItem(senderAddress, receipients, replyToAddress, subject, body, bcc, attachements, format);
            outlookMailItem.Save();
            try
            {
                outlookMailItem.Display(false);
                result = true;
            }
            catch (Exception ex)
            {
                TraceUtil.WriteTraceError(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Sends a RFC conform email using default .NET classes.
        /// </summary>
        /// <param name="fromAddress">Mail-address of the sender.</param>
        /// <param name="toAddress">Mail-address of the receiver.</param>
        /// <param name="subject">Subject of the mail.</param>
        /// <param name="body">The string for the body of the mail.</param>
        /// <param name="settings">A structure containing the settings for the server.</param>
        /// <returns><c>True</c> if the mail was sent, otherwise <c>false</c>.</returns>        
        public static bool SendMail(string fromAddress, string toAddress, string subject, string body, MailServerSettings settings)
        {
            return SendMail(fromAddress, new[] { toAddress }, subject, body, settings);
        }

        /// <summary>
        /// Sends a RFC conform email using default .NET classes.
        /// </summary>
        /// <param name="fromAddress">Mail-address of the sender.</param>
        /// <param name="toAddresses">Mail-addresses of the receivers.</param>
        /// <param name="subject">Subject of the mail.</param>
        /// <param name="body">The string for the body of the mail.</param>
        /// <param name="settings">A structure containing the settings for the server.</param>
        /// <returns><c>True</c> if the mail was sent, otherwise <c>false</c>.</returns>       
        public static bool SendMail(string fromAddress, string[] toAddresses, string subject, string body, MailServerSettings settings)
        {
            CheckUtil.ThrowIfNullOrWhitespace(() => subject);
            CheckUtil.ThrowIfNullOrWhitespace(() => body);
            var result = false;
            if (!fromAddress.IsValidEmailAddress())
            {
                throw new ArgumentException("Invalid e-mail-address of sender.", nameof(fromAddress));
            }
            if (!toAddresses.Any())
            {
                throw new ArgumentException("No receipient submitted!", nameof(toAddresses));
            }
            if (toAddresses.Any(a => !a.IsValidEmailAddress()))
            {
                throw new ArgumentException("Invalid e-mail-address of on of the recipient.", nameof(toAddresses));
            }
            // Send the mail and use credentials if given.
            using (var message = new MailMessage(fromAddress, toAddresses.First(), subject, body))
            {
                if (toAddresses.Count() > 1)
                {
                    // add the other receipients
                    for (var i = 1; i < toAddresses.Count(); i++)
                    {
                        message.To.Add(toAddresses[i]);
                    }
                }
                result = SendMail(message, settings);
            }
            return result;
        }

        /// <summary>
        /// Sends a RFC conform email using default .NET classes.
        /// </summary>
        /// <param name="messageToSend">The pre-configured <see cref="MailMessage" /> to send.</param>
        /// <param name="settings">A structure containing the settings for the server.</param>
        /// <returns><c>True</c> if the mail was sent, otherwise <c>false</c>.</returns>
        public static bool SendMail(MailMessage messageToSend, MailServerSettings settings)
        {
            return AsyncUtil.CallSync(() => SendMailAsync(messageToSend, settings));
        }

        /// <summary>
        /// Sends a RFC conform email using default .NET classes.
        /// </summary>
        /// <param name="messageToSend">The pre-configured <see cref="MailMessage" /> to send.</param>
        /// <param name="settings">A structure containing the settings for the server.</param>
        /// <returns><c>True</c> if the mail was sent, otherwise <c>false</c>.</returns>
        public static async Task<bool> SendMailAsync(MailMessage messageToSend, MailServerSettings settings)
        {
            CheckUtil.ThrowIfNull(() => messageToSend);
            try
            {
                // Send the mail and use credentials if given.
                using (var client = new SmtpClient(settings.ServerAddress, settings.Port))
                {
                    if (settings.UseDefaultCredentials)
                    {
                        client.Credentials = CredentialCache.DefaultNetworkCredentials;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(settings.Username) && !string.IsNullOrEmpty(settings.Password))
                        {
                            client.Credentials = new NetworkCredential(settings.Username, settings.Password, settings.Domain ?? string.Empty);
                        }
                    }
                    await client.SendMailAsync(messageToSend);
                }
                return true;
            }
            catch (Exception ex)
            {
                var error = string.Format(
                    CultureInfo.InvariantCulture,
                    "Cannot send e-mail from '{0}' to '{1}' with subject '{2}': {3}",
                    messageToSend.From,
                    messageToSend.To,
                    messageToSend.Subject,
                    ex);
                var file = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments), "mail.error");
                using (var writer = File.AppendText(file))
                {
                    writer.Write(error);
                    writer.Close();
                }
                throw new InvalidOperationException(error, ex);
            }
        }

        /// <summary>
        /// Sends an email using the installed Outlook.
        /// </summary>
        /// <param name="senderAddress">The sender.</param>
        /// <param name="receipient">The receipient.</param>
        /// <param name="replyToAddress">Give in an SMTP mail if you want to reply to another mail.</param>
        /// <param name="subject">The subject of the mail.</param>
        /// <param name="body">The body-text.</param>
        /// <param name="bcc">One or many BCC receipients seprated by ";".</param>
        /// <param name="attachements">A list of strings representing file-URIs to attach.</param>
        /// <param name="format">The format of the body.</param>
        /// <returns><c>true</c>, if the process succeeded.</returns>
        public static bool SendOutlookMail(
            string senderAddress,
            string receipient,
            string replyToAddress,
            string subject,
            string body = "",
            string bcc = "",
            IEnumerable<string> attachements = null,
            OutlookMailBodyFormat format = OutlookMailBodyFormat.Unspecified)
        {
            return SendOutlookMail(senderAddress, new[] { receipient }, replyToAddress, subject, body, bcc, attachements, format);
        }

        /// <summary>
        /// Sends an email using the installed Outlook.
        /// </summary>
        /// <param name="senderAddress">The sender.</param>
        /// <param name="receipients">The receipients.</param>
        /// <param name="replyToAddress">Give in an SMTP mail if you want to reply to another mail.</param>
        /// <param name="subject">The subject of the mail.</param>
        /// <param name="body">The body-text.</param>
        /// <param name="bcc">One or many BCC receipients seprated by ";".</param>
        /// <param name="attachements">A list of strings representing file-URIs to attach.</param>
        /// <param name="format">The format of the body.</param>
        /// <returns><c>true</c>, if the process succeeded.</returns>
        public static bool SendOutlookMail(
            string senderAddress,
            IEnumerable<string> receipients,
            string replyToAddress,
            string subject,
            string body,
            string bcc = "",
            IEnumerable<string> attachements = null,
            OutlookMailBodyFormat format = OutlookMailBodyFormat.Plain)
        {
            var result = false;
            var outlookMailItem = GetMailItem(senderAddress, receipients, replyToAddress, subject, body, bcc, attachements, format);
            outlookMailItem.Save();
            try
            {
                outlookMailItem.Send();
                result = true;
            }
            catch (Exception ex)
            {
                TraceUtil.WriteTraceError(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Generates an OutlookMailItem using dynamic types independently from the locally installed Office-Version.
        /// </summary>
        /// <param name="senderAddress">The sender.</param>
        /// <param name="receipients">The receipients.</param>
        /// <param name="replyToAddress">Give in an SMTP mail if you want to reply to another mail.</param>
        /// <param name="subject">The subject of the mail.</param>
        /// <param name="body">The body-text.</param>
        /// <param name="bcc">One or many BCC receipients seprated by ";".</param>
        /// <param name="attachements">A list of strings representing file-URIs to attach.</param>
        /// <param name="format">The format of the body.</param>
        /// <returns>The ready-to-use mail item.</returns>
        private static dynamic GetMailItem(
            string senderAddress,
            IEnumerable<string> receipients,
            string replyToAddress,
            string subject,
            string body,
            string bcc = "",
            IEnumerable<string> attachements = null,
            OutlookMailBodyFormat format = OutlookMailBodyFormat.Plain)
        {
            var outlookApplicationType = Type.GetTypeFromProgID("Outlook.Application");
            if (outlookApplicationType == null)
            {
                throw new InvalidOperationException("Outlook not installed on this machine.");
            }
            dynamic outlookApplication = Activator.CreateInstance(outlookApplicationType);
            dynamic outlookMailItem = outlookApplication.CreateItem(0);
            outlookMailItem.Subject = subject;
            outlookMailItem.Body = body;
            outlookMailItem.To = receipients.ToArray();
            outlookMailItem.Subject = subject;
            outlookMailItem.BodyFormat = (int)format;
            outlookMailItem.BCC = bcc;
            if (!string.IsNullOrEmpty(replyToAddress))
            {
                // we have to add a reply-address
                outlookMailItem.ReplyRecipients.Add(replyToAddress);
            }
            if (attachements != null && attachements.Any())
            {
                // add the attachements
                foreach (var strAtt in attachements.Where(strAtt => !string.IsNullOrEmpty(strAtt) && File.Exists(strAtt)))
                {
                    outlookMailItem.Attachments.Add(strAtt, 5, 1, string.Empty);
                }
            }
            return outlookMailItem;
        }

        #endregion
    }
}
namespace codingfreaks.cfUtils.Ui.TestConsole
{
    using Logic.Azure;
    using Logic.Base.Utilities;
    using Microsoft.WindowsAzure.Storage.Blob;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    using System.Linq;
    using System.Net;
    using System.Net.Mail;
    using System.Runtime.Remoting.Messaging;

    using Logic.Portable.Structures;
    using Logic.Portable.Utilities;
    using Logic.Utils.Utilities;

    class Program
    {
        #region methods

        static void Main(string[] args)
        {
            //TestMail();
            TestAzureToken();
            Console.WriteLine("Fertig");
            Console.ReadKey();
        }

        private static void TestAzureToken()
        {
            var token = TokenUtil.RetrieveTokenAsync().Result;
            var headers = new Dictionary<string, string>
                {
                    { HttpRequestHeader.Authorization.ToString(), "Bearer " + token }
                };
            var client = JsonApiClient.GetInstance(headers, new Uri("https://manage.office.com/api/v1.0/18ca94d4-b294-485e-b973-27ef77addb3e/activity/feed/"));            
            var result = client.GetWithResultAsync<IEnumerable<dynamic>>("subscriptions/list").Result;
            Console.WriteLine(result);
        }

        private static void TestMail()
        {
            var from = "";
            var to = "";
            var user = "";
            var pass = "";
            var domain = "";           
            var message = new MailMessage()
            {
                Body = "Testmail",
                Subject = "Test"
            };
            message.To.Add(to);
            message.From = new MailAddress(from);
            MailUtil.SendMail(message, new MailServerSettings()
            {
                Domain = domain,
                Password = pass,
                ServerAddress = "smtp.office365.com",
                Username = user,
                Port = 587,
                UseDefaultCredentials = false,
                UseSsl = true
            });            
        }

        #endregion
    }
}
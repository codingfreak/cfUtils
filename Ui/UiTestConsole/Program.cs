namespace s2.s2Utils.Ui.TestConsole
{
    using System;
    using System.Data.Entity;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Logic.Base.Interfaces;
    using Logic.Base.Structures;
    using Logic.Tests;
    using Logic.Utils.Interfaces;

    using s2.s2Utils.Logic.Azure;
    using s2.s2Utils.Logic.Base.Utilities;
    using s2.s2Utils.Logic.Screenshot;
    using s2.s2Utils.Logic.Utils.Enumerations;
    using s2.s2Utils.Logic.Utils.Utilities;
    using s2.s2Utils.Logic.Utils.Extensions;

    class Program
    {
        static void Main(string[] args)
        {
            Utils.Init(null, new ContextResolver());
            //TestFtp().Wait(); 
            //TestScreenshot();
            //TestMail();
            //TestBaseUtil();
            //TestContextSwitch();
            //TestCsv();
            TestClearTable();
            Console.WriteLine("Fertig");
            Console.ReadKey();
        }

        private static void TestClearTable()
        {
            var table = StorageHelper.GetTableReference(
                "WADLogsTable",
                "DefaultEndpointsProtocol=https;AccountName=cybertradingdevworker;AccountKey=Y8q9QQ9cznnnaQwr3Wnkm3mobfSsjQ+biTPbMOAw/KVld/lzaG7mZV/pElrcTcr0FVNKiWX5TcfNZtXTasIeRA==");
            if (table == null)
            {
                Console.WriteLine("Error retrieving cloud table.");
            }
            var complete = 0L;
            var watch = new Stopwatch();
            watch.Start();
            StorageHelper.TableItemsRemoved += (s, e) =>
            {
                complete += e.Amount;
                Console.SetCursorPosition(0, 1);
                Console.Write("StorageHelper deleted 100 items. {0} deleted in sum{1}", complete, new string(' ', 30));
                Console.SetCursorPosition(0, 3);
                Console.Write($"{complete / watch.Elapsed.TotalSeconds} items/s{new string(' ', 80)}");
            };
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;
            Task.Run(
                async () =>
                {
                    while (!token.IsCancellationRequested)
                    {
                        Console.SetCursorPosition(0, 2);
                        Console.Write("{0}{1}",watch.Elapsed, new string(' ', 80));
                        await Task.Delay(800);
                    }
                });
            Console.WriteLine("Clearing table.");
            table.ClearAsync().Wait();
            tokenSource.Cancel();
            Console.SetCursorPosition(0, 10);
            Console.WriteLine("Cleared table.");
        }

        private static void TestCsv()
        {
            Console.WriteLine(CsvUtil.IsValidCsvFile(@"d:\test.csv", true, ';'));
        }

        private static void TestContextSwitch()
        {
            var ctx1 = ContextUtil.Context;
            var ctx2 = ContextUtil.Context;
            var entity = ctx1.People.First();
            entity.Firstname = "Honk";
            Console.WriteLine(ctx2.TryAttachEntity(entity));
            ctx2.Dispose();
            ctx1.Dispose();
        }

        private static void TestBaseUtil()
        {
            var util = Utils.GetDirect<PersonUtil>();            
            var request = new PagedRequest()
            {
                ItemsPerPage = 10,
                PageToDeliver = 1
            };
            var result = util.GetBasePagedResultAsync(request, null, null, p => p.Firstname.Contains("Peter")).Result;
            Console.WriteLine(result == null);
        }

        private static void TestMail()
        {
            MailUtil.OpenOutlookMail("test@test.de", "test2@test.de", "test@test.de", "Test", string.Empty, string.Empty, new[] { "c:\\temp\\ablabla.txt" });
        }

        private static void TestScreenshot()
        {
            Screenshot.ForceMdiCapturing = true;
            var result = Screenshot.GetAllWindows(true, true).Where(s => s.Image != null); 
            Console.WriteLine(result.Count());
        }

        private static async Task TestFtp()
        {
            var ftp = new FtpUtil();
            var ok = await ftp.InitializeAsync("codingfreaks.de", "w00acea9", "b7qyh3je");
            if (!ok)
            {
                Console.WriteLine("Connection failed.");
                return;
            }
            Console.WriteLine("Connected");
            var uploadSuccesful = await ftp.UploadFileAsync(@"C:\temp\wordpress.txt", "files");
            Console.WriteLine(ftp.LastStatusDescription + " - " + uploadSuccesful);
            Console.ReadKey();

            const string LocalPath = @"C:\temp\ablabla.txt";
            System.IO.File.Delete(LocalPath);
            var downloadSuccessful = await ftp.DownloadFileAsync("files", "wordpress.txt", LocalPath);
            Console.WriteLine(ftp.LastStatusDescription + " - " + System.IO.File.Exists(LocalPath) + " & " + downloadSuccessful);
            Console.ReadKey();

            var deleteSuccesful = await ftp.DeleteFileAsync("files", "wordpress.txt");
            Console.WriteLine(ftp.LastStatusDescription + " - " + deleteSuccesful);
            Console.ReadKey();

            var list = await ftp.ListFilesAsync("files");
            list.ToList().ForEach(i => Console.WriteLine(i.Name));            
        }
       
    }
}

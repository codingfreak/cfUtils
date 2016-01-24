namespace codingfreaks.cfUtils.Ui.TestConsole
{
    using System;

    using codingfreaks.cfUtils.Logic.Azure;

    using System.Linq;
    using System.Threading;

    using codingfreaks.cfUtils.Logic.Tests;
    using codingfreaks.cfUtils.Logic.Utils.Utilities;

    class Program
    {
        #region methods

        static void Main(string[] args)
        {
            Utils.Init(null, new ContextResolver());
            TestAzureTableMonitoring();
            Console.WriteLine("Fertig");
            Console.ReadKey();
        }

        private static void TestAzureTableMonitoring()
        {
            var table = StorageHelper.GetTableReference(
                "WADLogsTable",
                "DefaultEndpointsProtocol=https;AccountName=cybertradingdevworker;AccountKey=Y8q9QQ9cznnnaQwr3Wnkm3mobfSsjQ+biTPbMOAw/KVld/lzaG7mZV/pElrcTcr0FVNKiWX5TcfNZtXTasIeRA==");
            WadLogTableHelper.EntriesReceived += (s, e) =>
            {
                e.Entries.ToList().ForEach(ent => Console.WriteLine(ent.Message));
            };
            var tokenSource = new CancellationTokenSource(TimeSpan.FromMinutes(1));
            var token = tokenSource.Token;
            table.MonitorTableAsync(token).Wait(token);
        }

        #endregion
    }
}
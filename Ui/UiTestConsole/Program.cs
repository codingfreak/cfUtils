namespace codingfreaks.cfUtils.Ui.TestConsole
{
    using System;
    using System.Diagnostics;

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
            //TestAzureTableMonitoring();
            TestClearAzureTable();
            Console.WriteLine("Fertig");
            Console.ReadKey();
        }

        private static void TestAzureTableMonitoring()
        {
            var table = StorageHelper.GetTableReference(
                "WADLogsTable",
                "DefaultEndpointsProtocol=https;AccountName=cybertradingshared;AccountKey=TSVajlX7b2m7xISNQqx2DrpIjsFR2YpghKJGLkCxk7HvvB3YCpGjpEQwGpqC+I45uC3q+YkrubEsMtxpN+kb/Q==");
            var helper = new TableHelper<WadLogEntity>();
            helper.MonitoringReceivedNewEntries += (s, e) =>
            {
                e.Entries.ToList().ForEach(ent => Console.WriteLine("{0} | {1}", ent.PreciseTimeStamp, ent.MessageCleaned));
            };
            helper.StartMonitoringTable(table, 5, TimeSpan.FromDays(2).TotalSeconds);
            Console.ReadKey();
            helper.StopMonitoringTable();
        }

        private static void TestClearAzureTable()
        {
            var table = StorageHelper.GetTableReference(
                "WADLogsTable",
                "DefaultEndpointsProtocol=https;AccountName=cybertradingshared;AccountKey=TSVajlX7b2m7xISNQqx2DrpIjsFR2YpghKJGLkCxk7HvvB3YCpGjpEQwGpqC+I45uC3q+YkrubEsMtxpN+kb/Q==");
            var removed = 0L;        
            var timer = new Stopwatch();              
            StorageHelper.TableItemsRemoved += (s, e) =>
            {
                removed += e.Amount;
                Console.SetCursorPosition(0,0);
                Console.Write("{0}                                     ", removed);
                Console.SetCursorPosition(0, 1);
                Console.Write("{0}                                     ", removed / timer.Elapsed.TotalSeconds);
            };
            timer.Start();
            table.ClearAsync().Wait();
            timer.Stop();
        }

        #endregion
    }
}
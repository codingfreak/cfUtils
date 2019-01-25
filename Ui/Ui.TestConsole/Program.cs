namespace codingfreaks.cfUtils.Ui.TestConsole
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;

    using Logic.Csv;

    internal class Program
    {
        #region methods

        private static object ConsoleLock = new object();

        private static async Task Main(string[] args)
        {
            await TestCsvImporterAsync();
            Console.WriteLine("Done!");
            Console.ReadKey();
        }

        private static async Task TestCsvImporterAsync()
        {
            var lastPercentage = 0;
            var options = new ImporterOptions
            {
                AutoDetectEncoding = true,
                Culture = new CultureInfo("de-DE"),                
                Logger = Console.WriteLine,
            };
            var importer = new Importer<CsvImporterSample>(options);
            var progress = new Progress<OperationProgress>(p =>
            {
                if (!(p.Percentage > lastPercentage))
                {
                    return;
                }
                lock (ConsoleLock)
                {
                    var lineBefore = Console.CursorTop;
                    Console.SetCursorPosition(1,Console.WindowHeight - 1);
                    Console.Write(new string(' ', Console.WindowWidth - 1));
                    Console.Write($"{p.Percentage}% done");
                    Console.SetCursorPosition(0, lineBefore);
                }                
                lastPercentage = p.Percentage.Value;
            });
            importer.ItemImported += (s, e) =>
            {

            };            
            var result = await importer.ImportAsync(@"C:\Users\schmidt\Desktop\sample\20190120_Playback-Echtdaten_01.csv", progress);
            Console.Clear();
            Console.WriteLine(result.ItemsCount);
            Console.WriteLine(result.Finished - result.Started);
        }

        #endregion
    }
}
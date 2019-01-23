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

        private static async Task Main(string[] args)
        {
            await TestCsvImporterAsync();
            Console.WriteLine("Done!");
            Console.ReadKey();
        }

        private static async Task TestCsvImporterAsync()
        {
            var lastPerc = 0;
            var options = new ImporterOptions
            {
                AutoDetectEncoding = true,
                Delimiter = ';',
                Logger = m => Console.WriteLine(m),
                FirstReadedLineContainsHeader = true,
                Culture = new CultureInfo("de-DE"),
                CheckFileBeforeImport = true,
                MaxDegreeOfParallelism = (uint)Environment.ProcessorCount
            };
            var importer = new Importer<CsvImporterSample>(options);
            var progress = new Progress<OperationProgress>(p =>
            {
                if (p.Percentage > lastPerc)
                {
                    Console.WriteLine(p.Percentage);
                    lastPerc = p.Percentage.Value;
                }
            });
            importer.ItemImported += (s, e) =>
            {

            };
            var result = await importer.ImportAsync(@"C:\Users\alexanderschmidt\Desktop\samples\20190120_Playback-Echtdaten_01.csv", progress);
            Console.WriteLine(result.ItemsCount);
            Console.WriteLine(result.Finished - result.Started);
        }

        #endregion
    }
}
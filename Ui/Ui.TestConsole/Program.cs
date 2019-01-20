namespace codingfreaks.cfUtils.Ui.TestConsole
{
    using System;
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
            var options = new ImporterOptions
            {
                AutoDetectEncoding = true,
                Delimiter = ';',
                Logger = m => Console.WriteLine(m),
                FirstReadedLineContainsHeader = true
            };
            var importer = new Importer<CsvImporterSample>(options);
            var progress = new Progress<OperationProgress>(p => Console.WriteLine(p.CurrentLine));
            var result = await importer.ImportAsync(@"C:\Users\schmidt\Desktop\sample\20190120_Playback-Echtdaten_01.csv", progress);
            Console.WriteLine(result.ItemsCount);            
        }

        #endregion
    }
}
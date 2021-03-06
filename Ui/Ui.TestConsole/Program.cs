﻿namespace codingfreaks.cfUtils.Ui.TestConsole
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
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

        private static readonly string SourceFileUri = @"C:\Users\schmidt\Desktop\samples\D000000.csv";

        /// <summary>
        /// Tests CSV import using logic from the NuGet package 'codingfreaks.libs.Csv'.
        /// </summary>
        /// <remarks>
        /// Uses a BMA import file <see cref="SourceFileUri"/>.
        /// </remarks>        
        private static async Task TestCsvImporterAsync()
        {
            var lastPercentage = 0;
            // prepare import options
            var options = new ImporterOptions
            {
                AutoDetectEncoding = true,
                Culture = new CultureInfo("de-DE"),                
                Logger = Console.WriteLine,
                ItemsPerWorker = 500,
                LogMappingPerformance = true
            };
            // get a new importer
            var importer = new Importer<CsvImporterSample>(options);
            // define a progress to visualize it later in the console
            var progress = new Progress<OperationProgress>(p =>
            {
                if (p.Percentage % 10 == 0 && p.Percentage > lastPercentage)
                {
                    Interlocked.Exchange(ref lastPercentage, p.Percentage.Value);
                    Console.WriteLine($"{p.Percentage}% imported");
                }
            });
            importer.ItemImported += (s, e) =>
            {

            };    
            // run the importer and wait for it to finish
            var result = await importer.ImportAsync(SourceFileUri, progress);
            // handle results
            Console.Clear();
            if (result.Failed)
            {

            }
            Console.WriteLine(result.ItemsCount);
            Console.WriteLine(result.Finished - result.Started);
            var resultItems = result.Items.ToList();
            Console.WriteLine(resultItems.Count);
        }

        #endregion
    }
}
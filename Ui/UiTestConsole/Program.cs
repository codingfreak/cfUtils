namespace codingfreaks.cfUtils.Ui.TestConsole
{
    using Logic.Azure;
    using Logic.Base.Utilities;
    using Microsoft.WindowsAzure.Storage.Blob;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    using System.Linq;
    using System.Runtime.Remoting.Messaging;

    class Program
    {
        #region methods

        static void Main(string[] args)
        {
            TestStorageSync();
            Console.WriteLine("Fertig");
            Console.ReadKey();
        }

        private static void TestStorageSync()
        {
            var container = StorageHelper.GetContainerAsync(ConfigurationUtil.Get<string>("CloudContainer")).Result;
            var elements = StorageHelper.GetElementsAsync(container, null, true).Result;
            foreach(var ele in elements)
            {
                Console.WriteLine("{0} {1}", ele.DirectoryPath, ele.Name);                
            }
        }

       

        #endregion
    }
}
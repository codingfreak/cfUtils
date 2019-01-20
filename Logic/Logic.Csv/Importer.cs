using System;
using System.Collections.Generic;
using System.Text;

namespace codingfreaks.cfUtils.Logic.Csv
{
    public class Importer
    {

        public Importer(ImporterOptions options)
        {            
            Options = options;
        }

        public ImporterOptions Options { get; set; }

    }
}

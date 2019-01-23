using System;
using System.Collections.Generic;
using System.Text;

namespace codingfreaks.cfUtils.Logic.Csv
{
    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoreAttribute : Attribute
    {
        public IgnoreAttribute(bool ignoreOnImport = true, bool ignoreOnExport = true)
        {
            IgnoreOnImport = ignoreOnImport;
            IgnoreOnExport = ignoreOnExport;
        }

        public bool IgnoreOnImport { get; set; }

        public bool IgnoreOnExport { get; set; }

    }
}

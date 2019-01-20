using System;
using System.Collections.Generic;
using System.Text;

namespace codingfreaks.cfUtils.Logic.Csv
{
    [AttributeUsage(AttributeTargets.Property)]
    public class PropertyAttribute : Attribute
    {
        /// <summary>
        /// Inializes a property which field is identified by the name passed in the header of the data file.
        /// </summary>
        /// <param name="fieldName"></param>
        public PropertyAttribute(string fieldName)
        {
            FieldName = fieldName;
        }

        /// <summary>
        /// Initializes a property which field is identified by the position in the data row.
        /// </summary>
        /// <param name="offset">The 0-based position in the data.</param>
        public PropertyAttribute(int offset)
        {
            Offset = offset;
        }

        /// <summary>
        /// 
        /// </summary>
        public string FieldName { get; }

        public int? Offset { get; set; }

        public bool IgnoreOnImport { get; set; }

        public bool IgnoreOnExport { get; set; }
        

    }
}

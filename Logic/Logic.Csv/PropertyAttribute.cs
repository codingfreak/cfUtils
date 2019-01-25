namespace codingfreaks.cfUtils.Logic.Csv
{
    using System;
    using System.Linq;

    /// <summary>
    /// Is used to decorate a .NET property in order to map a CSV <see cref="FieldName" /> or <see cref="Offset" /> to it.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class PropertyAttribute : Attribute
    {
        #region constructors and destructors

        /// <summary>
        /// Initializes a property which field is identified by the name passed in the header of the data file.
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

        #endregion

        #region properties

        /// <summary>
        /// The name of the field passed in the header of the CSV file.
        /// </summary>
        public string FieldName { get; }

        /// <summary>
        /// The 0-based offset of the field.
        /// </summary>
        public int? Offset { get; set; }

        #endregion
    }
}
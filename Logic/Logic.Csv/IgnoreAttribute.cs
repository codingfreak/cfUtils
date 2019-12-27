namespace codingfreaks.cfUtils.Logic.Csv
{
    using System;
    using System.Linq;

    /// <summary>
    /// Can be applied to a property in order to exclude it from import or export.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoreAttribute : Attribute
    {
        #region constructors and destructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="ignoreOnImport">Defines if this property is ignored during import.</param>
        /// <param name="ignoreOnExport">Defines if this property is ignored during export.</param>
        public IgnoreAttribute(bool ignoreOnImport = true, bool ignoreOnExport = true)
        {
            IgnoreOnImport = ignoreOnImport;
            IgnoreOnExport = ignoreOnExport;
        }

        #endregion

        #region properties

        /// <summary>
        /// Defines if this property is ignored during export.
        /// </summary>
        public bool IgnoreOnExport { get; set; }

        /// <summary>
        /// Defines if this property is ignored during import.
        /// </summary>
        public bool IgnoreOnImport { get; set; }

        #endregion
    }
}
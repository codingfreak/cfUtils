namespace s2.s2Utils.Logic.Base.Structures
{
    /// <summary>
    /// Contains informations on a single command line argument for a program.
    /// </summary>
    public class CommandlineArgumentInfo
    {
        #region properties

        /// <summary>
        /// The long name for the argument.
        /// </summary>
        public string ArgumentName { get; set; }

        /// <summary>
        /// The short name for the argument.
        /// </summary>
        public string Abbreviation { get; set; }

        /// <summary>
        /// The description for the argument.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// A longer description used when help is requested.
        /// </summary>
        public string DescriptionLong { get; set; }

        /// <summary>
        /// Some sample data for the help display.
        /// </summary>
        public string SampleValue { get; set; }

        /// <summary>
        /// A flag indicating, if this argument has to be set.
        /// </summary>
        public bool IsMandatory { get; set; }

        /// <summary>
        /// A number indicating the position of the argument if it is
        /// mandatory.
        /// </summary>
        public int OrderPosition { get; set; }

        /// <summary>
        /// Indicates, if the value have to be a number.
        /// </summary>
        public bool IsNumeric { get; set; }

        /// <summary>
        /// Indicitaes is the the value has to be boolean.
        /// </summary>
        public bool IsBool { get; set; }

        /// <summary>
        /// Indicates whether the value should be a URI.
        /// </summary>
        public bool IsUri { get; set; }

        /// <summary>
        /// Indicates whether this argument is a simple flag und thus
        /// won't take a value.
        /// </summary>
        public bool IsFlag { get; set; }

        /// <summary>
        /// The value given to the app.
        /// </summary>
        public string GivenValue { get; set; }

        /// <summary>
        /// The default value, if no given value is set.
        /// </summary>
        public string DefaultValue { get; set; }

        /// <summary>
        /// Returns either the given value (if set) or the default value.
        /// </summary>
        public string ResolvedValue
        {
            get { return string.IsNullOrEmpty(GivenValue) ? DefaultValue : GivenValue; }
        }

        /// <summary>
        /// Defines if a value can have multiple elements separated by ','.
        /// </summary>
        public bool IsCommaSeparated { get; set; }

        #endregion
    }
}
namespace s2.s2Utils.Logic.Base.Structures
{
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// A wrapper class to hold informations about an application together in one place.
    /// </summary>
    public class ApplicationInfo
    {
        #region properties
        
        /// <summary>
        /// The binary informations on the assembly.
        /// </summary>
        public Assembly AssemblyInfo { get; set; }

        /// <summary>
        /// A list of command line arguments this application understands.
        /// </summary>
        public List<CommandlineArgumentInfo> CommandlineArgumentInfos { get; set; }

        /// <summary>
        /// An option setting a char which is expected as the delimiter between a command line parameter-name
        /// and it's value.
        /// </summary>
        public char ParameterDelimiter { get; set; }

        /// <summary>
        /// An option setting a char which can be used to prefix command line parameter-names.
        /// </summary>
        public char ParameterPraefix { get; set; }

        #endregion
    }
}
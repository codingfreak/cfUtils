using System;
using System.Linq;

namespace codingfreaks.cfUtils.Logic.Standard.Attributes
{
    using System;

    /// <summary>
    /// A special attribute to put over enum-values for supporting multi-language enum-values using
    /// the localized methods in <see cref="EnumUtil"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class LocalizedEnumMemberAttribute : Attribute
    {
        #region constructors and destructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="resourceKey">The unique of the text in the resources.</param>
        /// <param name="resourceType">The type of the resources where to search for.</param>
        public LocalizedEnumMemberAttribute(string resourceKey, int resourceType)
        {
            ResourceKey = resourceKey;
            ResourceType = resourceType;
        }

        #endregion

        #region properties

        /// <summary>
        /// The unique of the text in the resources.
        /// </summary>
        public string ResourceKey { get; private set; }

        /// <summary>
        /// The type of the resources where to search for.
        /// </summary>
        public int ResourceType { get; private set; }

        #endregion
    }
}
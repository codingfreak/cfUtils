namespace codingfreaks.cfUtils.Logic.Standard.Structures
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    /// <summary>
    /// Wrapper for informations on months.
    /// </summary>
    public class MonthInfo
    {
        #region constructors and destructors

        /// <summary>
        /// Default constructor for this type.
        /// </summary>
        /// <param name="monthOffset">The index of the month between 1 and 12.</param>
        /// <param name="culture">The culture to use.</param>
        public MonthInfo(uint monthOffset, CultureInfo culture = null)
        {
            if (monthOffset > 12 || monthOffset < 1)
            {
                throw new ArgumentException("Month mus be between 1 and 12!", "monthOffset");
            }
            if (culture == null)
            {
                culture = CultureInfo.CurrentUICulture;
            }
            CultureInformation = culture;
            MonthOffset = monthOffset;
            LongName = CultureInformation.DateTimeFormat.MonthNames[monthOffset];
            AbbreviatedName = CultureInformation.DateTimeFormat.AbbreviatedMonthNames[monthOffset];
        }

        #endregion

        #region properties

        /// <summary>
        /// The short name of the month in the <see cref="CultureInformation"/>.
        /// </summary>
        public string AbbreviatedName { get; private set; }

        /// <summary>
        /// The culture to use.
        /// </summary>
        public CultureInfo CultureInformation { get; private set; }

        /// <summary>
        /// The full name of the month in the <see cref="CultureInformation"/>.
        /// </summary>
        public string LongName { get; private set; }

        /// <summary>
        /// The index of the month between 1 and 12.
        /// </summary>
        public uint MonthOffset { get; private set; }

        #endregion

        #region methods

        /// <summary>
        /// Retrieves a list of all months for a complete year. 
        /// </summary>
        /// <returns>The list of <see cref="MonthInfo"/>s for one year.</returns>
        public static IEnumerable<MonthInfo> GetList(CultureInfo culture = null)
        {
            for (uint i = 1; i <= 12; i++)
            {
                yield return new MonthInfo(i, culture);
            }
        }

        #endregion
    }
}
namespace codingfreaks.cfUtils.Tests.Core.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    using Logic.Standard.Extensions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Test methods for <see cref="DateTimeExtensions" />.
    /// </summary>
    [TestClass]
    public class DateTimeExtensionTests
    {
        #region methods

        /// <summary>
        /// Tests the correctness of the calculation of begin of weeks.
        /// </summary>
        [TestMethod]
        public void BeginOfWeekTest()
        {
            // arrange
            var german = CultureInfo.GetCultureInfo("de-DE");
            var us = CultureInfo.GetCultureInfo("en-US");
            var tests = new List<Tuple<DateTime, DateTime, CultureInfo>>
            {
                Tuple.Create(new DateTime(2015, 6, 11), new DateTime(2015, 6, 8), german),
                Tuple.Create(new DateTime(2015, 6, 8), new DateTime(2015, 6, 8), german),
                Tuple.Create(new DateTime(2015, 6, 7), new DateTime(2015, 6, 1), german),
                Tuple.Create(new DateTime(2015, 6, 8), new DateTime(2015, 6, 7), us)
            };
            // act & assert
            tests.ForEach(
                t =>
                {
                    Assert.AreEqual(t.Item2, t.Item1.BeginOfWeek(t.Item3));
                });
        }

        /// <summary>
        /// Tests the correctness of the GetCalendarHalfYearInfo method.
        /// </summary>
        [TestMethod]
        public void GetCalendarHalfYearInfoTest()
        {
            // arrange            
            var tests = new List<Tuple<DateTime, int, DateTime, DateTime>>
            {
                Tuple.Create(new DateTime(2015, 6, 11), 1, new DateTime(2015, 1, 1), new DateTime(2015, 6, 30)),
                Tuple.Create(new DateTime(2015, 8, 11), 2, new DateTime(2015, 7, 1), new DateTime(2015, 12, 31))
            };
            // act & assert
            tests.ForEach(
                t =>
                {
                    var result = t.Item1.GetCalendarHalfYearInfo();
                    Assert.AreEqual(t.Item2, result.SpanNumber);
                    Assert.AreEqual(t.Item3, result.DateStart);
                    Assert.AreEqual(t.Item4, result.DateEnd);
                });
        }

        #endregion
    }
}
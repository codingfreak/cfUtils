namespace codingfreaks.cfUtils.Tests.Core.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Logic.Standard.Extensions;
    using Logic.Standard.Utilities;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Provides test methods for the type <see cref="DateTimeExtensions" />.
    /// </summary>
    [TestClass]
    public class DateTimeUtilsTests
    {
        #region methods

        /// <summary>
        /// Tests the functionallity of <see cref="DateTimeExtensions.BeginOfDay" />.
        /// </summary>
        [TestMethod]
        public void BeginOfDayTest()
        {
            // Arrange
            var results = new List<Tuple<DateTime, DateTime>>
            {
                Tuple.Create(new DateTime(2010, 1, 1, 12, 15, 33), new DateTime(2010, 1, 1, 0, 0, 0)),
                Tuple.Create(new DateTime(2010, 1, 1, 0, 0, 0), new DateTime(2010, 1, 1, 0, 0, 0)),
                Tuple.Create(new DateTime(2010, 1, 1), new DateTime(2010, 1, 1, 0, 0, 0))
            };
            // Act and Assert
            results.ForEach(r => Assert.AreEqual(r.Item2, r.Item1.BeginOfDay()));
        }

        /// <summary>
        /// Tests the functionallity of <see cref="DateTimeExtensions.EndOfDay" />.
        /// </summary>
        [TestMethod]
        public void EndOfDayTest()
        {
            // Arrange
            var results = new List<Tuple<DateTime, DateTime>>
            {
                Tuple.Create(new DateTime(2010, 1, 1, 12, 15, 33), new DateTime(2010, 1, 1, 23, 59, 59, 999)),
                Tuple.Create(new DateTime(2010, 1, 1, 0, 0, 0), new DateTime(2010, 1, 1, 23, 59, 59, 999)),
                Tuple.Create(new DateTime(2010, 1, 1), new DateTime(2010, 1, 1, 23, 59, 59, 999))
            };
            // Act and Assert
            results.ForEach(r => Assert.AreEqual(r.Item2, r.Item1.EndOfDay()));
        }

        /// <summary>
        /// Tests the functionallity of <see cref="DateTimeExtensions.GetCalendarWeeksForYear" />.
        /// </summary>
        [TestMethod]
        public void GetCalendarWeeksForYearTest()
        {
            // Arrange
            var results = new List<Tuple<int, int>>
            {
                Tuple.Create(2014, 52),
                Tuple.Create(2013, 52),
                Tuple.Create(2009, 53)
            };
            // Act and Assert
            results.ForEach(r => Assert.AreEqual(r.Item2, r.Item1.GetCalendarWeeksForYear().Count()));
        }

        /// <summary>
        /// Tests the functionallity of <see cref="DateTimeExtensions.GetFirstDayOfYear" />.
        /// </summary>
        [TestMethod]
        public void GetFirstDayOfYearTest()
        {
            // Arrange
            var results = new List<Tuple<int, DateTime>>
            {
                Tuple.Create(2014, new DateTime(2013, 12, 30).BeginOfDay()),
                Tuple.Create(2013, new DateTime(2012, 12, 31).BeginOfDay())
            };
            // Act and Assert
            results.ForEach(r => Assert.AreEqual(r.Item2, r.Item1.GetFirstDayOfYear()));
        }

        /// <summary>
        /// Tests the functionallity of <see cref="DateTimeUtils.GetLastDayOfMonth" />.
        /// </summary>
        [TestMethod]
        public void GetLastDayOfMonthTest()
        {
            // Arrange
            var results = new List<Tuple<int, int, DateTime>>
            {
                Tuple.Create(2014, 2, new DateTime(2014, 2, 28).BeginOfDay()),
                Tuple.Create(2012, 2, new DateTime(2012, 2, 29).BeginOfDay()),
                Tuple.Create(2016, 2, new DateTime(2016, 2, 29).BeginOfDay()),
                Tuple.Create(2012, 10, new DateTime(2012, 10, 31).BeginOfDay())
            };
            // Act and Assert
            results.ForEach(r => Assert.AreEqual(r.Item3, DateTimeUtils.GetLastDayOfMonth(r.Item1, r.Item2)));
        }

        /// <summary>
        /// Tests the functionallity of
        /// <see cref="DateTimeExtensions.GetYearsDifference(System.DateTime,System.Nullable{System.DateTime})" />.
        /// </summary>
        /// <remarks>
        /// This method was tested in the year 2014.
        /// </remarks>
        [TestMethod]
        public void GetYearsDifferenceTest()
        {
            // Arrange
            var results = new List<Tuple<DateTime?, DateTime?, int?>>
            {
                Tuple.Create((DateTime?)new DateTime(2010, 1, 1), (DateTime?)null, (int?)-6),
                Tuple.Create((DateTime?)new DateTime(2019, 1, 2), (DateTime?)null, (int?)2),
                Tuple.Create((DateTime?)new DateTime(2010, 1, 1), (DateTime?)new DateTime(2009, 1, 1), (int?)1)
            };
            // Act and Assert
            results.ForEach(r => Assert.AreEqual(r.Item3, r.Item1.GetYearsDifference(r.Item2)));
        }

        /// <summary>
        /// Tests the functionallity of <see cref="DateTimeExtensions.ToDateTime" />.
        /// </summary>
        [TestMethod]
        public void ToDateTimeTest()
        {
            // Arrange
            var results = new List<Tuple<decimal, DateTime>>
            {
                Tuple.Create(12.25m, new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 12, 15, 0)),
                Tuple.Create(12.47m, new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 12, 28, 0)),
                Tuple.Create(3.75m, new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 3, 45, 0))
            };
            // Act and Assert
            results.ForEach(r => Assert.AreEqual(r.Item2, r.Item1.ToDateTime()));
        }

        /// <summary>
        /// Tests the functionallity of <see cref="DateTimeExtensions.ToDecimalTime" />.
        /// </summary>
        [TestMethod]
        public void ToDecimalTimeTest()
        {
            // Arrange
            var results = new List<Tuple<DateTime, decimal>>
            {
                Tuple.Create(new DateTime(2010, 1, 1, 12, 15, 33), 12.25m),
                Tuple.Create(new DateTime(2010, 1, 1, 12, 28, 33), 12.47m),
                Tuple.Create(new DateTime(2010, 1, 1, 3, 45, 0), 3.75m)
            };
            // Act and Assert
            results.ForEach(r => Assert.AreEqual(r.Item2, r.Item1.ToDecimalTime()));
        }

        #endregion
    }
}
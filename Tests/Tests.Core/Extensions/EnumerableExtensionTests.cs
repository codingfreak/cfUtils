namespace codingfreaks.cfUtils.Tests.Core.Extensions
{
    using System;
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using codingfreaks.cfUtils.Logic.Portable.Extensions;

    /// <summary>
    /// Test methods for <see cref="EnumerableExtensions"/>.
    /// </summary>
    [TestClass]
    public class EnumerableExtensionTests
    {
        #region methods

        /// <summary>
        /// Tests the correctness of the <see cref="EnumerableExtensions.GetIndexOf{T}(IEnumerable{T}, Func{T,bool})"/> method.
        /// </summary>
        [TestMethod]
        public void GetIndexOfTest()
        {
            // arrange            
            var listStrings = new List<string>
            {
                "First",
                "Second",
                "Third"
            };
            var listLongs = new List<long>
            {
                100L,
                5000000L,
                3L
            };
            var now = DateTime.Now;
            var listDates = new List<DateTime>
            {
                new DateTime(2012, 3, 5),
                now,
                now.AddDays(500)
            };
            // act
            var offsetStrings = listStrings.GetIndexOf("Second");
            var offsetLongs = listLongs.GetIndexOf(3L);
            var offsetDateNow = listDates.GetIndexOf(now);
            var offsetDateFuture = listDates.GetIndexOf(now.AddDays(500));
            // assert
            Assert.AreEqual(1, offsetStrings);
            Assert.AreEqual(2, offsetLongs);
            Assert.AreEqual(1, offsetDateNow);
            Assert.AreEqual(2, offsetDateFuture);
        }

        #endregion
    }
}
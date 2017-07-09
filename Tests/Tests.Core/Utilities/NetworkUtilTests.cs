using System;
using System.Linq;

namespace codingfreaks.cfUtils.Tests.Core.Utilities
{
    using System.Collections.Generic;

    using Logic.Base.Utilities;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Provides unit tests for <see cref="NetworkUtil"/>.
    /// </summary>
    [TestClass]
    public class NetworkUtilTests
    {
        #region methods

        /// <summary>
        /// Tests the functionallity of <see cref="NetworkUtil.IsPortOpened(int)"/>.
        /// </summary>
        [TestMethod]
        public void IsPortOpenedTest()
        {
            // arrange
            var test = new List<Tuple<string, int, bool, bool>>
            {
                Tuple.Create("google.de", 80, false, true),
                Tuple.Create("google.de", 80, true, false),
                Tuple.Create("sdfdsfsdfsdfsdf.de", 10, true, false),
                Tuple.Create("sdfdsfsdfsdfsdf.de", 10, false, false)
            };
            // act and assert
            test.ForEach(
                run =>
                {
                    var result = NetworkUtil.IsPortOpened(run.Item1, run.Item2, 2, run.Item3);
                    Assert.AreEqual(run.Item4, result);
                });
        }

        #endregion
    }
}
﻿namespace s2.s2Utils.Tests.Portable
{
    using System;

    using Logic.Portable.Utilities;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Contains test methods for <see cref="ConversionUtil"/>.
    /// </summary>
    [TestClass]
    public class ConversionUtilTests
    {
        #region methods

        /// <summary>
        /// Tests the functionallity of <see cref="ConversionUtil.GetFormattedDistance(string,int,bool,int)"/> and <see cref="ConversionUtil.GetFormattedDistance(string,double,bool,int)"/>
        /// </summary>
        [TestMethod]
        public void GetDistanceFormattedTest()
        {
            // arrange
            var distance = 1077;
            var result = ConversionUtil.GetFormattedDistance("en-US", distance);
            Assert.IsNotNull(result);
        }

        #endregion
    }
}
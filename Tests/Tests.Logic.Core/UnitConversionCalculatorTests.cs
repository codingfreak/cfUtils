namespace codingfreaks.cfUtils.Tests.Logic.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using cfUtils.Logic.Core.Utilities;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Contains unit tests for <see cref="UnitConversionCalculator" />
    /// </summary>
    [TestClass]
    public class UnitConversionCalculatorTests
    {
        #region methods

        /// <summary>
        /// Tests the correct functionallity of <see cref="UnitConversionCalculator.ConvertAccelerationToGForce" />.
        /// </summary>
        /// <remarks>
        /// Uses an accuracy of 0.0001.
        /// </remarks>
        [TestMethod]
        public void ConvertAccelerationToGForceTest()
        {
            // arrange
            var values = new List<(double acc, double gForce)>
            {
                (395.3, 40.2956),
                (100, 10.19367),
                (10, 1.01936)
            };
            // act && assert
            values.ForEach(
                v =>
                {
                    var result = UnitConversionCalculator.ConvertAccelerationToGForce(v.acc);
                    Assert.AreEqual(v.gForce, result, 0.0001, "Expected result is not inside the expected range.");
                });
        }

        /// <summary>
        /// Tests the correct functionallity of <see cref="UnitConversionCalculator.ConvertCentimetersToInches" />.
        /// </summary>
        /// <remarks>
        /// Uses an accuracy of 0.001.
        /// </remarks>
        [TestMethod]
        public void ConvertCentimetersToInchesTest()
        {
            // arrange
            var values = new List<(double centimeter, double inch)>
            {
                (2.54, 1)
            };
            // act && assert
            values.ForEach(
                v =>
                {
                    var result = UnitConversionCalculator.ConvertCentimetersToInches(v.centimeter);
                    Assert.AreEqual(v.inch, result, 0.001, "Expected result is not inside the expected range.");
                });
        }

        #endregion
    }
}
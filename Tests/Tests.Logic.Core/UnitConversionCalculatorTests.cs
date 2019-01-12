namespace codingfreaks.cfUtils.Tests.Logic.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using cfUtils.Logic.Core.Utilities;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Contains unit tests for <see cref="UnitConversionCalculator" />
    /// </summary>
    [TestClass]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class UnitConversionCalculatorTests
    {
        #region methods

        /// <summary>
        /// Tests the correct functionality of <see cref="UnitConversionCalculator.ConvertAccelerationToGForce" />.
        /// </summary>
        /// <remarks>
        /// Uses an accuracy of 0.0001.
        /// </remarks>
        [TestMethod]
        public void ConvertAccelerationToGForce_Test()
        {
            // arrange
            var values = new List<(double acc, double gForce)>
            {
                (395.3, 40.2956),
                (100, 10.19367),
                (10, 1.01936),
                (0, 0)
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
        /// Tests the correct functionality of <see cref="UnitConversionCalculator.ConvertCentimetersToInches" />.
        /// </summary>
        /// <remarks>
        /// Uses an accuracy of 0.001.
        /// </remarks>
        [TestMethod]
        public void ConvertCentimetersToInches_Test()
        {
            // arrange
            var values = new List<(double centimeter, double inch)>
            {
                (2.54, 1),
                (500, 196.8505),
                (0, 0)
            };
            // act && assert
            values.ForEach(
                v =>
                {
                    var result = UnitConversionCalculator.ConvertCentimetersToInches(v.centimeter);
                    Assert.AreEqual(v.inch, result, 0.001, "Expected result is not inside the expected range.");
                });
        }

        /// <summary>
        /// Tests the correct functionality of <see cref="UnitConversionCalculator.ConvertKilogramToPounds" />.
        /// </summary>
        /// <remarks>
        /// Uses an accuracy of 0.01.
        /// </remarks>
        [TestMethod]
        public void ConvertKilogramsToPound_Test()
        {
            // arrange
            var values = new List<(double kilogram, double pound)>
            {
                (100, 220.462),
                (423, 932.55),
                (0, 0)
            };
            // act && assert
            values.ForEach(
                v =>
                {
                    var result = UnitConversionCalculator.ConvertKilogramToPounds(v.kilogram);
                    Assert.AreEqual(v.pound, result, 0.01, "Expected result is not inside the expected range.");
                });
        }

        /// <summary>
        /// Tests the correct functionality of <see cref="UnitConversionCalculator.ConvertMetersPerSecondToKilometersPerHour" />.
        /// </summary>
        /// <remarks>
        /// Uses an accuracy of 0.01.
        /// </remarks>
        [TestMethod]
        public void ConvertMetersPerSecondToKilometersPerHour_Test()
        {
            // arrange
            var values = new List<(double meters, double kilometers)>
            {
                (100, 360),
                (423.233, 1523.6388),
                (0, 0)
            };
            // act && assert
            values.ForEach(
                v =>
                {
                    var result = UnitConversionCalculator.ConvertMetersPerSecondToKilometersPerHour(v.meters);
                    Assert.AreEqual(v.kilometers, result, 0.01, "Expected result is not inside the expected range.");
                });
        }

        /// <summary>
        /// Tests the correct functionality of <see cref="UnitConversionCalculator.ConvertMetersPerSecondToMilesPerHour" />.
        /// </summary>
        /// <remarks>
        /// Uses an accuracy of 0.01.
        /// </remarks>
        [TestMethod]
        public void ConvertMetersPerSecondToMilesPerHour_Test()
        {
            // arrange
            var values = new List<(double meters, double miles)>
            {
                (100, 223.694),
                (3, 6.71),
                (0, 0)
            };
            // act && assert
            values.ForEach(
                v =>
                {
                    var result = UnitConversionCalculator.ConvertMetersPerSecondToMilesPerHour(v.meters);
                    Assert.AreEqual(v.miles, result, 0.01, "Expected result is not inside the expected range.");
                });
        }

        /// <summary>
        /// Tests the correct functionality of <see cref="UnitConversionCalculator.ConvertMetersToFeet" />.
        /// </summary>
        /// <remarks>
        /// Uses an accuracy of 0.01.
        /// </remarks>
        [TestMethod]
        public void ConvertMetersToFeet_Test()
        {
            // arrange
            var values = new List<(double meters, double feet)>
            {
                (100, 328.084),
                (3, 9.84252),
                (0, 0)
            };
            // act && assert
            values.ForEach(
                v =>
                {
                    var result = UnitConversionCalculator.ConvertMetersToFeet(v.meters);
                    Assert.AreEqual(v.feet, result, 0.01, "Expected result is not inside the expected range.");
                });
        }

        /// <summary>
        /// Tests the correct functionality of <see cref="UnitConversionCalculator.ConvertMetersToMiles" />.
        /// </summary>
        /// <remarks>
        /// Uses an accuracy of 0.01.
        /// </remarks>
        [TestMethod]
        public void ConvertMetersToMiles_Test()
        {
            // arrange
            var values = new List<(double meters, double miles)>
            {
                (14366, 8.9266185),
                (3, 0.00186411),
                (1, 0.00062137121212121),
                (0, 0)
            };
            // act && assert
            values.ForEach(
                v =>
                {
                    var result = UnitConversionCalculator.ConvertMetersToMiles(v.meters);
                    Assert.AreEqual(v.miles, result, 0.0001, "Expected result is not inside the expected range.");
                });
        }

        /// <summary>
        /// Tests the correct functionality of <see cref="UnitConversionCalculator.ConvertMetersToYards" />.
        /// </summary>
        /// <remarks>
        /// Uses an accuracy of 0.01.
        /// </remarks>
        [TestMethod]
        public void ConvertMetersToYards_Test()
        {
            // arrange
            var values = new List<(double meters, double yards)>
            {
                (3477, 3802.4472),
                (12.433, 13.5967288),
                (1, 1.0936),
                (0, 0)
            };
            // act && assert
            values.ForEach(
                v =>
                {
                    var result = UnitConversionCalculator.ConvertMetersToYards(v.meters);
                    Assert.AreEqual(v.yards, result, 0.0001, "Expected result is not inside the expected range.");
                });
        }

        /// <summary>
        /// Tests the correct functionality of <see cref="UnitConversionCalculator.GetApplePlaceString" />.
        /// </summary>
        /// <remarks>
        /// Uses an accuracy of 0.01.
        /// </remarks>
        [TestMethod]
        public void GetApplePlaceString_Test()
        {
            // arrange
            var values = new List<(string place, string city)>
            {
                (null, string.Empty),
                (string.Empty, string.Empty),
                ("D;99999;Testcity;Teststreet;House", "D-99999 Testcity, Teststreet House"),
                ("D;99999;Testcity;Teststreet;House;Some;Other;Fields", "D-99999 Testcity, Teststreet House"),
                ("Too;few;elements", "Too;few;elements"),
                ("Invalid", "Invalid")
            };
            // act && assert
            values.ForEach(
                v =>
                {
                    var result = UnitConversionCalculator.GetApplePlaceString(v.place);
                    Assert.AreEqual(v.city, result, "iOS place string not extracted correctly.");
                });
        }

        #endregion
    }
}
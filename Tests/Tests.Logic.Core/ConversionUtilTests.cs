namespace codingfreaks.cfUtils.Tests.Logic.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using cfUtils.Logic.Core.Utilities;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Contains unit tests for <see cref="ConversionUtil" />
    /// </summary>
    [TestClass]
    public class ConversionUtilTests
    {
        #region constants

        private static readonly object Lock = new object();

        #endregion

        #region methods

        /// <summary>
        /// Tests the correct functionality of <see cref="ConversionUtil.GetFormattedAltitude" />.
        /// </summary>
        /// <remarks>
        /// Uses an accuracy of 0.0001.
        /// </remarks>
        [TestMethod]
        public void GetFormattedAltitudeTest()
        {
            // arrange
            var values = new List<(double meters, string locale, string result, int decimals)>
            {
                ( 1, "de-DE", "1 m", 0),
                ( 5000, "de-DE", "5.000 m", 0),
                ( 200000, "de-DE", "200.000 m", 0),
                ( 12.5, "de-DE", "12,50 m", 2)
            };
            // act && assert
            values.ForEach(
                v =>
                {
                    var result = ConversionUtil.GetFormattedAltitude(v.locale, v.meters, v.decimals);
                    Assert.AreEqual(v.result, result, "Resulting text does not match.");
                });
        }

        /// <summary>
        /// Tests the correct functionality of <see cref="ConversionUtil.GetFormattedDistance" />.
        /// </summary>
        /// <remarks>
        /// Uses an accuracy of 0.0001.
        /// </remarks>
        [TestMethod]
        public void GetFormattedDistanceTest()
        {
            // arrange
            var values = new List<(double meters, string locale, string result, int decimals, bool enforceBig)>
            {
                ( 1, "de-DE", "1 m", 0, false),
                ( 5000, "de-DE", "5 km", 0, false),
                ( 200000, "de-DE", "200 km", 0, false),
                ( 12.5, "de-DE", "12,50 m", 2, false),
                ( 1, "de-DE", "0,001 km", 3, true),
                ( 1, "de-DE", "0,00 km", 2, true),
            };
            // act && assert
            values.ForEach(
                v =>
                {
                    var result = ConversionUtil.GetFormattedDistance(v.locale, v.meters, v.enforceBig, v.decimals);
                    Assert.AreEqual(v.result, result, "Resulting text does not match.");
                });
        }

        /// <summary>
        /// Checks if property changes are producing valid results.
        /// </summary>
        [TestMethod]
        public void PropertyChangeTest()
        {
            // arrange
            const string NewVal = "?";
            var imperialAltitudeUnit = ConversionUtil.ImperialAltitudeUnit;
            var imperialDistanceSmallUnit = ConversionUtil.ImperialDistanceSmallUnit;
            var imperialDistanceUnit = ConversionUtil.ImperialDistanceUnit;
            var imperialHeightUnit = ConversionUtil.ImperialHeightUnit;
            var imperialSpeedUnit = ConversionUtil.ImperialSpeedUnit;
            var imperialWeightUnit = ConversionUtil.ImperialWeightUnit;
            var metricAltitudeUnit = ConversionUtil.MetricAltitudeUnit;
            var metricDistanceSmallUnit = ConversionUtil.MetricDistanceSmallUnit;
            var metricDistanceUnit = ConversionUtil.MetricDistanceUnit;
            var metricHeightUnit = ConversionUtil.MetricHeightUnit;
            var metricSpeedUnit = ConversionUtil.MetricSpeedUnit;
            var metricWeightUnit = ConversionUtil.MetricWeightUnit;
            // act
            lock (Lock)
            {
                ConversionUtil.ImperialAltitudeUnit = NewVal;
                ConversionUtil.ImperialDistanceSmallUnit = NewVal;
                ConversionUtil.ImperialDistanceUnit = NewVal;
                ConversionUtil.ImperialHeightUnit = NewVal;
                ConversionUtil.ImperialSpeedUnit = NewVal;
                ConversionUtil.ImperialWeightUnit = NewVal;
                ConversionUtil.MetricAltitudeUnit = NewVal;
                ConversionUtil.MetricDistanceSmallUnit = NewVal;
                ConversionUtil.MetricDistanceUnit = NewVal;
                ConversionUtil.MetricHeightUnit = NewVal;
                ConversionUtil.MetricSpeedUnit = NewVal;
                ConversionUtil.MetricWeightUnit = NewVal;
            }
            // assert
            Assert.AreEqual(NewVal, ConversionUtil.ImperialAltitudeUnit);
            Assert.AreEqual(NewVal, ConversionUtil.ImperialDistanceSmallUnit);
            Assert.AreEqual(NewVal, ConversionUtil.ImperialDistanceUnit);
            Assert.AreEqual(NewVal, ConversionUtil.ImperialHeightUnit);
            Assert.AreEqual(NewVal, ConversionUtil.ImperialSpeedUnit);
            Assert.AreEqual(NewVal, ConversionUtil.ImperialWeightUnit);
            Assert.AreEqual(NewVal, ConversionUtil.MetricAltitudeUnit);
            Assert.AreEqual(NewVal, ConversionUtil.MetricDistanceSmallUnit);
            Assert.AreEqual(NewVal, ConversionUtil.MetricDistanceUnit);
            Assert.AreEqual(NewVal, ConversionUtil.MetricHeightUnit);
            Assert.AreEqual(NewVal, ConversionUtil.MetricSpeedUnit);
            Assert.AreEqual(NewVal, ConversionUtil.MetricWeightUnit);
            // set defaults because other tests might need those values
            ConversionUtil.ImperialAltitudeUnit = imperialAltitudeUnit;
            ConversionUtil.ImperialDistanceSmallUnit = imperialDistanceSmallUnit;
            ConversionUtil.ImperialDistanceUnit = imperialDistanceUnit;
            ConversionUtil.ImperialHeightUnit = imperialHeightUnit;
            ConversionUtil.ImperialSpeedUnit = imperialSpeedUnit;
            ConversionUtil.ImperialWeightUnit = imperialWeightUnit;
            ConversionUtil.MetricAltitudeUnit = metricAltitudeUnit;
            ConversionUtil.MetricDistanceSmallUnit = metricDistanceSmallUnit;
            ConversionUtil.MetricDistanceUnit = metricDistanceUnit;
            ConversionUtil.MetricHeightUnit = metricHeightUnit;
            ConversionUtil.MetricSpeedUnit = metricSpeedUnit;
            ConversionUtil.MetricWeightUnit = metricWeightUnit;
        }

        /// <summary>
        /// Checks if all initial property states are valid.
        /// </summary>
        [TestMethod]
        public void PropertyInitTest()
        {
            lock (Lock)
            {
                Assert.AreEqual("m", ConversionUtil.MetricAltitudeUnit);
                Assert.AreEqual("m", ConversionUtil.MetricDistanceSmallUnit);
                Assert.AreEqual("km", ConversionUtil.MetricDistanceUnit);
                Assert.AreEqual("cm", ConversionUtil.MetricHeightUnit);
                Assert.AreEqual("km/h", ConversionUtil.MetricSpeedUnit);
                Assert.AreEqual("kg", ConversionUtil.MetricWeightUnit);
                Assert.AreEqual("ft", ConversionUtil.ImperialAltitudeUnit);
                Assert.AreEqual("yd", ConversionUtil.ImperialDistanceSmallUnit);
                Assert.AreEqual("mi", ConversionUtil.ImperialDistanceUnit);
                Assert.AreEqual("in", ConversionUtil.ImperialHeightUnit);
                Assert.AreEqual("mph", ConversionUtil.ImperialSpeedUnit);
                Assert.AreEqual("lbs", ConversionUtil.ImperialWeightUnit);
            }
        }

        #endregion
    }
}
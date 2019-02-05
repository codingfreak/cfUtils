namespace codingfreaks.cfUtils.Tests.Logic.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using cfUtils.Logic.Core.Utilities;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using NAssert = NUnit.Framework.Assert;

    /// <summary>
    /// Contains unit tests for <see cref="ConversionUtil" />
    /// </summary>
    [TestClass]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class ConversionUtilTests
    {
        #region constants

        private static readonly object Lock = new object();

        #endregion

        #region methods

        /// <summary>
        /// Tests the correct functionality of <see cref="ConversionUtil.GetAltitude" />.
        /// </summary>
        [TestMethod]
        public void GetAltitude_Test()
        {
            // arrange
            var values = new List<(string locale, double input, double result)>
            {
                ("de-DE", 12, 12),
                ("en-us", 12, 39.37008),
                ("en-GB", 12, 12),
                ("de-DE", 1343.677677772, 1343.677677772)
            };
            // act && assert
            values.ForEach(
                v =>
                {
                    var result = ConversionUtil.GetAltitude(v.locale, v.input);
                    Assert.AreEqual(v.result, result, "Resulting altitude is wrong.");
                });
        }

        /// <summary>
        /// Tests the correct functionality of <see cref="ConversionUtil.GetAltitudeUnit" />.
        /// </summary>
        [TestMethod]
        public void GetAltitudeUnit_Test()
        {
            // arrange
            var values = new List<(string locale, string result, Type exceptionType)>
            {
                ("", "", typeof(ArgumentException)),
                ("de", "", typeof(ArgumentException)),
                ("de-DE", "m", null),
                ("en-US", "ft", null),
                ("en-GB", "m", null)
            };
            // act && assert
            values.ForEach(
                v =>
                {
                    if (v.exceptionType != null)
                    {
                        NAssert.Throws(v.exceptionType, () => ConversionUtil.GetAltitudeUnit(v.locale), "Expected exception not thrown.");
                        return;
                    }
                    var result = ConversionUtil.GetAltitudeUnit(v.locale);
                    Assert.AreEqual(v.result, result, "Resulting text does not match.");
                });
        }

        /// <summary>
        /// Tests the correct functionality of <see cref="ConversionUtil.GetDistance" />.
        /// </summary>
        [TestMethod]
        public void GetDistance_Test()
        {
            // arrange
            var values = new List<(string locale, double input, double result)>
            {
                ("de-DE", 1000, 1),
                ("de-DE", 100000, 100),
                ("de-DE", 5, 0.005),
                ("de-DE", 1, 0.001),
                ("en-US", 1000, 0.621371192),
                ("en-US", 1, 0.000621371192),
                ("en-US", 1000000, 621.371192),
                ("en-US", 0.0000000005, 0.0000000005)
            };
            // act && assert
            values.ForEach(
                v =>
                {
                    var result = ConversionUtil.GetDistance(v.locale, v.input);
                    Assert.AreEqual(v.result, result, "Resulting distance is wrong.");
                });
        }

        /// <summary>
        /// Tests the correct functionality of <see cref="ConversionUtil.GetDistanceUnit" />.
        /// </summary>
        [TestMethod]
        public void GetDistanceUnit_Test()
        {
            // arrange
            var values = new List<(string locale, string result, Type exceptionType)>
            {
                ("", "", typeof(ArgumentException)),
                ("de-DE", "km", null),
                ("en-US", "mi", null),
                ("en-GB", "km", null)
            };
            // act && assert
            values.ForEach(
                v =>
                {
                    if (v.exceptionType != null)
                    {
                        NAssert.Throws(v.exceptionType, () => ConversionUtil.GetDistanceUnit(v.locale), "Expected exception not thrown.");
                        return;
                    }
                    var result = ConversionUtil.GetDistanceUnit(v.locale);
                    Assert.AreEqual(v.result, result, "Resulting text does not match.");
                });
        }

        /// <summary>
        /// Tests the correct functionality of <see cref="ConversionUtil.GetFormattedAltitude" />.
        /// </summary>
        /// <remarks>
        /// Uses an accuracy of 0.0001.
        /// </remarks>
        [TestMethod]
        public void GetFormattedAltitude_Test()
        {
            // arrange
            var values = new List<(double meters, string locale, string result, int decimals)>
            {
                (1, "de-DE", "1 m", 0),
                (5000, "de-DE", "5.000 m", 0),
                (200000, "de-DE", "200.000 m", 0),
                (12.5, "de-DE", "12,50 m", 2),
                (0, "de-DE", "0 m", 0)
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
        public void GetFormattedDistance_Test()
        {
            // arrange
            var values = new List<(double meters, string locale, string result, int decimals, bool enforceBig)>
            {
                (1, "de-DE", "1 m", 0, false),
                (5000, "de-DE", "5 km", 0, false),
                (200000, "de-DE", "200 km", 0, false),
                (12.5, "de-DE", "12,50 m", 2, false),
                (1, "de-DE", "0,001 km", 3, true),
                (1, "de-DE", "0,00 km", 2, true)
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
        /// Tests the correct functionality of <see cref="ConversionUtil.GetFormattedHeight" />.
        /// </summary>
        /// <remarks>
        /// Uses an accuracy of 0.0001.
        /// </remarks>
        [TestMethod]
        public void GetFormattedHeight_Test()
        {
            // arrange
            var values = new List<(double centiMeters, string locale, string result, int decimals)>
            {
                (1, "de-DE", "1 cm", 0),
                (5000, "de-DE", "5.000 cm", 0),
                (200000, "de-DE", "200.000 cm", 0),
                (12.5, "de-DE", "12,50 cm", 2),
                (1, "en-US", "0.394 in", 3),
                (2.54, "en-us", "1.00 in", 2)
            };
            // act && assert
            values.ForEach(
                v =>
                {
                    var result = ConversionUtil.GetFormattedHeight(v.locale, v.centiMeters, v.decimals);
                    Assert.AreEqual(v.result, result, "Resulting text does not match.");
                });
        }

        /// <summary>
        /// Tests the correct functionality of <see cref="ConversionUtil.GetFormattedSeconds" />.
        /// </summary>
        /// <remarks>
        /// Uses an accuracy of 0.0001.
        /// </remarks>
        [TestMethod]
        public void GetFormattedSeconds_Test()
        {
            // arrange
            var values = new List<(long seconds, string locale, string result)>
            {
                (1, "de-DE", "00:00:01"),
                (100, "en-US", "00:01:40")
            };
            // act && assert
            values.ForEach(
                v =>
                {
                    var result = ConversionUtil.GetFormattedSeconds(v.locale, v.seconds);
                    Assert.AreEqual(v.result, result, "Resulting text does not match.");
                });
        }

        /// <summary>
        /// Tests the correct functionality of <see cref="ConversionUtil.GetFormattedTimespan" />.
        /// </summary>
        /// <remarks>
        /// Uses an accuracy of 0.0001.
        /// </remarks>
        [TestMethod]
        public void GetFormattedTimespan_Test()
        {
            // arrange
            var values = new List<(long seconds, string locale, string result)>
            {
                (1, "de-DE", "00:00:01"),
                (100, "en-US", "00:01:40")
            };
            // act && assert
            values.ForEach(
                v =>
                {
                    var result = ConversionUtil.GetFormattedTimespan(v.locale, v.seconds);
                    Assert.AreEqual(v.result, result, "Resulting text does not match.");
                });
        }

        /// <summary>
        /// Tests the correct functionality of <see cref="ConversionUtil.GetFormattedSpeed" />.
        /// </summary>
        /// <remarks>
        /// Uses an accuracy of 0.0001.
        /// </remarks>
        [TestMethod]
        public void GetFormattedSpeed_Test()
        {
            // arrange
            var values = new List<(double metersPerSecond, string locale, string result, int decimals)>
            {
                (6, "de-DE", "22 km/h", 0),
                (6, "de-DE", "21,6 km/h", 1),
                (6, "en-US", "13 mph", 0)
            };
            // act && assert
            values.ForEach(
                v =>
                {
                    var result = ConversionUtil.GetFormattedSpeed(v.locale, v.metersPerSecond, v.decimals);
                    Assert.AreEqual(v.result, result, "Resulting text does not match.");
                });
        }

        /// <summary>
        /// Checks if property changes are producing valid results.
        /// </summary>
        [TestMethod]
        public void PropertyChange_Test()
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
            Assert.AreEqual(NewVal, ConversionUtil.ImperialAltitudeUnit, "Change of imperial altitude unit not accepted.");
            Assert.AreEqual(NewVal, ConversionUtil.ImperialDistanceSmallUnit, "Change of imperial small distance unit not accepted.");
            Assert.AreEqual(NewVal, ConversionUtil.ImperialDistanceUnit, "Change of imperial distance unit not accepted.");
            Assert.AreEqual(NewVal, ConversionUtil.ImperialHeightUnit, "Change of imperial height unit not accepted.");
            Assert.AreEqual(NewVal, ConversionUtil.ImperialSpeedUnit, "Change of imperial speed unit not accepted.");
            Assert.AreEqual(NewVal, ConversionUtil.ImperialWeightUnit, "Change of imperial weight unit not accepted.");
            Assert.AreEqual(NewVal, ConversionUtil.MetricAltitudeUnit, "Change of metric altitude unit not accepted.");
            Assert.AreEqual(NewVal, ConversionUtil.MetricDistanceSmallUnit, "Change of metric small distance unit not accepted.");
            Assert.AreEqual(NewVal, ConversionUtil.MetricDistanceUnit, "Change of metric distance unit not accepted.");
            Assert.AreEqual(NewVal, ConversionUtil.MetricHeightUnit, "Change of metric height unit not accepted.");
            Assert.AreEqual(NewVal, ConversionUtil.MetricSpeedUnit, "Change of metric speed unit not accepted.");
            Assert.AreEqual(NewVal, ConversionUtil.MetricWeightUnit, "Change of metric weight unit not accepted.");
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
        public void PropertyInit_Test()
        {
            lock (Lock)
            {
                Assert.AreEqual("ft", ConversionUtil.ImperialAltitudeUnit, "Incorrect imperial altitude unit.");
                Assert.AreEqual("yd", ConversionUtil.ImperialDistanceSmallUnit, "Incorrect imperial small distance unit.");
                Assert.AreEqual("mi", ConversionUtil.ImperialDistanceUnit, "Incorrect imperial distance unit.");
                Assert.AreEqual("in", ConversionUtil.ImperialHeightUnit, "Incorrect imperial height unit.");
                Assert.AreEqual("mph", ConversionUtil.ImperialSpeedUnit, "Incorrect imperial speed unit.");
                Assert.AreEqual("lbs", ConversionUtil.ImperialWeightUnit, "Incorrect imperial weight unit.");
                Assert.AreEqual("m", ConversionUtil.MetricAltitudeUnit, "Incorrect metric altitude unit.");
                Assert.AreEqual("m", ConversionUtil.MetricDistanceSmallUnit, "Incorrect metric small distance unit.");
                Assert.AreEqual("km", ConversionUtil.MetricDistanceUnit, "Incorrect metric distance unit.");
                Assert.AreEqual("cm", ConversionUtil.MetricHeightUnit, "Incorrect metric height unit.");
                Assert.AreEqual("km/h", ConversionUtil.MetricSpeedUnit, "Incorrect metric speed unit.");
                Assert.AreEqual("kg", ConversionUtil.MetricWeightUnit, "Incorrect metric weight unit.");
            }
        }

        #endregion
    }
}
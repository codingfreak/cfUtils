namespace codingfreaks.cfUtils.Tests.Core.Helpers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using codingfreaks.cfUtils.Logic.Base.Structures;
    using codingfreaks.cfUtils.Logic.Base.Utilities;

    /// <summary>
    /// Test methods for <see cref="AppUtil"/>.
    /// </summary>
    [TestClass]
    public class AppUtilTests
    {
        #region methods

        /// <summary>
        /// Tests the logic of <see cref="AppUtil.AreCommandArgumentsValid"/> with a collection of valid in invalid calls.
        /// </summary>
        [TestMethod]
        public void AreCommandArgumentsValidTest()
        {
            // Create a dictionary containing a set of arguments passed to the testet method combined with a bool indicating, if the
            // ApplicationInfo provided in the next step should result in a valid or invalid result.
            var testValues = new Dictionary<string[], bool>
            {
                { new[] { "testValue1", "testValue2", "-v=1", "-x=true", "-flagParam" }, true },
                { new[] { "testValue1", "testValue2", "-version=1", "-someOption=true", "-flagParam" }, true },
                { new[] { "testValue1", "testValue2", "-v=1", "-x=true" }, true },
                { new[] { "testValue1", "-version=1", "-x=true", "-flagParam" }, false }
            };
            // Generate the ApplicationInfo to test against
            var appInfo = GetTestAppInfo();
            // Perform all tests
            testValues.Keys.ToList().ForEach(val => Assert.AreEqual(testValues[val], AppUtil.AreCommandArgumentsValid(val, appInfo)));
        }

        /// <summary>
        /// Tests the logic of <see cref="AppUtil.MapCommandArguments"/> with a collection of valid in invalid calls.
        /// </summary>
        [TestMethod]
        public void MapCommandArgumentsTest()
        {
            // Create a dictionary containing a set of arguments passed to the testet method combined with a number representing
            // the amount of arguments the MapCommandArguments should return.
            var testValues = new Dictionary<string[], int>
            {
                { new[] { "testValue1", "testValue2", "-v=1", "-x=true", "-flagParam" }, 5 },
                { new[] { "testValue1", "testValue2", "-version=1", "-someOption=true", "-flagParam" }, 5 },
                { new[] { "testValue1", "testValue2", "-v=1", "-x=true" }, 4 },
                { new[] { "testValue1", "-version=1", "-x=true", "-flagParam" }, 0 }
            };
            // Generate the ApplicationInfo to test against
            var appInfo = GetTestAppInfo();
            // Perform all tests
            testValues.Keys.ToList().ForEach(val => Assert.AreEqual(testValues[val], AppUtil.MapCommandArguments(val, appInfo).Count));
        }

        private static ApplicationInfo GetTestAppInfo()
        {
            return new ApplicationInfo
            {
                AssemblyInfo = Assembly.GetExecutingAssembly(),
                CommandlineArgumentInfos = new List<CommandlineArgumentInfo>
                {
                    new CommandlineArgumentInfo
                    {
                        Abbreviation = "a1",
                        ArgumentName = "argument1",
                        IsMandatory = true,
                        OrderPosition = 1
                    },
                    new CommandlineArgumentInfo
                    {
                        Abbreviation = "a2",
                        ArgumentName = "argument2",
                        IsMandatory = true,
                        OrderPosition = 2
                    },
                    new CommandlineArgumentInfo
                    {
                        Abbreviation = "v",
                        ArgumentName = "version",
                        IsNumeric = true
                    },
                    new CommandlineArgumentInfo
                    {
                        Abbreviation = "x",
                        ArgumentName = "someOption",
                        IsBool = true
                    },
                    new CommandlineArgumentInfo
                    {
                        Abbreviation = "f",
                        ArgumentName = "flagParam",
                        IsFlag = true
                    }
                },
                ParameterDelimiter = '=',
                ParameterPraefix = '-'
            };
        }

        #endregion
    }
}
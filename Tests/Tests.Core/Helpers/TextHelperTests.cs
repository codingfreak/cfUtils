namespace codingfreaks.cfUtils.Tests.Core.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Logic.Standard.Extensions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Contains test-methods
    /// </summary>
    [TestClass]
    public class TextHelperTests
    {
        #region methods

        /// <summary>
        /// Tests the functionallity of the <see cref="StringExtensions.IsValidEmailAddress" /> method.
        /// </summary>
        [TestMethod]
        public void IsValidEmailAddressTest()
        {
            var testMails = new Dictionary<string, bool>
            {
                { "test@sakple.com", true },
                { "dsdsd@s.c", false },
                { "dasdasdasd@test.museum", true }
            };
            testMails.Keys.ToList().ForEach(key => Assert.AreEqual(testMails[key], key.IsValidEmailAddress()));
        }

        #endregion
    }
}
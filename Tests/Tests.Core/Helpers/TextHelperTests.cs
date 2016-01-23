namespace s2.s2Utils.Tests.Core.Helpers
{
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using s2.s2Utils.Logic.Base.Extensions;
    using s2.s2Utils.Logic.Portable.Extensions;

    /// <summary>
    /// Contains test-methods
    /// </summary>
    [TestClass]
    public class TextHelperTests
    {
        #region methods

        /// <summary>
        /// Tests the functionallity of the <see cref="StringExtensions.IsValidEmailAddress"/> method.
        /// </summary>
        [TestMethod]
        public void IsValidEmailAddressTest()
        {
            var testMails = new Dictionary<string, bool>
            {
                { "schmidt@sab-team.com", true },
                { "schmidt@s.c", false },
                { "schmidt@test.museum", true }
            };
            testMails.Keys.ToList().ForEach(key => Assert.AreEqual(testMails[key], key.IsValidEmailAddress()));
        }

        #endregion
    }
}
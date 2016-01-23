using System;
using System.Linq;

namespace s2.s2Utils.Tests.Core.Utilities
{
    using System.Collections.Generic;

    using Logic.Utils.Enumerations;
    using Logic.Utils.Utilities;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Provides unit tests for <see cref="MailUtil"/>.
    /// </summary>
    [TestClass]
    public class MailUtilTests
    {
        #region methods

        /// <summary>
        /// Tests the functionallity of <see cref="MailUtil.OpenOutlookMail(string,System.Collections.Generic.IEnumerable{string},string,string,string,string,IEnumerable{string},OutlookMailBodyFormat)"/>.
        /// </summary>
        [TestMethod]
        public void OpenOutlookMailMultiTest()
        {
            var rec = new List<string>
            {
                "testmann@test.de",
                "mustermann@muster.com"
            };
            Assert.IsTrue(MailUtil.OpenOutlookMail("schmidt@sab-team.com", rec, "schmidt@sab-team.com", "Testnachricht", "Dies ist ein Test", null));
        }

        /// <summary>
        /// Tests the functionallity of <see cref="MailUtil.OpenOutlookMail(string,System.Collections.Generic.IEnumerable{string},string,string,string,string,IEnumerable{string},OutlookMailBodyFormat)"/>.
        /// </summary>
        [TestMethod]
        public void OpenOutlookMailTest()
        {
            Assert.IsTrue(MailUtil.OpenOutlookMail("schmidt@sab-team.com", "testmann@test.de", "schmidt@sab-team.com", "Testnachricht", "Dies ist ein Test"));
        }

        #endregion
    }
}
namespace codingfreaks.cfUtils.Tests.Core.Extensions
{
    using System;
    using System.Linq;

    using Logic.Standard.Extensions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Test methods for <see cref="StringExtensions" />.
    /// </summary>
    [TestClass]
    public class StringExtensionTests
    {
        #region methods

        /// <summary>
        /// Tests the correctness of the <see cref="StringExtensions.ToChar" /> extension method.
        /// </summary>
        [TestMethod]
        public void ToCharTest()
        {
            Assert.AreEqual('d', "d".ToChar());
            Assert.AreEqual(default(char), "".ToChar());
            Assert.AreEqual('H', "Hello".ToChar());
        }

        #endregion
    }
}
using System;
using System.Linq;

namespace s2.s2Utils.Tests.Core.Utilities
{
    using System.Threading;

    using Logic.Base.Structures;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Provides unit tests for <see cref="ClaimBasedUser"/>.
    /// </summary>
    [TestClass]
    public class ClaimBasedUserTests
    {
        #region methods

        /// <summary>
        /// Tests the method <see cref="ClaimBasedUser.Create(Thread)"/>.
        /// </summary>
        [TestMethod]
        public void TestCreationFromThread()
        {
            var claimUser = ClaimBasedUser.Create(Thread.CurrentThread);
            Assert.IsNotNull(claimUser);
        }

        #endregion
    }
}
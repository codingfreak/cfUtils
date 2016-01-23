using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s2.s2Utils.Tests.Core.Utilities
{
    using System.Threading;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using s2.s2Utils.Logic.Base.Extensions;
    using s2.s2Utils.Logic.Base.Utilities;
    using s2.s2Utils.Logic.Portable.Extensions;

    /// <summary>
    /// Contains unit tests for the type <see cref="AlarmClockUtil"/>.
    /// </summary>
    [TestClass]
    public class AlarmClockUtilTests
    {

        /// <summary>
        /// Tests the start method.
        /// </summary>
        [TestMethod]
        public void TestStart()
        {
            // arrange
            var util = new AlarmClockUtil();
            var occured = 0;
            var now = DateTime.Now;
            var current = now.Subtract(now.BeginOfDay());
            var times = new List<TimeSpan>
            {
                TimeSpan.FromSeconds(current.TotalSeconds + 10),
                TimeSpan.FromSeconds(current.TotalSeconds + 20)
            };
            util.AlarmOccured += (s, e) => occured++;
            // act 
            util.Start(times);
            Task.Delay(30000).Wait();
            // assert
            Assert.AreEqual(times.Count, occured);
            Assert.IsFalse(util.IsRunning);
            util.Dispose();
        }

        /// <summary>
        /// Tests the start method.
        /// </summary>
        [TestMethod]
        public void TestRevolvingStart()
        {
            // arrange
            var util = new AlarmClockUtil();            
            var now = DateTime.Now;
            var nextStart = now.AddHours(1);
            var current = now.Subtract(now.BeginOfDay());
            var times = new List<TimeSpan>
            {
                TimeSpan.FromSeconds(current.TotalSeconds + 10),
                TimeSpan.FromSeconds(current.TotalSeconds + 3600)
            };
            var handle = new AutoResetEvent(false);
            util.AlarmOccured += (s, e) => handle.Set();            
            // act             
            util.Start(times, false);
            handle.WaitOne();
            // assert
            Assert.IsTrue(nextStart.Subtract(util.NextPlannedStart).TotalSeconds <= 0.001d);
            Assert.IsTrue(util.IsRunning);
            util.Dispose();
        }

        /// <summary>
        /// Tests if exception handling on empty parameters is corrrect.
        /// </summary>
        [TestMethod]
        public void TestStartExceptionOnEmpty()
        {
            // arrange
            var util = new AlarmClockUtil();
            var tests = new List<Tuple<IEnumerable<TimeSpan>, Type>>
            {
                Tuple.Create(Enumerable.Empty<TimeSpan>(), typeof(ArgumentException)), 
                Tuple.Create((IEnumerable<TimeSpan>)null, typeof(ArgumentNullException))
            };
            // act & assert
            tests.ForEach(
                t =>
                {
                    NUnit.Framework.Assert.Throws(t.Item2, () => util.Start(t.Item1));
                });  
            util.Dispose();
        }

    }
}

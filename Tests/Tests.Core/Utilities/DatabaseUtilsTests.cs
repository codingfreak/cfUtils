namespace s2.s2Utils.Tests.Core.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    using Logic.Base.Utilities;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Contains tests for <see cref="DatabaseUtils" />
    /// </summary>
    [TestClass]
    public class DatabaseUtilsTests
    {
        #region methods

        /// <summary>
        /// Tests the functionallity of <see cref="DatabaseUtils.GetConnectionAsync" />.
        /// </summary>
        [TestMethod]
        public async void GetConnectionAsyncTest()
        {
            // arrange
            var tests = new List<Tuple<string, string, bool, bool>>
            {
                Tuple.Create("Data Source=rc-oase-db.ocn.ottogroup.com;Initial Catalog=OASE;Persist Security Info=True;User ID=oaseadmin;Password=GrKHIQ9epd", "System.Data.SqlClient", false, true),
                Tuple.Create("TestSql", string.Empty, true, true)
            };
            // act and assert
            foreach (var t in tests)
            {
                var conn = await DatabaseUtils.GetConnectionAsync(t.Item1, t.Item3, t.Item2);
                if (t.Item4)
                {
                    Assert.IsNotNull(conn);
                    Assert.IsTrue(conn.State == ConnectionState.Open);
                }
                else
                {
                    Assert.IsNull(conn);
                }
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }

        /// <summary>
        /// Tests the functionallity of <see cref="DatabaseUtils.GetReaderAsync" /> and indirectly
        /// <see cref="DatabaseUtils.GetConnectionAsync" /> and <see cref="DatabaseUtils.GetScalarResultAsync{TResult}" /> .
        /// </summary>
        [TestMethod]
        public async void GetReaderTest()
        {
            // arrange
            var tests = new List<Tuple<string, string, string>>
            {
                Tuple.Create("TestSql", "SELECT COUNT(*) FROM TBL_EMPLOYEES", "SELECT * FROM TBL_EMPLOYEES")
            };
            // act and assert
            foreach (var t in tests)
            {
                var elements = 0;
                var expectedCount = 0;
                using (var conn = await DatabaseUtils.GetConnectionAsync(t.Item1, true))
                {
                    expectedCount = await DatabaseUtils.GetScalarResultAsync<int>(t.Item2, conn);
                    using (var reader = await DatabaseUtils.GetReaderAsync(t.Item3, conn))
                    {
                        while (reader.Read())
                        {
                            elements++;
                        }
                    }
                }
                Assert.AreEqual(expectedCount, elements);
            }
        }

        #endregion
    }
}
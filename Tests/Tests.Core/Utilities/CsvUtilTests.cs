namespace s2.s2Utils.Tests.Core.Utilities
{
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using s2.s2Utils.Logic.Base.Utilities;

    /// <summary>
    /// Provides test methods for the type <see cref="CsvUtil" />.
    /// </summary>
    [TestClass]
    public class CsvUtilTests
    {
        #region methods

        /// <summary>
        /// Tests if <see cref="CsvUtil.IsValidCsvFile(string,bool,char)" /> acts correctly.
        /// </summary>
        [TestMethod]
        public void IsValidCsvFileTest()
        {
            // act and assert
            Assert.IsTrue(CsvUtil.IsValidCsvFile("TestData\\parsefile.csv", true));
            Assert.IsFalse(CsvUtil.IsValidCsvFile("TestData\\parsefile_columnerror.csv", true));
        }

        /// <summary>
        /// Tests if <see cref="CsvUtil.ParseFile(string,System.Text.Encoding,bool,char,bool)" /> works correctly and simply omits
        /// the one line which has only 2 columns.
        /// </summary>
        [TestMethod]
        public void ParseFileColumnErrorTest()
        {
            // arrange 
            const int ExpectedRowsWithoutValidity = 4;
            // act
            var actualRows = CsvUtil.ParseFile("TestData\\parsefile_columnerror.csv", true).Count();
            // assert            
            Assert.AreEqual(ExpectedRowsWithoutValidity, actualRows);
            Assert.IsNull(CsvUtil.ParseFile("TestData\\parsefile_columnerror.csv", true, ',', true));
        }

        /// <summary>
        /// Tests if <see cref="CsvUtil.ParseFile(string,System.Text.Encoding,bool,char,bool)" /> works correctly.
        /// </summary>
        [TestMethod]
        public void ParseFileTest()
        {
            // arrange 
            var columns = new[] { "Column1", "Column2", "Column3" };
            // act
            var line = CsvUtil.ParseFile("TestData\\parsefile.csv", true).First();
            // assert            
            columns.ToList().ForEach(
                col =>
                {
                    Assert.IsTrue(line.Any(l => l.Key == col));
                });
        }

        #endregion
    }
}
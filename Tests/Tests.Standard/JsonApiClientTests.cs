using System;
using System.Linq;

namespace codingfreaks.cfUtils.Tests.Standard
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Logic.Standard.Utilities;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class JsonApiClientTests
    {
        #region member vars

        private readonly Uri _issTrackerEndpoint = new Uri("https://api.wheretheiss.at/v1/");

        #endregion

        #region methods

        /// <summary>
        /// Checks the functionallity of the method
        /// <see cref="JsonApiClient.GetWithResultAsync{TResult}(string,System.Collections.Generic.IEnumerable{System.Collections.Generic.KeyValuePair{string,string}})" />
        /// </summary>
        [TestMethod]
        public async Task ApiClientGetTest()
        {
            var client = JsonApiClient.GetInstance(_issTrackerEndpoint);
            var result = await client.GetWithResultAsync<object>("satellites/25544");
            Assert.IsNotNull(result);
        }

        #endregion
    }
}
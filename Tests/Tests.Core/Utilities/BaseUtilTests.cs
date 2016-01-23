using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s2.s2Utils.Tests.Core.Utilities
{
    using Logic.Base.Structures;
    using Logic.Tests;
    using Logic.Utils.Utilities;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    
    [TestClass]
    public class BaseUtilTests
    {

        [TestMethod]
        public void Foo()
        {
            var util = Utils.GetDirect<PersonUtil>();
            Utils.Init(null, new ContextResolver());
            var request = new PagedRequest()
            {
                ItemsPerPage = 10,
                PageToDeliver = 1
            };
            var result = util.GetBasePagedResultAsync(request, null, null, p => p.Firstname.Contains("Peter")).Result;
            Assert.IsNotNull(result);
        }

    }
}

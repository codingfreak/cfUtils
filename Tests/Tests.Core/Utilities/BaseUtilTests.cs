using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace codingfreaks.cfUtils.Tests.Core.Utilities
{
    using codingfreaks.cfUtils.Logic.Tests;
    using codingfreaks.cfUtils.Logic.Utils.Utilities;

    using Logic.Base.Structures;


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

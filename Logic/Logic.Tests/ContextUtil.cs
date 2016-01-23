using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace codingfreaks.cfUtils.Logic.Tests
{
    public static class ContextUtil
    {

        public static TestDbEntities Context
        {
            get
            {
                return new TestDbEntities();
            }
        }

    }
}

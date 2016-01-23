using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace codingfreaks.cfUtils.Logic.Tests
{
    using System.Data.Entity;

    using Utils.Interfaces;

    public class ContextResolver : IContextResolver
    {
        public DbContext GetContext(Type targetType)
        {
            return ContextUtil.Context;
        }
    }
}

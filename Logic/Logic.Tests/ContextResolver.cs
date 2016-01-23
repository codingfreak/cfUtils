using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s2.s2Utils.Logic.Tests
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

namespace s2.s2Utils.Logic.Tests
{
    using System;
    using System.Linq;

    using Logic.Base.Interfaces;
    using Logic.Utils.Interfaces;
    using Logic.Utils.Utilities;

    public class PersonUtil : BaseUtil<Person, TestDbEntities>
    {
        #region constructors and destructors

        public PersonUtil(ILogger logger, IContextResolver contextResolver) : base(logger, contextResolver)
        {
        }

        #endregion
    }
}
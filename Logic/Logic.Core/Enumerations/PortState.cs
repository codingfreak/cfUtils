namespace codingfreaks.cfUtils.Logic.Core.Enumerations
{
    using System;
    using System.Linq;

    using Utilities;

    /// <summary>
    /// Defines possible results for the <see cref="NetworkUtil.GetPortState" /> method.
    /// </summary>
    public enum PortState
    {
        Unknown = 0,

        Open = 1,

        Closed = 2,

        TimedOut = 3,

        Refused = 4
    }
}
using System;
using System.Linq;

namespace codingfreaks.cfUtils.Logic.Redis.Interfaces
{
    using System;
    using System.Linq;

    /// <summary>
    /// Must be implemented by all types that can be stored in Redis.
    /// </summary>
    public interface IRedisEntity
    {
        #region properties

        /// <summary>
        /// The entity id.
        /// </summary>
        long Id { get; set; }

        /// <summary>
        /// The entity key.
        /// </summary>
        string RedisKey { get; }

        #endregion
    }
}
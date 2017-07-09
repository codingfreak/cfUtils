using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace codingfreaks.cfUtils.Logic.Redis.BaseTypes
{
    using Interfaces;
    /// <summary>
    /// Abstract base class for all Redis entities.
    /// </summary>
    public abstract class BaseRedisEntity : IRedisEntity
    {
        /// <inheritdoc />
        public long Id { get; set; }

        /// <inheritdoc />
        public string RedisKey => $"urn:{GetType().Name.ToLower()}:{Id}";
    }
}

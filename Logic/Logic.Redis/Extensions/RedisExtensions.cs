using System;
using System.Linq;

namespace codingfreaks.cfUtils.Logic.Redis.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Interfaces;

    using ServiceStack.Redis;
    using ServiceStack.Redis.Generic;

    /// <summary>
    /// The client wraps RedisExtensions operations into a more readable and simpler c# API.
    /// </summary>
    public static class RedisExtensions
    {
        #region methods

        /// <summary>
        /// The entry should expire in a certain point of time.
        /// </summary>
        /// <typeparam name="T">The type of the entity which the operation targets.</typeparam>
        /// <param name="entity">The entity which should be traged.</param>
        /// <param name="days">Expires in days.</param>
        /// <param name="hours">Expires in hours.</param>
        /// <param name="minutes">Expires in minutes.</param>
        /// <param name="secounds">Expires in secounds.</param>
        /// <param name="client">The Redis client instance to use.</param>
        /// <returns><c>true</c> if the entity successfully setted.</returns>
        public static bool RedisExpireEntryIn<T>(this T entity, int days, int hours, int minutes, int secounds, IRedisTypedClient<T> client)
            where T : IRedisEntity
        {
            try
            {
                var expiresAt = new TimeSpan(days, hours, minutes, secounds);
                client.ExpireEntryIn(entity.RedisKey, expiresAt);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// The entry should expire in a certain point of time.
        /// </summary>
        /// <typeparam name="T">The type of the entity which the operation targets.</typeparam>
        /// <param name="entity">The entity which should be traged.</param>
        /// <param name="days">Expires in days.</param>
        /// <param name="hours">Expires in hours.</param>
        /// <param name="minutes">Expires in minutes.</param>
        /// <param name="secounds">Expires in secounds.</param>
        /// <param name="client">The Redis client instance to use.</param>
        public static bool RedisExpireEntryIn<T>(this T entity, int days, int hours, int minutes, int secounds, RedisClient client)
            where T : IRedisEntity
        {
            try
            {
                var typedClient = client.As<T>();
                return entity.RedisExpireEntryIn(days, hours, minutes, secounds, typedClient);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// The entry should expire in a certain point of time.
        /// </summary>
        /// <typeparam name="T">The type of the entity which the operation targets.</typeparam>
        /// <param name="entity">The entity which should be traged.</param>
        /// <param name="hours">Expires in hours.</param>
        /// <param name="minutes">Expires in minutes.</param>
        /// <param name="secounds">Expires in secounds.</param>
        /// <param name="client">The Redis client instance to use.</param>
        /// <returns><c>true</c> if the entity successfully setted.</returns>
        public static bool RedisExpireEntryIn<T>(this T entity, int hours, int minutes, int secounds, IRedisTypedClient<T> client)
            where T : IRedisEntity
        {
            try
            {
                var expiresAt = new TimeSpan(hours, minutes, secounds);
                client.ExpireEntryIn(entity.RedisKey, expiresAt);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// The entry should expire in a certain point of time.
        /// </summary>
        /// <typeparam name="T">The type of the entity which the operation targets.</typeparam>
        /// <param name="entity">The entity which should be traged.</param>
        /// <param name="hours">Expires in hours.</param>
        /// <param name="minutes">Expires in minutes.</param>
        /// <param name="secounds">Expires in secounds.</param>
        /// <param name="client">The Redis client instance to use.</param>
        public static bool RedisExpireEntryIn<T>(this T entity, int hours, int minutes, int secounds, RedisClient client)
            where T : IRedisEntity
        {
            try
            {
                var typedClient = client.As<T>();
                return entity.RedisExpireEntryIn(hours, minutes, secounds, typedClient);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Get all elements from Redis cahce.
        /// </summary>
        /// <typeparam name="T">The type of the entity which the operation targets.</typeparam>
        /// <param name="client">The Redis client instance to use.</param>
        /// <returns>All elements of the type <typeparamref name="T" />.</returns>
        public static IList<T> RedisGetAll<T>(this IRedisTypedClient<T> client)
            where T : IRedisEntity
        {
            try
            {
                return client.GetAll();
            }
            catch
            {
                return default(IList<T>);
            }
        }

        /// <summary>
        /// Get all elements from Redis cahce, which belong to an entity
        /// </summary>
        /// <typeparam name="T">The type of the entity which the operation targets.</typeparam>
        /// <param name="client">The Redis client instance to use.</param>
        /// <returns>All elements.</returns>
        public static IList<T> RedisGetAll<T>(this RedisClient client)
            where T : IRedisEntity
        {
            try
            {
                var typedClient = client.As<T>();
                return typedClient.GetAll();
            }
            catch
            {
                return default(IList<T>);
            }
        }

        /// <summary>
        /// Get an element by id from Redis cache.
        /// </summary>
        /// <typeparam name="T">The type of the entity which the operation targets.</typeparam>
        /// <param name="client">The Redis client instance to use.</param>
        /// <param name="id">The id of the entity which should be called.</param>
        /// <returns>The element.</returns>
        public static T RedisGetById<T>(this IRedisTypedClient<T> client, long id)
            where T : IRedisEntity
        {
            try
            {
                return client.GetById(id);
            }
            catch
            {
                return default(T);
            }
        }

        /// <summary>
        /// Get an element by id from redis cache, which belong to an entity.
        /// </summary>
        /// <typeparam name="T">The type of the entity which the operation targets.</typeparam>
        /// <param name="client">The Redis client instance to use.</param>
        /// <param name="id">The id of the entity which should be called.</param>
        /// <returns>The element.</returns>
        public static T RedisGetById<T>(this RedisClient client, long id)
            where T : IRedisEntity
        {
            try
            {
                var typedClient = client.As<T>();
                return typedClient.GetById(id);
            }
            catch
            {
                return default(T);
            }
        }

        /// <summary>
        /// Removes an element from a Redis cache.
        /// </summary>
        /// <typeparam name="T">The type of the entity which the operation targets.</typeparam>
        /// <param name="entity">The entity which should be removed.</param>
        /// <param name="client">The Redis client instance to use.</param>
        /// <returns><c>true</c> if the element successfully removed.</returns>
        public static bool RedisRemove<T>(this T entity, IRedisTypedClient<T> client)
            where T : IRedisEntity
        {
            try
            {
                client.RemoveEntry(entity.RedisKey);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Remove an element from redis cache, which belong to an entity.
        /// </summary>
        /// <typeparam name="T">The type of the entity which the operation targets.</typeparam>
        /// <param name="entity">The entity which should be removed.</param>
        /// <param name="client">The Redis client instance to use.</param>
        public static bool RedisRemove<T>(this T entity, RedisClient client)
            where T : IRedisEntity
        {
            try
            {
                var typedClient = client.As<T>();
                return entity.RedisRemove(typedClient);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Deletes all elements from redis cache and removes the database.
        /// </summary>
        /// <param name="client">The Redis client instance to use.</param>
        public static bool RedisRemoveDatabase(this IRedisNativeClient client)
        {
            try
            {
                client.FlushDb();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Deletes all elements from redis cache.
        /// </summary>
        /// <param name="client">The Redis client instance to use.</param>
        public static bool RedisRemoveEntities(this IRedisNativeClient client)
        {
            try
            {
                client.FlushAll();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Store an element in redis cache.
        /// </summary>
        /// <typeparam name="T">The type of the entity which the operation targets.</typeparam>
        /// <param name="entity">The entity which should be stored.</param>
        /// <param name="client">The Redis client instance to use.</param>
        /// <returns><c>true</c> if the entity is successfully stored.</returns>
        public static bool RedisStore<T>(this T entity, IRedisTypedClient<T> client)
            where T : IRedisEntity
        {
            try
            {
                client.Store(entity);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Store an element in redis cache, which belong to an entity.
        /// </summary>
        /// <typeparam name="T">The type of the entity which the operation targets.</typeparam>
        /// <param name="entity">The entity which should be stored.</param>
        /// <param name="client">The Redis client instance to use.</param>
        public static bool RedisStore<T>(this T entity, RedisClient client)
            where T : IRedisEntity
        {
            try
            {
                var typedClient = client.As<T>();
                return entity.RedisStore(typedClient);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Store a list of elements in redis cache.
        /// </summary>
        /// <typeparam name="T">The type of the entity which the operation targets.</typeparam>
        /// <param name="entities">Entities which should be stored.</param>
        /// <param name="client">The Redis client instance to use.</param>
        /// <returns><c>true</c> if the list successfully stored.</returns>
        public static bool RedisStoreList<T>(this IEnumerable<T> entities, IRedisTypedClient<T> client)
            where T : IRedisEntity
        {
            try
            {
                client.StoreAll(entities);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Store a list of elements in redis cache, which belong to an entity.
        /// </summary>
        /// <typeparam name="T">The type of the entity which the operation targets.</typeparam>
        /// <param name="entities">Entities which should be stored.</param>
        /// <param name="client">The Redis client instance to use.</param>
        public static bool RedisStoreList<T>(this IEnumerable<T> entities, RedisClient client)
            where T : IRedisEntity
        {
            try
            {
                var typedClient = client.As<T>();
                return entities.RedisStoreList(typedClient);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Update an element in redis cache.
        /// </summary>
        /// <typeparam name="T">The type of the entity which the operation targets.</typeparam>
        /// <param name="entity">The entity which should be updated.</param>
        /// <param name="client">The Redis client instance to use.</param>
        /// <returns><c>true</c> if the element is successfully updated.</returns>
        public static bool RedisUpdate<T>(this T entity, IRedisTypedClient<T> client)
            where T : IRedisEntity
        {
            try
            {
                client.GetAndSetValue(entity.RedisKey, entity);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Update an element in redis cache, which belong to an entity.
        /// </summary>
        /// <typeparam name="T">The type of the entity which the operation targets.</typeparam>
        /// <param name="entity">The entity which should be updated.</param>
        /// <param name="client">The Redis client instance to use.</param>
        /// <returns><c>true</c> if the element is successfully updated.</returns>
        public static bool RedisUpdate<T>(this T entity, RedisClient client)
            where T : IRedisEntity
        {
            try
            {
                var typedClient = client.As<T>();
                return entity.RedisUpdate(typedClient);
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}
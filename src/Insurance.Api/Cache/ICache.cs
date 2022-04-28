using System;
using System.Threading.Tasks;

namespace Insurance.Api.Cache
{
    /// <summary>
    /// Contract for content caching.
    /// </summary>
    public interface ICache
    {
        /// <summary>
        /// Gets or sets entry produced by given factory into cache with
        /// provided expiration in milliseconds or cache default expiration.
        /// </summary>
        /// <param name="key">The key for entry to be cached.</param>
        /// <param name="factory">The factory which generates value to be cached.</param>
        /// <param name="expirationInMilliseconds">Time in milliseconds to keep
        /// entry in cache.</param>
        /// <typeparam name="T">Type of object produced by factory.</typeparam>
        /// <returns>A <see cref="Task{T}"/> containing the object
        /// fetched by given key if exists otherwise the object
        /// produced by given factory.</returns>
        Task<T> GetOrSetEntry<T>(string key, Func<Task<T>> factory, int? expirationInMilliseconds  = null);
    }
}
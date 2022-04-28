using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace Insurance.Api.Cache
{
    /// <summary>
    /// In memory cache.
    /// </summary>
    public class MemoryCache : ICache
    {
        private readonly IMemoryCache internalCache;
        private readonly CacheOptions options;

        /// <summary>
        /// Initializes a new instance of <see cref="MemoryCache"/>
        /// </summary>
        /// <param name="internalCache">Internal in memory cache for <see cref="MemoryCache"/>.</param>
        /// <param name="options">Cache options.</param>
        public MemoryCache(IMemoryCache internalCache, CacheOptions options)
        {
            this.internalCache = internalCache;
            this.options = options;
        }

        /// <inheritdoc/>
        public async Task<T> GetOrSetEntry<T>(string key, Func<Task<T>> factory, int? expirationInMilliseconds  = null)
        {
            Func<ICacheEntry, Task<string>> func;
            var result = await internalCache.GetOrCreateAsync<T>(key, (entry) =>
            {
                var expiration = expirationInMilliseconds ?? options.DefaultExpirationInMilliseconds;
                
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMilliseconds(expiration);

                return factory();
            });
            return result;
        }
    }
}
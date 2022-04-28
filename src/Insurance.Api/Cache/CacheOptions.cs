namespace Insurance.Api.Cache
{
    /// <summary>
    /// Options to control caching.
    /// </summary>
    public class CacheOptions
    {
        /// <summary>
        /// Time in milliseconds to keep entries in cache.
        /// </summary>
        public int DefaultExpirationInMilliseconds { get; set; }
    }
}
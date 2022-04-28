namespace Insurance.Api.Models
{
    /// <summary>
    /// Configuration values for application.
    /// </summary>
    public class AppConfiguration 
    {
        /// <summary>
        /// Url of Product Api.
        /// </summary>
        public string ProductApi {get;set;}
        
        /// <summary>
        /// Determines how http client should behave in case of failures.
        /// </summary>
        public FaultToleranceConfiguration FaultTolerance { get; set; }
        
        /// <summary>
        /// Time in milliseconds to keep Product Api responses in cache.
        /// </summary>
        public int ResponseCacheExpirationInMilliseconds { get; set; }

        /// <summary>
        /// Determines if caching is enabled.
        /// </summary>
        public bool ResponseCachingEnabled { get; set; }
    }
}
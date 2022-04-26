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
    }
}
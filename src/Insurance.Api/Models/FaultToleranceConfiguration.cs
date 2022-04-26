namespace Insurance.Api.Models
{
    /// <summary>
    /// Configurations for retry and circuit breaker policies.
    /// </summary>
    public class FaultToleranceConfiguration
    {
        /// <summary>
        /// Totals seconds that circuit breaker should stay open.
        /// </summary>
        public int DurationOfBreakInSeconds { get; set; }
        
        /// <summary>
        /// Determines if circuit breaker policy for http client must be enabled.
        /// </summary>
        public bool CircuitBreakerEnabled { get; set; }

        /// <summary>
        /// Number of events to be handled before opening the circuit.
        /// </summary>
        public int HandledEventsAllowedBeforeBreaking { get; set; }

        /// <summary>
        /// Determines if retry policy for http client must be enabled.
        /// </summary>
        public bool RetryPolicyEnabled { get; set; }

        /// <summary>
        /// Number of retries for http client in case of failures.
        /// </summary>
        public int RetryCount { get; set; }
    }
}
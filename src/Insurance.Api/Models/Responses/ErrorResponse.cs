namespace Insurance.Api.Models.Responses
{
    /// <summary>
    /// Response model for request with error.
    /// </summary>
    public class ErrorResponse
    {
        public ErrorResponse(string message)
        {
            Message = message;
        }
        
        /// <summary>
        /// Indicates failure of the request processing.
        /// </summary>
        public bool Success { get; set; } = false;
        
        /// <summary>
        /// Diagnostic message about failure.
        /// </summary>
        public string Message { get; set; }
    }
}
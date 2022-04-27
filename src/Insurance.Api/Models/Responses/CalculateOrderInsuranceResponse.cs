namespace Insurance.Api.Models.Responses
{
    /// <summary>
    /// Response model for calculating order insurance.
    /// </summary>
    public class CalculateOrderInsuranceResponse
    {
        /// <summary>
        /// Calculated value for order insurance.
        /// </summary>
        public  float InsuranceValue { get; set; }
    }
}
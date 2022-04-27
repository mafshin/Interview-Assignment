namespace Insurance.Api.Models.Responses
{
    /// <summary>
    /// Response model for calculated product insurance.
    /// </summary>
    public class CalculateProductInsuranceResponse
    {     
        /// <summary>
        /// The id of the product for which insurance is calculated.
        /// </summary>
        public int ProductId { get; set; }
        
        /// <summary>
        /// The calculated insurance value for the product.
        /// </summary>
        public float InsuranceValue { get; set; }
    }
}
namespace Insurance.Api.Models.Requests
{
    /// <summary>
    /// Request model for calculating product insurance.
    /// </summary>
    public class CalculateProductInsuranceRequest
    {
        /// <summary>
        /// The id of the product for which insurance is calculated.
        /// </summary>
        public int ProductId { get; set; }
    }
}
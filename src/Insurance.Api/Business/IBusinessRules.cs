using System.Threading.Tasks;
using Insurance.Api.Models;
using Insurance.Api.Models.Dto;

namespace Insurance.Api.Business
{
    /// <summary>
    /// Contract for the business rules of insurance calculation.
    /// </summary>
    public interface IBusinessRules
    {
        /// <summary>
        /// Calculates the insurance value for the given product request.
        /// </summary>
        /// <param name="toInsure">Product info</param>
        /// <returns>A <see cref="Task{float}"/> containing the insurance value
        /// for the given product.</returns>
        Task<float> CalculateProductInsurance(ProductInfoDto toInsure);
        
        /// <summary>
        /// Calculates the insurance for the given order request.
        /// </summary>
        /// <param name="order">Order request to calculate its insurance.</param>
        /// <returns>A <see cref="Task{float}"/> including the insurance value
        /// for the given order request.</returns>
        Task<float> CalculateOrderInsurance(OrderInsuranceDto order);
    }
}
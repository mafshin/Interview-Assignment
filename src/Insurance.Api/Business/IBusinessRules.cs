using System.Threading.Tasks;
using Insurance.Api.Controllers;

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
        /// <param name="toInsure"></param>
        /// <returns></returns>
        Task<HomeController.InsuranceDto> CalculateProductInsurance(HomeController.InsuranceDto toInsure);
        
        /// <summary>
        /// Calculates the insurance for the given order request.
        /// </summary>
        /// <param name="order">Order request to calculate its insurance.</param>
        /// <returns>A <see cref="Task{float}"/> including the insurance value
        /// for the given order request.</returns>
        Task<float> CalculateOrderInsurance(HomeController.OrderInsuranceDto order);
    }
}
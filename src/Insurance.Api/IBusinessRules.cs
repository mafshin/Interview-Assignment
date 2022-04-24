using System.Threading.Tasks;
using Insurance.Api.Controllers;

namespace Insurance.Api
{
    public interface IBusinessRules
    {
        Task<HomeController.InsuranceDto> CalculateProductInsurance(HomeController.InsuranceDto toInsure);
        Task<float> CalculateOrderInsurance(HomeController.OrderInsuranceDto dto);
    }
}
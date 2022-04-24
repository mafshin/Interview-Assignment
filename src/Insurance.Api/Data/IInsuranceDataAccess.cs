using System.Threading.Tasks;
using Insurance.Api.Models;

namespace Insurance.Api.Data
{
    public interface IInsuranceDataAccess
    {
        Task UpdateProductTypeSurcharges(ProductTypeSurcharge[] surcharges);
        Task<float?> GetSurchargeByProductTypeId(int productTypeId);
    }
}
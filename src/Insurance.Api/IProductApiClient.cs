using System.Threading.Tasks;
using Insurance.Api.Models;

namespace Insurance.Api
{
    public interface IProductApiClient
    {
        Task<ProductType> GetProductTypeByProductId(int productID);

        Task<float> GetSalesPriceByProductId(int productID);

        Task<ProductType> GetProductTypeById(int productTypeId);

        Task<dynamic> GetProductById(int productID);

    }
}
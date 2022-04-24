using System.Threading.Tasks;
using Insurance.Api.Models;

namespace Insurance.Api.Clients
{
    public interface IProductApiClient
    {
        Task<ProductType> GetProductTypeById(int productTypeId);

        Task<Product> GetProductById(int productID);

    }
}
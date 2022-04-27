using System.Threading.Tasks;
using Insurance.Api.Models;

namespace Insurance.Api.Clients
{
    /// <summary>
    /// Contract for the client of Product Api.
    /// </summary>
    public interface IProductApiClient
    {
        /// <summary>
        /// Gets the product type by given id.
        /// </summary>
        /// <param name="productTypeId">Id of the product type.</param>
        /// <returns>A <see cref="Task{ProductType}"/> including the <see cref="ProductType"/>
        /// for the given id.</returns>
        Task<ProductType> GetProductTypeById(int productTypeId);

        /// <summary>
        /// Gets the product by given id.
        /// </summary>
        /// <param name="productId">Id of the product.</param>
        /// <returns>A <see cref="Task{Product}"/> including the <see cref="Product"/>
        /// for the given id.</returns>
        Task<Product> GetProductById(int productId);

    }
}
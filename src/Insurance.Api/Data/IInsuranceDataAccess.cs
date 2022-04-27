using System.Threading.Tasks;
using Insurance.Api.Models;

namespace Insurance.Api.Data
{
    /// <summary>
    /// Contract for Insurance Data Access.
    /// </summary>
    public interface IInsuranceDataAccess
    {
        /// <summary>
        /// Update surcharge rates for product types.
        /// </summary>
        /// <param name="surcharges">The surcharge rates to be updated.</param>
        /// <returns>A <see cref="Task"/> to await the operation.</returns>
        Task UpdateProductTypeSurcharges(ProductTypeSurcharge[] surcharges);
        
        /// <summary>
        /// Gets surcharge rate for the given product type id.
        /// </summary>
        /// <param name="productTypeId">Id of the product type to get its surcharge rate.</param>
        /// <returns>A <see cref="Task{float?}"/> with the surcharge rate of the given product id.</returns>
        Task<float?> GetSurchargeByProductTypeId(int productTypeId);
    }
}
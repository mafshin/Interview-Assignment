using System.Collections.Concurrent;
using System.Threading.Tasks;
using Insurance.Api.Models;

namespace Insurance.Api.Data
{
    public class InsuranceDataAccess : IInsuranceDataAccess
    {
        private ConcurrentDictionary<int, ProductTypeSurcharge> productTypeSurcharges =
            new ConcurrentDictionary<int, ProductTypeSurcharge>();

        public Task UpdateProductTypeSurcharges(ProductTypeSurcharge[] surcharges)
        {
            foreach (var item in surcharges)
            {
                var newValue = new ProductTypeSurcharge()
                {
                    ProductTypeId = item.ProductTypeId,
                    Surcharge = item.Surcharge
                };

                productTypeSurcharges.AddOrUpdate(item.ProductTypeId,
                    (productTypeId) => newValue,
                    (productTypeId, oldValue) => newValue);
            }

            return Task.CompletedTask;
        }

        public Task<float?> GetSurchargeByProductTypeId(int productTypeId)
        {
            if (productTypeSurcharges.TryGetValue(productTypeId, out var productTypeSurcharge))
            {
                return Task.FromResult((float?) productTypeSurcharge.Surcharge);
            }
            else
            {
                return Task.FromResult((float?) null);
            }
        }
    }
}
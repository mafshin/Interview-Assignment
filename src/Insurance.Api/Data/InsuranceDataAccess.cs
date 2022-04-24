using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Insurance.Api.Models;

namespace Insurance.Api.Data
{
    public class InsuranceDataAccess : IInsuranceDataAccess
    {
        private ConcurrentDictionary<int, ProductTypeSurcharge> productTypeSurcharges =
            new ConcurrentDictionary<int, ProductTypeSurcharge>();

        public async Task UpdateProductTypeSurcharges(ProductTypeSurcharge[] surcharges)
        {
            foreach (var item in surcharges)
            {
                var newValue = new ProductTypeSurcharge()
                {
                    ProductTypeId = item.ProductTypeId,
                    Surcharge = item.Surcharge
                };

                if (productTypeSurcharges.TryGetValue(item.ProductTypeId, out var productTypeSurcharge))
                {
                    var updateResult =
                        productTypeSurcharges.TryUpdate(item.ProductTypeId, newValue, productTypeSurcharge);

                    if (!updateResult)
                    {
                        throw new InvalidOperationException("Failed to update product type surcharge");
                    }
                }
                else
                {
                    var addResult = productTypeSurcharges.TryAdd(item.ProductTypeId, newValue);

                    if (!addResult)
                    {
                        throw new InvalidOperationException("Failed to set product type surcharge");
                    }
                }
            }
        }

        public async Task<float?> GetSurchargeByProductTypeId(int productTypeId)
        {
            if (productTypeSurcharges.TryGetValue(productTypeId, out var productTypeSurcharge))
            {
                return productTypeSurcharge.Surcharge;
            }
            else
            {
                return null;
            }
        }
    }
}
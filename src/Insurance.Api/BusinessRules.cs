using System;
using System.Threading.Tasks;
using Insurance.Api.Clients;
using Insurance.Api.Controllers;
using Insurance.Api.Data;
using Insurance.Api.Models;

namespace Insurance.Api
{
    public class BusinessRules : IBusinessRules
    {
        private readonly IProductApiClient productApiClient;
        private readonly IInsuranceDataAccess insuranceDataAccess;

        public BusinessRules(IProductApiClient productApiClient, IInsuranceDataAccess insuranceDataAccess)
        {
            this.productApiClient = productApiClient;
            this.insuranceDataAccess = insuranceDataAccess;
        }
      
        public async Task<HomeController.InsuranceDto> CalculateProductInsurance(HomeController.InsuranceDto toInsure)
        {
            var newInsurance = await GetProductInfo(toInsure.ProductId);

            float insurance =  CalculateProductInsuranceValue(newInsurance);
            
            newInsurance.InsuranceValue = await ApplySurcharge(newInsurance.ProductTypeId, insurance).ConfigureAwait(false);

            return newInsurance;
        }

        public async Task<float> CalculateOrderInsurance(HomeController.OrderInsuranceDto dto)
        {
            float totalInsurance = 0;
            bool isDigitalCameraCheckDone = false; 
            
            foreach (var item in dto.OrderItems)
            {
                var newInsurance = await GetProductInfo(item.ProductId);
                
                float insurance =  CalculateProductInsuranceValue(newInsurance);

                float productInsurance  = insurance * item.Quantity;

                if (!isDigitalCameraCheckDone && newInsurance.ProductTypeHasInsurance && newInsurance.ProductTypeName == "Digital cameras")
                {
                    productInsurance += 500;
                    isDigitalCameraCheckDone = true;
                }

                productInsurance = await ApplySurcharge(newInsurance.ProductTypeId, productInsurance).ConfigureAwait(false);

                totalInsurance += productInsurance;
            }

            return totalInsurance;
        }

        private async Task<HomeController.InsuranceDto> GetProductInfo(int productId)
        {
            var newInsurance = new HomeController.InsuranceDto
            {
                ProductId = productId
            };
            
            Product product = await productApiClient.GetProductById(productId).ConfigureAwait(false);

            var productType = await productApiClient.GetProductTypeById(product.ProductTypeId);
            newInsurance.ProductTypeName = productType.Name;
            newInsurance.ProductTypeHasInsurance = productType.CanBeInsured;
            newInsurance.SalesPrice = product.SalesPrice;
            newInsurance.ProductTypeId = product.ProductTypeId;
            
            return newInsurance;
        }

        private async Task<float> ApplySurcharge(int productTypeId, float insurance)
        {
            if (insurance > 0)
            {
                var productSurcharge = await insuranceDataAccess.GetSurchargeByProductTypeId(productTypeId)
                    .ConfigureAwait(false);

                float surchargeRate = productSurcharge.GetValueOrDefault();

                if (surchargeRate > 0)
                {
                    insurance = (float) Math.Round(insurance * (1 + surchargeRate));
                }
            }

            return insurance;
        }
        
        private float CalculateProductInsuranceValue(HomeController.InsuranceDto toInsure)
        {
            float insurance = 0f;

            if (toInsure.ProductTypeHasInsurance)
            {
                if (toInsure.SalesPrice < 500)
                    insurance = 0;
                else
                {
                    if (toInsure.SalesPrice > 500 && toInsure.SalesPrice < 2000)
                        insurance += 1000;
                    if (toInsure.SalesPrice >= 2000)
                        insurance += 2000;
                }

                if (toInsure.ProductTypeName == "Laptops" || toInsure.ProductTypeName == "Smartphones")
                    insurance += 500;
            }

            return insurance;
        }

    }
}
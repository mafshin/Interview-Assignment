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
            var newInsurance = new HomeController.InsuranceDto
            {
                ProductId = toInsure.ProductId,
            };
            
            int productId = newInsurance.ProductId;
            Product product = await productApiClient.GetProductById(productId).ConfigureAwait(false);

            var productType = await productApiClient.GetProductTypeById(product.ProductTypeId);
            newInsurance.ProductTypeName = productType.Name;
            newInsurance.ProductTypeHasInsurance = productType.CanBeInsured;
            newInsurance.SalesPrice = product.SalesPrice;
            newInsurance.ProductTypeId = product.ProductTypeId;

            float insurance = await CalculateProductInsuranceValue(newInsurance);
            newInsurance.InsuranceValue = insurance;

            return newInsurance;
        }
        
        public async Task<float> CalculateOrderInsurance(HomeController.OrderInsuranceDto dto)
        {
            float totalInsurance = 0;
            bool isDigitalCameraCheckDone = false; 

            foreach (var item in dto.OrderItems)
            {
                var toInsure = new HomeController.InsuranceDto()
                {
                    ProductId = item.ProductId
                };

                toInsure = await CalculateProductInsurance(toInsure).ConfigureAwait(false);

                totalInsurance += toInsure.InsuranceValue * item.Quantity;

                if (!isDigitalCameraCheckDone && toInsure.ProductTypeHasInsurance && toInsure.ProductTypeName == "Digital cameras")
                {
                    totalInsurance += 500;
                    isDigitalCameraCheckDone = true;
                }
            }

            return totalInsurance;
        }
        
        private async Task<float> CalculateProductInsuranceValue(HomeController.InsuranceDto toInsure)
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

                var productSurcharge = await insuranceDataAccess.GetSurchargeByProductTypeId(toInsure.ProductTypeId);

                insurance += productSurcharge.GetValueOrDefault();
            }

            return insurance;
        }

    }
}
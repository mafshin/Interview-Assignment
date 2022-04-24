using System.Threading.Tasks;
using Insurance.Api.Clients;
using Insurance.Api.Controllers;
using Insurance.Api.Models;

namespace Insurance.Api
{
    public class BusinessRules 
    {
        public static async Task<HomeController.InsuranceDto> CalculateProductInsurance(IProductApiClient productApiClient, HomeController.InsuranceDto toInsure)
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

            float insurance = CalculateProductInsuranceValue(newInsurance);
            newInsurance.InsuranceValue = insurance;

            return newInsurance;
        }
        
        public static async Task<float> CalculateOrderInsurance(IProductApiClient productApiClient, HomeController.OrderInsuranceDto dto)
        {
            float totalInsurance = 0;
            bool isDigitalCameraCheckDone = false; 

            foreach (var item in dto.OrderItems)
            {
                var toInsure = new HomeController.InsuranceDto()
                {
                    ProductId = item.ProductId
                };

                toInsure = await CalculateProductInsurance(productApiClient, toInsure).ConfigureAwait(false);

                totalInsurance += toInsure.InsuranceValue * item.Quantity;

                if (!isDigitalCameraCheckDone && toInsure.ProductTypeHasInsurance && toInsure.ProductTypeName == "Digital cameras")
                {
                    totalInsurance += 500;
                    isDigitalCameraCheckDone = true;
                }
            }

            return totalInsurance;
        }
        
        private static float CalculateProductInsuranceValue(HomeController.InsuranceDto toInsure)
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
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Insurance.Api.Clients;
using Insurance.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Insurance.Api.Controllers
{
    public class HomeController : BaseController<HomeController>
    {
        private readonly IProductApiClient productApiClient;

        public HomeController(IOptions<AppConfiguration> appConfiguration, ILogger<HomeController> logger,
            IProductApiClient productApiClient)
            : base(appConfiguration, logger)
        {
            this.productApiClient = productApiClient;
        }

        [HttpPost]
        [Route("api/insurance/product")]
        public async Task<InsuranceDto> CalculateInsurance([FromBody] InsuranceDto toInsure)
        {
            if (toInsure == null)
            {
                throw new ArgumentNullException(nameof(toInsure));
            }

            toInsure = await CalculateProductInsurance(toInsure);

            return toInsure;
        }

        [HttpPost]
        [Route("api/insurnace/order")]
        public async Task<float> CalculateOrderInsurance(OrderInsuranceDto dto)
        {
            float totalInsurance = 0;

            foreach (var item in dto.OrderItems)
            {
                var toInsure = new InsuranceDto()
                {
                    ProductId = item.ProductId
                };

                toInsure = await CalculateProductInsurance(toInsure).ConfigureAwait(false);

                totalInsurance += toInsure.InsuranceValue * item.Quantity;
            }

            return totalInsurance;
        }

        private async Task<InsuranceDto> CalculateProductInsurance(InsuranceDto toInsure)
        {
            var newInsurance = new InsuranceDto
            {
                ProductId = toInsure.ProductId,
            };
            
            int productId = newInsurance.ProductId;

            var productType = await productApiClient.GetProductTypeByProductId(productId).ConfigureAwait(false);
            newInsurance.ProductTypeName = productType.Name;
            newInsurance.ProductTypeHasInsurance = productType.CanBeInsured;

            var salesPrice = await productApiClient.GetSalesPriceByProductId(productId).ConfigureAwait(false);
            newInsurance.SalesPrice = salesPrice;

            float insurance = BusinessRules.CalculateProductInsuranceValue(newInsurance);
            newInsurance.InsuranceValue = insurance;

            return newInsurance;
        }

        public class InsuranceDto
        {
            public int ProductId { get; set; }
            public float InsuranceValue { get; set; }
            [JsonIgnore] public string ProductTypeName { get; set; }
            [JsonIgnore] public bool ProductTypeHasInsurance { get; set; }
            [JsonIgnore] public float SalesPrice { get; set; }
        }

        public class OrderInsuranceDto
        {
            public IEnumerable<OrderItemDto> OrderItems { get; set; }
        }

        public class OrderItemDto
        {
            public int ProductId { get; set; }
            public float Quantity { get; set; }
        }
    }
}
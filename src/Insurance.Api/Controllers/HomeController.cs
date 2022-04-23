using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Insurance.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Insurance.Api.Controllers
{
    public class HomeController: BaseController<HomeController>
    {
        private readonly IProductApiClient productApiClient;

        public HomeController(IOptions<AppConfiguration> appConfiguration, ILogger<HomeController> logger, IProductApiClient productApiClient) 
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

            int productId = toInsure.ProductId;

            var productType = await productApiClient.GetProductTypeByProductId(productId).ConfigureAwait(false);
            toInsure.ProductTypeName = productType.Name;
            toInsure.ProductTypeHasInsurance = productType.CanBeInsured;

            var salesPrice = await productApiClient.GetSalesPriceByProductId(productId).ConfigureAwait(false);
            toInsure.SalesPrice = salesPrice;
            
            float insurance = BusinessRules.CalculateInsuranceValue(toInsure);
            toInsure.InsuranceValue = insurance;

            return toInsure;
        }

        public class InsuranceDto
        {
            public int ProductId { get; set; }
            public float InsuranceValue { get; set; }
            [JsonIgnore]
            public string ProductTypeName { get; set; }
            [JsonIgnore]
            public bool ProductTypeHasInsurance { get; set; }
            [JsonIgnore]
            public float SalesPrice { get; set; }
        }
    }
}
using System;
using System.Net;
using System.Net.Http;
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
        private readonly IHttpClientFactory httpClientFactory;
        public HomeController(IOptions<AppConfiguration> appConfiguration, ILogger<HomeController> logger, IHttpClientFactory httpClientFactory) 
            : base(appConfiguration, logger)
        {
            this.httpClientFactory = httpClientFactory;
        }

        [HttpPost]
        [Route("api/insurance/product")]
        public async Task<InsuranceDto> CalculateInsurance([FromBody] InsuranceDto toInsure)
        {
            if(toInsure == null)
            {
                throw new ArgumentNullException(nameof(toInsure));
            }

            int productId = toInsure.ProductId;

            var productType = await BusinessRules.GetProductType(httpClientFactory, productId).ConfigureAwait(false);
            toInsure.ProductTypeName = productType.Name;
            toInsure.ProductTypeHasInsurance = productType.CanBeInsured;
    
            var salesPrice =  await BusinessRules.GetSalesPrice(httpClientFactory, productId).ConfigureAwait(false);
            toInsure.SalesPrice = salesPrice;

            float insurance = 0f;

            if (toInsure.SalesPrice < 500)
                toInsure.InsuranceValue = 0;
            else
            {
                if (toInsure.SalesPrice > 500 && toInsure.SalesPrice < 2000)
                    if (toInsure.ProductTypeHasInsurance)
                        toInsure.InsuranceValue += 1000;
                if (toInsure.SalesPrice >= 2000)
                    if (toInsure.ProductTypeHasInsurance)
                        toInsure.InsuranceValue += 2000;
            }

            if ((toInsure.ProductTypeName == "Laptops" || toInsure.ProductTypeName == "Smartphones") && toInsure.ProductTypeHasInsurance)
                toInsure.InsuranceValue += 500;

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
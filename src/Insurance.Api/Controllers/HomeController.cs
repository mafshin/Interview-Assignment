using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Insurance.Api.Clients;
using Insurance.Api.Data;
using Insurance.Api.Models;
using Insurance.Api.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Insurance.Api.Controllers
{
    public class HomeController : BaseController<HomeController>
    {
        private readonly IInsuranceDataAccess insuranceDataAccess;
        private readonly IBusinessRules businessRules;

        public HomeController(IOptions<AppConfiguration> appConfiguration, ILogger<HomeController> logger,
            IInsuranceDataAccess insuranceDataAccess, IBusinessRules businessRules)
            : base(appConfiguration, logger)
        {
            this.insuranceDataAccess = insuranceDataAccess;
            this.businessRules = businessRules;
        }

        [HttpPost]
        [Route("api/insurance/product")]
        public async Task<CalculateProductInsuranceResponse> CalculateProductInsurance([FromBody] InsuranceDto toInsure)
        {
            if (toInsure == null)
            {
                throw new ArgumentNullException(nameof(toInsure));
            }

            toInsure = await businessRules.CalculateProductInsurance(toInsure);

            var response = new CalculateProductInsuranceResponse()
            {
                InsuranceValue = toInsure.InsuranceValue,
                ProductId = toInsure.ProductId
            };

            return response;
        }

        [HttpPost]
        [Route("api/insurance/order")]
        public async Task<CalculateOrderInsuranceResponse> CalculateOrderInsurance([FromBody] OrderInsuranceDto orderInsuranceDto)
        {
            if (orderInsuranceDto == null)
            {
                throw new ArgumentException(nameof(orderInsuranceDto));
            }
            
            var totalInsurance = await businessRules.CalculateOrderInsurance(orderInsuranceDto);

            var response = new CalculateOrderInsuranceResponse()
            {
                InsuranceValue = totalInsurance
            };

            return response;
        }

        [HttpPut]
        [Route("api/insurance/surcharge")]
        public async Task SetProductTypeSurcharges([FromBody] ProductTypeSurcharge[] surcharges)
        {
            await insuranceDataAccess.UpdateProductTypeSurcharges(surcharges).ConfigureAwait(false);
        }

        public class InsuranceDto
        {
            public int ProductId { get; set; }
            public float InsuranceValue { get; set; }
            [JsonIgnore] public string ProductTypeName { get; set; }
            [JsonIgnore] public bool ProductTypeHasInsurance { get; set; }
            [JsonIgnore] public float SalesPrice { get; set; }
            public int ProductTypeId { get; set; }
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
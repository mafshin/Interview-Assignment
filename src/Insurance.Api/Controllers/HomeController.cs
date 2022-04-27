using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Insurance.Api.Business;
using Insurance.Api.Clients;
using Insurance.Api.Data;
using Insurance.Api.Models;
using Insurance.Api.Models.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Insurance.Api.Controllers
{
    /// <summary>
    /// Home controller of Insurance Api.
    /// </summary>
    [Route("api/insurance")]
    public class HomeController : BaseController<HomeController>
    {
        private readonly IInsuranceDataAccess insuranceDataAccess;
        private readonly IBusinessRules businessRules;
        private readonly IProductApiClient productApiClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController"/>.
        /// </summary>
        /// <param name="appConfiguration">Application configuration.</param>
        /// <param name="logger">Logger instance.</param>
        /// <param name="insuranceDataAccess">Data access instance for data persistense.</param>
        /// <param name="businessRules">An instance <see cref="IBusinessRules"/> for
        /// calculating insurance values.</param>
        public HomeController(IOptions<AppConfiguration> appConfiguration, ILogger<HomeController> logger,
            IInsuranceDataAccess insuranceDataAccess, IBusinessRules businessRules, IProductApiClient productApiClient)
            : base(appConfiguration, logger)
        {
            this.insuranceDataAccess = insuranceDataAccess;
            this.businessRules = businessRules;
            this.productApiClient = productApiClient;
        }

        /// <summary>
        /// Calculates the insurance for the given request.
        /// </summary>
        /// <param name="toInsure">The request to calculate its instance.</param>
        /// <returns>A <see cref="Task{CalculateProductInsuranceResponse}"/> including the
        /// instance value calculated for the given request.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        [HttpPost]
        [Route("product")]
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

        /// <summary>
        /// Calculates the insurance for the given order.
        /// </summary>
        /// <param name="orderInsuranceDto">Order request to calculate its insurance.</param>
        /// <returns>A <see cref="Task{CalculateOrderInsuranceResonse}"/> including the
        /// insurance value for the given order.</returns>
        /// <exception cref="ArgumentException"></exception>
        [HttpPost]
        [Route("order")]
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

        /// <summary>
        /// Updates the surcharge rate for 
        /// </summary>
        /// <param name="surcharges">The surcharge rates to be updated.</param>
        /// <returns>A <see cref="Task{IActionResult}"/> containing the
        /// response of operation.</returns>
        [HttpPut]
        [Route("surcharge")]
        public async Task<IActionResult> SetProductTypeSurcharges([FromBody] ProductTypeSurcharge[] surcharges)
        {
            IEnumerable<ProductType> productTypes = await productApiClient.GetProductTypes().ConfigureAwait(false);

            var invalidItems = surcharges.Where(x => productTypes.All(p => p.Id != x.ProductTypeId)).ToArray();

            if (invalidItems.Any())
            {
                var message =
                    $"Some ProductTypeIds are invalid: {String.Join(", ", invalidItems.Select(x => x.ProductTypeId))}";
                return this.StatusCode(StatusCodes.Status422UnprocessableEntity, message);
            }
            
            await insuranceDataAccess.UpdateProductTypeSurcharges(surcharges).ConfigureAwait(false);

            return Ok();
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Insurance.Api.Business;
using Insurance.Api.Clients;
using Insurance.Api.Data;
using Insurance.Api.Models;
using Insurance.Api.Models.Dto;
using Insurance.Api.Models.Requests;
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
        /// <param name="insuranceDataAccess">Data access instance for data persistence.</param>
        /// <param name="businessRules">An instance <see cref="IBusinessRules"/> for
        /// calculating insurance values.</param>
        /// <param name="productApiClient">Client for accessing Product Api.</param>
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
        public async Task<IActionResult> CalculateProductInsurance(
            [FromBody] CalculateProductInsuranceRequest toInsure)
        {
            if (toInsure == null)
            {
                return BadRequest();
            }

            var product = await productApiClient.GetProductById(toInsure.ProductId).ConfigureAwait(false);

            if (product is null)
            {
                return StatusCode(StatusCodes.Status422UnprocessableEntity,
                    new ErrorResponse($"Product with id '{toInsure.ProductId}' does not exist"));
            }

            ProductInfoDto productInfoDto = new ProductInfoDto()
            {
                ProductId = toInsure.ProductId
            };

            float insurance = await businessRules.CalculateProductInsurance(productInfoDto).ConfigureAwait(false);

            var response = new CalculateProductInsuranceResponse()
            {
                InsuranceValue = insurance,
                ProductId = toInsure.ProductId
            };

            return Ok(response);
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
        public async Task<IActionResult> CalculateOrderInsurance(
            [FromBody] CalculateOrderInsuranceRequest orderInsuranceDto)
        {
            if (orderInsuranceDto == null || orderInsuranceDto.OrderItems == null)
            {
                return BadRequest();
            }

            var products =
                await Task.WhenAll(
                    orderInsuranceDto.OrderItems.Select(async (x) =>
                        new {x.ProductId, Product = await productApiClient.GetProductById(x.ProductId)}));

            var invalidProducts = products.Where(x => x.Product == null).Select(x => x.ProductId).ToArray();

            if (invalidProducts.Any())
            {
                return StatusCode(StatusCodes.Status422UnprocessableEntity, new ErrorResponse(
                    $"Some product ids are invalid: {(string.Join(", ", invalidProducts))}"));
            }

            OrderInsuranceDto dto = new OrderInsuranceDto()
            {
                OrderItems = orderInsuranceDto.OrderItems.Select(x => new OrderItemDto()
                {
                    ProductId = x.ProductId,
                    Quantity = x.Quantity
                })
            };

            var totalInsurance = await businessRules.CalculateOrderInsurance(dto);

            var response = new CalculateOrderInsuranceResponse()
            {
                InsuranceValue = totalInsurance
            };

            return Ok(response);
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
            if (surcharges == null)
            {
                return BadRequest();
            }
            
            IEnumerable<ProductType> productTypes = await productApiClient.GetProductTypes().ConfigureAwait(false);

            var invalidItems = surcharges.Where(x => productTypes.All(p => p.Id != x.ProductTypeId)).ToArray();

            if (invalidItems.Any())
            {
                var message =
                    $"Some ProductTypeIds are invalid: {String.Join(", ", invalidItems.Select(x => x.ProductTypeId))}";
                return this.StatusCode(StatusCodes.Status422UnprocessableEntity, new ErrorResponse(message));
            }

            await insuranceDataAccess.UpdateProductTypeSurcharges(surcharges).ConfigureAwait(false);

            return Ok();
        }
    }
}
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Insurance.Api.Extensions;
using Insurance.Api.Models;

namespace Insurance.Api.Clients
{
    public class ProductApiClient : IProductApiClient
    {
        private readonly IHttpClientFactory httpClientFactory;

        /// <summary>
        /// Initializes a new instance of <see cref="ProductApiClient"/>.
        /// </summary>
        /// <param name="httpClientFactory">Http Client Factory for creating <see cref="HttpClient"/>
        /// instances.</param>
        public ProductApiClient(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        /// <inheritdoc />
        public async Task<ProductType> GetProductTypeById(int productTypeId)
        {
            HttpClient client = CreateClient();
            var requestUri = $"/product_types/{productTypeId}";
            var productType = await client.GetAsync<ProductType>(requestUri).ConfigureAwait(false);
            return productType;
        }

        /// <inheritdoc />
        public async Task<Product> GetProductById(int productId)
        {
            HttpClient client = CreateClient();
            var requestUri = $"/products/{productId}";
            var product = await client.GetAsync<Product>(requestUri).ConfigureAwait(false);
            return product;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<ProductType>> GetProductTypes()
        {
            HttpClient client = CreateClient();
            var requestUri = $"/product_types";
            var productTypes = await client.GetAsync<IEnumerable<ProductType>>(requestUri).ConfigureAwait(false);
            return productTypes;
        }

        /// <summary>
        /// Creates the product api client.
        /// </summary>
        /// <returns>A <see cref="HttpClient"/> for accessing product api.</returns>
        private HttpClient CreateClient()
        {
            return httpClientFactory.CreateClient("ProductApiClient");
        }
    }
}
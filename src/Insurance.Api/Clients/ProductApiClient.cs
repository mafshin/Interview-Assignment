using System.Net.Http;
using System.Threading.Tasks;
using Insurance.Api.Models;
using Newtonsoft.Json;

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
            string json = await client.GetStringAsync($"/product_types/{productTypeId}").ConfigureAwait(false);
            var productType = JsonConvert.DeserializeObject<ProductType>(json);
            return productType;
        }

        /// <inheritdoc />
        public async Task<Product> GetProductById(int productId)
        {
            HttpClient client = CreateClient();
            string json = await client.GetStringAsync($"/products/{productId}").ConfigureAwait(false);
            var product = JsonConvert.DeserializeObject<Product>(json);
            return product;
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
using System.Net.Http;
using System.Threading.Tasks;
using Insurance.Api.Models;
using Newtonsoft.Json;

namespace Insurance.Api.Clients
{
    public class ProductApiClient : IProductApiClient
    {
        private readonly IHttpClientFactory httpClientFactory;

        public ProductApiClient(IHttpClientFactory clientFactory)
        {
            this.httpClientFactory = clientFactory;
        }

        public async Task<ProductType> GetProductTypeById(int productTypeId)
        {
            HttpClient client = CreateClient();
            string json = await client.GetStringAsync($"/product_types/{productTypeId}").ConfigureAwait(false);
            var productType = JsonConvert.DeserializeObject<ProductType>(json);
            return productType;
        }

        public async Task<Product> GetProductById(int productID)
        {
            HttpClient client = CreateClient();
            string json = await client.GetStringAsync(string.Format("/products/{0:G}", productID)).ConfigureAwait(false);
            var product = JsonConvert.DeserializeObject<Product>(json);
            return product;
        }

        private HttpClient CreateClient()
        {
            return httpClientFactory.CreateClient("ProductApiClient");
        }
    }
}
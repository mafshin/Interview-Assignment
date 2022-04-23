using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Insurance.Api.Models;

namespace Insurance.Api
{
    public class ProductApiClient : IProductApiClient
    {
        private readonly IHttpClientFactory httpClientFactory;

        public ProductApiClient(IHttpClientFactory clientFactory)
        {
            this.httpClientFactory = clientFactory;
        }
        public async Task<ProductType> GetProductTypeByProductId(int productID)
        {
            var product = await GetProductById(productID).ConfigureAwait(false);

            int productTypeId = product.productTypeId;

            ProductType productType = await GetProductTypeById(productTypeId).ConfigureAwait(false);

            return productType;
        }

        public async Task<float> GetSalesPriceByProductId(int productID)
        {
            dynamic product = await GetProductById(productID).ConfigureAwait(false);

            return product.salesPrice;
        }

        public async Task<ProductType> GetProductTypeById(int productTypeId)
        {
            HttpClient client = httpClientFactory.CreateClient("ProductApiClient");
            string json = await client.GetStringAsync($"/product_types/{productTypeId}").ConfigureAwait(false);
            var productType = JsonConvert.DeserializeObject<ProductType>(json);
            return productType;
        }

        public async Task<dynamic> GetProductById(int productID)
        {
            HttpClient client = httpClientFactory.CreateClient("ProductApiClient");
            string json = await client.GetStringAsync(string.Format("/products/{0:G}", productID)).ConfigureAwait(false);
            var product = JsonConvert.DeserializeObject<dynamic>(json);
            return product;
        }
    }
}
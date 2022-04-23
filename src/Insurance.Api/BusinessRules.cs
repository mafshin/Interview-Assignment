using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Insurance.Api.Models;

namespace Insurance.Api
{
    public static class BusinessRules
    {
        public static async Task<ProductType> GetProductType(IHttpClientFactory httpClientFactory, int productID)
        {
            var product = await GetProduct(httpClientFactory, productID).ConfigureAwait(false);

            int productTypeId = product.productTypeId;

            ProductType productType = await GetProductTypeById(httpClientFactory, productTypeId).ConfigureAwait(false);

            return productType;
        }

        public static async Task<float> GetSalesPrice(IHttpClientFactory httpClientFactory, int productID)
        {
            dynamic product = await GetProduct(httpClientFactory, productID).ConfigureAwait(false);

            return product.salesPrice;
        }

        private static async Task<ProductType> GetProductTypeById(IHttpClientFactory httpClientFactory, int productTypeId)
        {
            HttpClient client = httpClientFactory.CreateClient("ProductApiClient");
            string json = await client.GetStringAsync($"/product_types/{productTypeId}").ConfigureAwait(false);
            var productType = JsonConvert.DeserializeObject<ProductType>(json);
            return productType;
        }

        private static async Task<dynamic> GetProduct(IHttpClientFactory httpClientFactory, int productID)
        {
            HttpClient client = httpClientFactory.CreateClient("ProductApiClient");
            string json = await client.GetStringAsync(string.Format("/products/{0:G}", productID)).ConfigureAwait(false);
            var product = JsonConvert.DeserializeObject<dynamic>(json);
            return product;
        }
    }
}
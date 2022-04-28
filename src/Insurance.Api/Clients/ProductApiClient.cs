using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Insurance.Api.Cache;
using Insurance.Api.Extensions;
using Insurance.Api.Models;

namespace Insurance.Api.Clients
{
    public class ProductApiClient : IProductApiClient
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ICache cache;

        /// <summary>
        /// Initializes a new instance of <see cref="ProductApiClient"/>.
        /// </summary>
        /// <param name="httpClientFactory">Http Client Factory for creating <see cref="HttpClient"/>
        /// instances.</param>
        /// <param name="cache">The optional cache object for caching
        /// api responses.</param>
        public ProductApiClient(IHttpClientFactory httpClientFactory, ICache cache = null)
        {
            this.httpClientFactory = httpClientFactory;
            this.cache = cache;
        }

        /// <inheritdoc />
        public async Task<ProductType> GetProductTypeById(int productTypeId)
        {
            var requestUri = $"/product_types/{productTypeId}";

            return await Get<ProductType>(requestUri);
        }

        /// <inheritdoc />
        public async Task<Product> GetProductById(int productId)
        {
            var requestUri = $"/products/{productId}";

            return await Get<Product>(requestUri);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<ProductType>> GetProductTypes()
        {
            var requestUri = $"/product_types";

            return await Get<IEnumerable<ProductType>>(requestUri);
        }

        /// <summary>
        /// Creates the product api client.
        /// </summary>
        /// <returns>A <see cref="HttpClient"/> for accessing product api.</returns>
        private HttpClient CreateClient()
        {
            return httpClientFactory.CreateClient("ProductApiClient");
        }

        private Task<T> Get<T>(string requestUri) where T : class
        {
            var factory = new Func<Task<T>>(async () =>
            {
                HttpClient client = CreateClient();
                var productType = await client.GetAsync<T>(requestUri).ConfigureAwait(false);
                return productType;
            });

            return GetOrSetCacheEntry(requestUri, factory);
        }

        private Task<T> GetOrSetCacheEntry<T>(string requestUri, Func<Task<T>> factory)
        {
            return cache is null ? factory() : cache.GetOrSetEntry(requestUri, factory);
        }
    }
}
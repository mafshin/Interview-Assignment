using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Insurance.Api.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="HttpClient"/>.
    /// </summary>
    public static class HttpClientExtensions
    {
        /// <summary>
        /// Sends a GET request to provided Uri and deserializes
        /// the response json to <typeparam name="T">Type</typeparam>
        /// </summary>
        /// <param name="httpClient">HttpClient used for sending request.</param>
        /// <param name="requestUri">The Uri of the request.</param>
        /// <typeparam name="T">Type of object to deserialize the response
        /// json into.</typeparam>
        /// <returns>A <see cref="Task{T}"/> containing the deserialized
        /// object from response.</returns>
        public static async Task<T> GetAsync<T>(this HttpClient httpClient, string requestUri)
        {
            string json = await httpClient.GetStringAsync(requestUri).ConfigureAwait(false);
            var model = JsonConvert.DeserializeObject<T>(json);
            return model;
        }
    }
}
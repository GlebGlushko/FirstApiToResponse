using System;
using System.Net.Http;
using System.Threading.Tasks;
using Weather.DataTransferObjects;

namespace Weather.Services
{
    public static class PerformQueryService
    {
        public static async Task<HttpResponseMessage> PerformQueryAsync(HttpClient client, string route, string query)
        {
            var response = await client.GetAsync(route + '?' + query);
            if (!response.IsSuccessStatusCode)
            {
                string errorMessage = await response.Content.ReadAsStringAsync();
                throw new Exception(errorMessage);
            }
            return response;
        }
    }
}
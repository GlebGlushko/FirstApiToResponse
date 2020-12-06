using Weather.Services.Interfaces;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading;
using Weather.DataTransferObjects;
using Newtonsoft.Json;

namespace Weather.Services
{
    public class WeatherbitService : IWeatherService
    {
        private readonly HttpClient _client;
        private readonly WeatherbitApiOptions _options;
        private readonly PerformQueryService _performQueryService;

        public WeatherbitService(IOptions<WeatherbitApiOptions> options, PerformQueryService performQueryService)
        {
            _options = options.Value;
            _performQueryService = performQueryService;
            _client = new HttpClient
            {
                BaseAddress = new Uri(_options.URL)
            };
        }
        public async Task<CommonWeatherDto> PerformQueryAsync(string query)
        {
            var response = await _performQueryService.PerformQueryAsync(_client, _options.WeatherRouter, query);
            return new CommonWeatherDto(await response.Content.ReadAsAsync<WeatherbitDto>());
        }
        public async Task<CommonWeatherDto> GetWeatherAsync(double lat, double lng) =>
            await PerformQueryAsync(string.Join('&', new[]{
                "key=" + _options.API_KEY,
                "lat=" + lat,
                "lon=" + lng
            }));

        public async Task<CommonWeatherDto> GetWeatherAsync(string address) =>
              await PerformQueryAsync(string.Join('&',
              new[]{
                "key=" + _options.API_KEY,
                "city=" + address
            }));
    }
    public class WeatherbitApiOptions
    {
        public string API_KEY { get; set; }
        public string URL { get; set; }
        public string WeatherRouter { get; set; }

    }
}
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
    public class OpenWeatherMapService : IWeatherService
    {
        private readonly HttpClient _client;
        private readonly OpenWeatherMapOptions _options;
        private readonly PerformQueryService _performQueryService;
        public OpenWeatherMapService(IOptions<OpenWeatherMapOptions> options, PerformQueryService performQueryService)
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

            return new CommonWeatherDto(await response.Content.ReadAsAsync<OpenWeatherMapDto>());
        }
        public async Task<CommonWeatherDto> GetWeatherAsync(double lat, double lng) =>
            await PerformQueryAsync(string.Join('&', new[]
            {
                "appid=" + _options.API_KEY,
                "lat=" + lat,
                "lon=" + lng,
                "units=metric"
            }));

        public async Task<CommonWeatherDto> GetWeatherAsync(string address) =>
            await PerformQueryAsync(string.Join('&', new[]
            {
                "appid=" + _options.API_KEY,
                "q=" + address,
                "units=metric"
            }));

    }
    public class OpenWeatherMapOptions
    {
        public string API_KEY { get; set; }
        public string URL { get; set; }
        public string WeatherRouter { get; set; }

    }

}
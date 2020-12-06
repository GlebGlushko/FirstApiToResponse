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
    public class AccuWeatherService : IWeatherService
    {
        private readonly HttpClient _client;
        private readonly AccuWeatherOptions _options;
        private readonly PerformQueryService _performQueryService;

        public AccuWeatherService(IOptions<AccuWeatherOptions> options, PerformQueryService performQueryService)
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
            return new CommonWeatherDto((await response.Content.ReadAsAsync<AccuWeatherDto[]>())[0]);
        }

        public async Task<CommonWeatherDto> GetWeatherAsync(double lat, double lng)
        {
            var locationId = await (await _client.GetAsync(_options.LocationsRouter + FormQuery(lat, lng))).Content.ReadAsAsync<LocationKey>();
            return await PerformQueryAsync($"/{locationId.Key}?apikey={_options.API_KEY}&details=true");
        }
        public async Task<CommonWeatherDto> GetWeatherAsync(string address)
        {
            var locationId = (await (await _client.GetAsync(_options.LocationsRouter + FormQuery(address))).Content.ReadAsAsync<LocationKey[]>())[0];
            return await PerformQueryAsync($"/{locationId.Key}?apikey={_options.API_KEY}&details=true");
        }
        private string FormQuery(double lat, double lng)
        {
            return "/geoposition/search?" + string.Join('&', new[]{
                    "apikey=" + _options.API_KEY,
                    "q=" + $"{lat},{lng}"
                }
            );
        }
        private string FormQuery(string address)
        {
            return "/search?" + string.Join('&', new[]{
                    "apikey=" + _options.API_KEY,
                    "q=" + address,
                }
            );
        }

    }
    public class AccuWeatherOptions
    {
        public string API_KEY { get; set; }
        public string URL { get; set; }
        public string WeatherRouter { get; set; }
        public string LocationsRouter { get; set; }

    }
    public class LocationKey
    {
        public string Key { get; set; }
    }
}
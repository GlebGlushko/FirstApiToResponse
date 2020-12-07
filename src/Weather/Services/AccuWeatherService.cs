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

        public AccuWeatherService(IOptions<AccuWeatherOptions> options, IHttpClientFactory clientFactory)
        {
            _options = options.Value;
            _client = clientFactory.CreateClient("AccuWeather");

        }
        public async Task<CommonWeatherDto> FetchAsync(LocationKey locationId)
        {
            var query = $"apikey={_options.API_KEY}&details=true";
            var response = await PerformQueryService.PerformQueryAsync(_client, _options.WeatherRouter + $"/{locationId.Key}", query);
            return new CommonWeatherDto((await response.Content.ReadAsAsync<AccuWeatherDto[]>())[0]);
        }

        public async Task<CommonWeatherDto> GetWeatherAsync(double lat, double lng)
        {
            var locationId = (await (await PerformQueryService.PerformQueryAsync(_client, _options.LocationsRouter + "/geoposition/search", FormQuery(lat, lng))).Content.ReadAsAsync<LocationKey>());
            return await FetchAsync(locationId);
        }
        public async Task<CommonWeatherDto> GetWeatherAsync(string address)
        {
            var locationId = (await (await PerformQueryService.PerformQueryAsync(_client, _options.LocationsRouter + "/search", FormQuery(address))).Content.ReadAsAsync<LocationKey[]>())[0];
            return await FetchAsync(locationId);
        }
        private string FormQuery(double lat, double lng)
        {
            return string.Join('&', new[]{
                    "apikey=" + _options.API_KEY,
                    "q=" + $"{lat},{lng}"
                }
            );
        }
        private string FormQuery(string address)
        {
            return string.Join('&', new[]{
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
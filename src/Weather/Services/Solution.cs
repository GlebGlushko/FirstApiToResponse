using System.Threading.Tasks;
using Weather.DataTransferObjects;

namespace Weather.Services
{
    public class Solution
    {
        private readonly WeatherAggregator _aggregator; //weather aggregator is a set of weather services we use
        public Solution(WeatherAggregator aggregator)
        {
            _aggregator = aggregator;
            _aggregator.AddWeatherBitService();  //here we easily add or remove weather services
            _aggregator.AddOpenWeatherMap();     //and using a builder pattern, we don't need to create 
            _aggregator.AddAccuWeather();        //various constructors with huge number of parameters
        }
        public async Task<CommonWeatherDto> PerformAsync(double lat, double lng) => await _aggregator.GetFirstResponseAsync(lat, lng);
        public async Task<CommonWeatherDto> PerformAsync(string address) => await _aggregator.GetFirstResponseAsync(address);

    }
}
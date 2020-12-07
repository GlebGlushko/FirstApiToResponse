using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Weather.DataTransferObjects;
using Weather.Services.Interfaces;
namespace Weather.Controllers
{
    [Route("weather")]
    [ApiController]
    public class WeatherController : ControllerBase
    {
        private readonly IWeatherAggregator _aggregator;
        public WeatherController(IWeatherAggregator aggregator)
        {
            _aggregator = aggregator;

        }
        //Returns weather info by geoposition
        [HttpGet("coords")]
        public async Task<ActionResult<CommonWeatherDto>> Get(double lat, double lng)
        {
            if (lat < -90 || lat > 90 || lng < -180 || lng > 180) return BadRequest("Coordinates are not in range");
            return await _aggregator.GetFirstResponseAsync(lat, lng);
        }
        //Returns weather info by city name
        [HttpGet("city")]
        public async Task<ActionResult<CommonWeatherDto>> Get(string address)
        {
            if (string.IsNullOrWhiteSpace(address)) return BadRequest("Address is empty or white spaces");
            try
            {
                return await _aggregator.GetFirstResponseAsync(address);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
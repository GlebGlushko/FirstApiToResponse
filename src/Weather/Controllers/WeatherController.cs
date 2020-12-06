using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Weather.DataTransferObjects;
using Weather.Services;
using Weather.Services.Interfaces;
namespace Weather.Controllers
{
    [Route("weather")]
    [ApiController]
    public class WeatherController : ControllerBase
    {
        private readonly Solution _solution;
        public WeatherController(Solution solution)
        {
            _solution = solution;
        }
        //Returns weather info by geoposition
        [HttpGet("coords")]
        public async Task<CommonWeatherDto> Get(double lat, double lng)
        {
            return await _solution.PerformAsync(lat, lng);
        }
        //Returns weather info by city name
        [HttpGet("city")]
        public async Task<CommonWeatherDto> Get(string address)
        {
            return await _solution.PerformAsync(address);
        }

    }
}

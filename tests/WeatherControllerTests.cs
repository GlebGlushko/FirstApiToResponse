using System;
using Xunit;
using Weather.Services;
using Microsoft.Extensions.Options;
using Weather.Controllers;
using Moq;
using System.Threading.Tasks;
using Weather.DataTransferObjects;
using Weather.Services.Interfaces;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace tests
{
    public class WeatherControllerTests
    {

        [Theory()]
        [InlineData(0, 0)]
        [InlineData(90, 180)]
        [InlineData(-90, 45)]
        public async void GetWeatherInfo_ReturnsOk_CorrectCoordinates(double lat, double lng)
        {
            var expected = new CommonWeatherDto { Description = "Test description" };

            var mock = new Mock<IWeatherAggregator>();
            mock.Setup(x => x.GetFirstResponseAsync(lat, lng)).Returns(Task.FromResult(expected));
            WeatherController controller = new WeatherController(mock.Object);

            var result = await controller.Get(lat, lng);
            Assert.Equal(result.Value, expected);
        }
        [Theory()]
        [InlineData(-91, 10)]
        [InlineData(93, 90)]
        [InlineData(45, 192)]
        [InlineData(-100, -200)]
        [InlineData(100, 500)]
        public async void GetWeatherInfo_ReturnBadRequest_WrongCoordinates(double lat, double lng)
        {
            var mock = new Mock<IWeatherAggregator>();
            WeatherController controller = new WeatherController(mock.Object);

            var response = await controller.Get(lat, lng);
            Assert.IsType<BadRequestObjectResult>(response.Result);
        }

        [Theory()]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("      ")]
        public async void GetWeatherInfo_ReturnBadRequest_EmptyStringOrNullOrSpaces(string query)
        {
            var mock = new Mock<IWeatherAggregator>();
            WeatherController controller = new WeatherController(mock.Object);

            var response = await controller.Get(query);
            Assert.IsType<BadRequestObjectResult>(response.Result);
        }
    }
}

using System;
using Xunit;
using Weather.Services;
using Microsoft.Extensions.Options;
using Weather.Controllers;
using Moq;
using Moq.Protected;
using System.Threading.Tasks;
using Weather.DataTransferObjects;
using Weather.Services.Interfaces;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net;
using System.Threading;
using Newtonsoft.Json;
using System.Text;

namespace tests
{
    public class WeatherbitTests
    {
        [Fact]
        public async Task FetchAsync_ReturnsCommonWeatherResult()
        {
            var weatherbitService = CreateMockWeatherbitService();

            var result = await weatherbitService.FetchAsync("query");

            Assert.IsType<CommonWeatherDto>(result);

        }
        [Theory]
        [InlineData(10, 20)]
        [InlineData(-10, 90)]
        public async Task GetWeatherAsyncWithCoordinates_ReturnsCommonWeatherResult(double lat, double lng)
        {
            var weatherbitService = CreateMockWeatherbitService();

            var result = await weatherbitService.GetWeatherAsync(lat, lng);

            Assert.IsType<CommonWeatherDto>(result);

        }
        [InlineData("Kyiv")]
        [InlineData("London")]
        public async Task GetWeatherAsyncWithAddress_ReturnsCommonWeatherResult(string address)
        {
            var weatherbitService = CreateMockWeatherbitService();

            var result = await weatherbitService.GetWeatherAsync(address);

            Assert.IsType<CommonWeatherDto>(result);

        }

        private WeatherbitService CreateMockWeatherbitService()
        {
            var fakeOptions = new WeatherbitApiOptions();
            var fakeDto = new WeatherbitDto();
            var mockOptions = new Mock<IOptions<WeatherbitApiOptions>>();
            mockOptions.Setup(x => x.Value).Returns(fakeOptions);

            var mockHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            mockHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .Returns(Task.FromResult(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,

                    Content = new StringContent(JsonConvert.SerializeObject(fakeDto), Encoding.UTF8, "application/json"),

                }));

            var httpClient = new HttpClient(mockHandler.Object)
            {
                BaseAddress = new Uri("http://any.url")
            };

            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            mockHttpClientFactory.Setup(x => x.CreateClient("Weatherbit")).Returns(httpClient);

            return new WeatherbitService(mockOptions.Object, mockHttpClientFactory.Object);
        }

    }
}

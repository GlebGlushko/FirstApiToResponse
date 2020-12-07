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
    public class OpenWeatherMapTests
    {
        [Fact]
        public async Task FetchAsync_ReturnsCommonWeatherResult()
        {
            var openWeatherService = CreateMockOpenWeatherService();

            var result = await openWeatherService.FetchAsync("query");

            Assert.IsType<CommonWeatherDto>(result);

        }
        [Theory]
        [InlineData(10, 20)]
        [InlineData(-10, 90)]
        public async Task GetWeatherAsyncWithCoordinates_ReturnsCommonWeatherResult(double lat, double lng)
        {
            var openWeatherService = CreateMockOpenWeatherService();

            var result = await openWeatherService.GetWeatherAsync(lat, lng);

            Assert.IsType<CommonWeatherDto>(result);

        }
        [Theory]
        [InlineData("Kyiv")]
        [InlineData("London")]
        public async Task GetWeatherAsyncWithAddress_ReturnsCommonWeatherResult(string address)
        {
            var openWeatherService = CreateMockOpenWeatherService();

            var result = await openWeatherService.GetWeatherAsync(address);

            Assert.IsType<CommonWeatherDto>(result);

        }

        private OpenWeatherMapService CreateMockOpenWeatherService()
        {
            var fakeOptions = new OpenWeatherMapOptions();
            var fakeDto = new OpenWeatherMapDto();

            var mockOptions = new Mock<IOptions<OpenWeatherMapOptions>>();
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
            mockHttpClientFactory.Setup(x => x.CreateClient("OpenWeatherMap")).Returns(httpClient);

            return new OpenWeatherMapService(mockOptions.Object, mockHttpClientFactory.Object);
        }

    }
}

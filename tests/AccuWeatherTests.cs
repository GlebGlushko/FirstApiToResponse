using System;
using Xunit;
using Weather.Services;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using System.Threading.Tasks;
using Weather.DataTransferObjects;
using System.Net.Http;
using System.Net;
using System.Threading;
using Newtonsoft.Json;
using System.Text;

namespace tests
{
    public class AccuWeatherTests
    {
        [Fact]
        public async Task FetchAsync_ReturnsCommonWeatherResult()
        {
            var accuWeatherService = CreateMockAccuWeatherService();

            var result = await accuWeatherService.FetchAsync(new LocationKey() { Key = "locationId" });

            Assert.IsType<CommonWeatherDto>(result);

        }
        private AccuWeatherService CreateMockAccuWeatherService()
        {
            var fakeOptions = new AccuWeatherOptions();
            var fakeDto = new AccuWeatherDto();
            var mockOptions = new Mock<IOptions<AccuWeatherOptions>>();
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

                    Content = new StringContent(JsonConvert.SerializeObject(new[] { fakeDto }), Encoding.UTF8, "application/json"),

                }));

            var httpClient = new HttpClient(mockHandler.Object)
            {
                BaseAddress = new Uri("http://any.url")
            };

            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            mockHttpClientFactory.Setup(x => x.CreateClient("AccuWeather")).Returns(httpClient);

            return new AccuWeatherService(mockOptions.Object, mockHttpClientFactory.Object);
        }

    }
}

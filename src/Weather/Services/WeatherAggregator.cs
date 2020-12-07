using System;
using System.Collections.Generic;
using Weather.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using System.Threading;
using Weather.DataTransferObjects;

namespace Weather.Services
{
    public class WeatherAggregator : IWeatherAggregator
    {
        private readonly List<IWeatherService> _weatherServices;
        public WeatherAggregator(List<IWeatherService> services)
        {
            _weatherServices = services;
        }
        public async Task<CommonWeatherDto> GetFirstResponseAsync(double lat, double lng) => await RequestAllApis(api => api.GetWeatherAsync(lat, lng));
        public async Task<CommonWeatherDto> GetFirstResponseAsync(string address) => await RequestAllApis(api => api.GetWeatherAsync(address));
        private async Task<CommonWeatherDto> RequestAllApis(Func<IWeatherService, Task<CommonWeatherDto>> func)
        {
            var firstResponse = new TaskCompletionSource<CommonWeatherDto>();
            int remainingTasks = _weatherServices.Count;
            foreach (var api in _weatherServices)
            {
                //var _ used for ignoring warning, because we're not awaiting all task from continueWith
                var _ = func(api).ContinueWith(t =>
                {
                    if (t.Status == TaskStatus.RanToCompletion)
                        firstResponse.TrySetResult(t.Result);
                    else
                        if (Interlocked.Decrement(ref remainingTasks) == 0)
                        firstResponse.SetException(new Exception("No weather service found the info about your data"));
                });
            }
            return await firstResponse.Task;
        }


    }
}
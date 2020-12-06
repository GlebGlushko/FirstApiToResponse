using System;
using System.Collections.Generic;
using Weather.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using System.Threading;
using Weather.DataTransferObjects;

namespace Weather.Services
{
    public class WeatherAggregator
    {
        private List<IWeatherService> _weatherServices;
        private readonly IServiceProvider _serviceProvider;
        public WeatherAggregator(IServiceProvider provider) //i could get all services into constructor, but decided to use builder patter, because number of services could be huge
        {
            _weatherServices = new List<IWeatherService>();
            _serviceProvider = provider;
        }
        public void AddWeatherBitService() =>
            _weatherServices.Add(_serviceProvider.GetRequiredService<WeatherbitService>());
        public void AddOpenWeatherMap() =>
            _weatherServices.Add(_serviceProvider.GetRequiredService<OpenWeatherMapService>());

        public void AddAccuWeather() =>
            _weatherServices.Add(_serviceProvider.GetRequiredService<AccuWeatherService>());

        public async Task<CommonWeatherDto> GetFirstResponseAsync(double lat, double lng)
        {
            var firstResponse = new TaskCompletionSource<CommonWeatherDto>();  //create a variable that will save first answer or error
            int remainingTasks = _weatherServices.Count;
            foreach (var api in _weatherServices)                               //make a request to each of our weather services
            {
                api.GetWeatherAsync(lat, lng).ContinueWith(t =>                 //it's ok we don't await our calls, because we don't want all results, we need only one
                {                                                               //and we will await Task that waits for our result
                    if (t.Status == TaskStatus.RanToCompletion)
                        firstResponse.TrySetResult(t.Result);                  //first one that ended successefully will store the answer
                    else
                        if (Interlocked.Decrement(ref remainingTasks) == 0)
                        firstResponse.SetException(new Exception("No weather service responsed to your request:'(  "));
                });
            }
            try
            {
                return await firstResponse.Task;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<CommonWeatherDto> GetFirstResponseAsync(string address)
        {
            var firstResponse = new TaskCompletionSource<CommonWeatherDto>();
            int remainingTasks = _weatherServices.Count;
            foreach (var api in _weatherServices)
            {
                api.GetWeatherAsync(address).ContinueWith(t =>
                {
                    if (t.Status == TaskStatus.RanToCompletion)
                        firstResponse.TrySetResult(t.Result);
                    else
                        if (Interlocked.Decrement(ref remainingTasks) == 0)
                        firstResponse.SetException(new Exception("No weather service responsed to your request:'(  "));
                });
            }

            return await firstResponse.Task;
        }
    }
}
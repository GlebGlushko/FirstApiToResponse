using System.Threading.Tasks;
using Weather.DataTransferObjects;

namespace Weather.Services.Interfaces
{
    public interface IWeatherAggregator
    {
        Task<CommonWeatherDto> GetFirstResponseAsync(double lat, double lng);
        Task<CommonWeatherDto> GetFirstResponseAsync(string address);
    }
}
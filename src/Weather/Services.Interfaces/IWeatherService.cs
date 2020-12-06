using System.Threading.Tasks;
using Weather.DataTransferObjects;
namespace Weather.Services.Interfaces
{
    public interface IWeatherService
    {
        Task<CommonWeatherDto> GetWeatherAsync(double lat, double lng);
        Task<CommonWeatherDto> GetWeatherAsync(string address);
    }
}
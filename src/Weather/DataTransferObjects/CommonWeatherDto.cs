using System;
namespace Weather.DataTransferObjects
{
    public class CommonWeatherDto
    {
        public string Source { get; set; }
        public DateTime TimeStamp = DateTime.Now;
        public double Temperature { get; set; }
        public double Pressure { get; set; }
        public double Humidity { get; set; }
        public string Description { get; set; }
        public WindInfo Wind { get; set; }
        public class WindInfo
        {
            public double Direction { get; set; }
            public double Speed { get; set; }
        }

        public CommonWeatherDto(WeatherbitDto input)
        {
            var data = input.Data[0];
            Source = "Weatherbit.io";
            Temperature = data.Temp;
            Pressure = data.Pres;
            Humidity = data.Rh;
            Description = data.Weather.Description;
            Wind = new WindInfo
            {
                Direction = data.Wind_dir,
                Speed = data.Wind_spd
            };

        }
        public CommonWeatherDto(OpenWeatherMapDto input)
        {
            Source = "OpenWeatherMap.org";
            Temperature = input.Main.Temp;
            Pressure = input.Main.Pressure;
            Humidity = input.Main.Humidity;
            Description = input.Weather[0].Description;
            Wind = new WindInfo
            {
                Direction = input.Wind.Deg,
                Speed = input.Wind.Speed
            };
        }
        public CommonWeatherDto(AccuWeatherDto input)
        {
            Source = "Accuweather.com";
            Temperature = input.Temperature.Metric.Value;
            Pressure = input.Pressure.Metric.Value;
            Humidity = input.RelativeHumidity;
            Description = input.WeatherText;
            Wind = new WindInfo
            {
                Direction = input.Wind.Direction.Degrees,
                Speed = input.Wind.Speed.Metric.Value * 1000.0 / 60.0 / 60.0
            };

        }
    }
}
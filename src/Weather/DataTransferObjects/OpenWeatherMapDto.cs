namespace Weather.DataTransferObjects
{
    public class OpenWeatherMapDto
    {
        public MainInfo Main { get; set; }
        public WindInfo Wind { get; set; }
        public WeatherInfo[] Weather { get; set; }
        public class MainInfo
        {
            public double Temp { get; set; }
            public double Pressure { get; set; }
            public double Humidity { get; set; }
        }
        public class WindInfo
        {
            public double Speed { get; set; }
            public double Deg { get; set; }
        }
        public class WeatherInfo
        {
            public int Id { get; set; }
            public string Description { get; set; }
        }
    }
}
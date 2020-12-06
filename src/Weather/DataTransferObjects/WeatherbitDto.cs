namespace Weather.DataTransferObjects
{
    public class WeatherbitDto
    {
        public WeatherbitWeatherInfo[] Data { get; set; }
        public class WeatherbitWeatherInfo
        {
            public double Temp { get; set; }
            public double Pres { get; set; }
            public string Sunrise { get; set; }
            public double Wind_spd { get; set; }
            public double Wind_dir { get; set; }
            public string Wind_cdir_full { get; set; }
            public double Clouds { get; set; }
            public double Rh { get; set; }
            public WeatherInfo Weather { get; set; }
            public class WeatherInfo
            {
                public int Code { get; set; }
                public string Description { get; set; }
            }
        }
    }
}
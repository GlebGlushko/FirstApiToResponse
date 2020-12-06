namespace Weather.DataTransferObjects
{
    public class AccuWeatherDto
    {
        public string WeatherText { get; set; }
        public TemperatureInfo Temperature { get; set; }
        public double RelativeHumidity { get; set; }
        public WindInfo Wind { get; set; }
        public PressureInfo Pressure { get; set; }
        public class TemperatureInfo
        {
            public MetricInfo Metric { get; set; }
            public class MetricInfo
            {
                public double Value { get; set; }
            }
        }
        public class WindInfo
        {
            public DirectionInfo Direction { get; set; }
            public SpeedInfo Speed { get; set; }
            public class DirectionInfo
            {
                public double Degrees { get; set; }
                public string English { get; set; }
            }
            public class SpeedInfo
            {
                public MetricInfo Metric { get; set; }
                public class MetricInfo
                {
                    public double Value { get; set; }
                }
            }

        }
        public class PressureInfo
        {
            public MetricInfo Metric { get; set; }
            public class MetricInfo
            {
                public double Value { get; set; }
            }
        }
    }
}
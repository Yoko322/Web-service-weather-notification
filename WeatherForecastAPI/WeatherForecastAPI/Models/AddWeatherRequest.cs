namespace WeatherForecastAPI.Models
{
    public class AddWeatherRequest
    {
        public string City { get; set; } = string.Empty;
        public double TemperatureC { get; set; }
        public string Summary { get; set; } = string.Empty;
        public double Humidity { get; set; }
        public double WindSpeed { get; set; }
        public string Condition { get; set; } = string.Empty;
    }
}
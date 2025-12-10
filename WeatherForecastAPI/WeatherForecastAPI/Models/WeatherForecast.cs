namespace WeatherForecastAPI.Models
{
    public class WeatherForecast
    {
        public int Id { get; set; }
        public string City { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public double TemperatureC { get; set; }
        public double TemperatureF => 32 + (TemperatureC / 0.5556);
        public string Summary { get; set; } = string.Empty;
        public double Humidity { get; set; }
        public double WindSpeed { get; set; }
        public string Condition { get; set; } = string.Empty;
    }
}
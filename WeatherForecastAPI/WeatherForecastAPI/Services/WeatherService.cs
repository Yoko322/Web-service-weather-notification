using WeatherForecastAPI.Models;

namespace WeatherForecastAPI.Services
{
    public class WeatherService : IWeatherService
    {
        private static List<WeatherForecast> _forecasts = new()
        {
            new WeatherForecast
            {
                Id = 1,
                City = "Moscow",
                Date = DateTime.Now,
                TemperatureC = 20,
                Summary = "Sunny",
                Humidity = 65,
                WindSpeed = 5.2,
                Condition = "Clear"
            },
            new WeatherForecast
            {
                Id = 2,
                City = "Saint Petersburg",
                Date = DateTime.Now,
                TemperatureC = 18,
                Summary = "Cloudy",
                Humidity = 70,
                WindSpeed = 3.8,
                Condition = "Partly Cloudy"
            }
        };

        public IEnumerable<WeatherForecast> GetAllForecasts()
        {
            return _forecasts;
        }

        public WeatherForecast? GetForecastByCity(string city)
        {
            return _forecasts.FirstOrDefault(f =>
                f.City.Equals(city, StringComparison.OrdinalIgnoreCase));
        }

        public WeatherForecast AddForecast(AddWeatherRequest request)
        {
            var forecast = new WeatherForecast
            {
                Id = _forecasts.Count + 1,
                City = request.City,
                Date = DateTime.Now,
                TemperatureC = request.TemperatureC,
                Summary = request.Summary,
                Humidity = request.Humidity,
                WindSpeed = request.WindSpeed,
                Condition = request.Condition
            };

            _forecasts.Add(forecast);
            return forecast;
        }

        public WeatherForecast? UpdateForecast(string city, AddWeatherRequest request)
        {
            var existingForecast = _forecasts.FirstOrDefault(f =>
                f.City.Equals(city, StringComparison.OrdinalIgnoreCase));

            if (existingForecast != null)
            {
                existingForecast.City = request.City;
                existingForecast.TemperatureC = request.TemperatureC;
                existingForecast.Summary = request.Summary;
                existingForecast.Humidity = request.Humidity;
                existingForecast.WindSpeed = request.WindSpeed;
                existingForecast.Condition = request.Condition;
                existingForecast.Date = DateTime.Now;
            }

            return existingForecast;
        }

        public bool DeleteForecast(string city)
        {
            var forecast = _forecasts.FirstOrDefault(f =>
                f.City.Equals(city, StringComparison.OrdinalIgnoreCase));

            if (forecast != null)
            {
                return _forecasts.Remove(forecast);
            }

            return false;
        }
    }
}
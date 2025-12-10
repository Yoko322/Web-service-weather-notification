using WeatherForecastAPI.Models;

namespace WeatherForecastAPI.Services
{
    public interface IWeatherService
    {
        IEnumerable<WeatherForecast> GetAllForecasts();
        WeatherForecast? GetForecastByCity(string city);
        WeatherForecast AddForecast(AddWeatherRequest request);
        WeatherForecast? UpdateForecast(string city, AddWeatherRequest request);
        bool DeleteForecast(string city);
    }
}
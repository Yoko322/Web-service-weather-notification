using Microsoft.AspNetCore.Mvc;
using WeatherForecastAPI.Models;
using WeatherForecastAPI.Services;

namespace WeatherForecastAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly IWeatherService _weatherService;

        public WeatherController(IWeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<WeatherForecast>> GetAllForecasts()
        {
            var forecasts = _weatherService.GetAllForecasts();
            return Ok(forecasts);
        }

        [HttpGet("{city}")]
        public ActionResult<WeatherForecast> GetForecastByCity(string city)
        {
            var forecast = _weatherService.GetForecastByCity(city);

            if (forecast == null)
            {
                return NotFound($"Forecast for city '{city}' not found");
            }

            return Ok(forecast);
        }

        [HttpPost]
        public ActionResult<WeatherForecast> AddForecast([FromBody] AddWeatherRequest request)
        {
            if (string.IsNullOrEmpty(request.City))
            {
                return BadRequest("City name is required");
            }

            var forecast = _weatherService.AddForecast(request);
            return CreatedAtAction(nameof(GetForecastByCity), new { city = forecast.City }, forecast);
        }

        [HttpPut("{city}")]
        public ActionResult<WeatherForecast> UpdateForecast(string city, [FromBody] AddWeatherRequest request)
        {
            var updatedForecast = _weatherService.UpdateForecast(city, request);

            if (updatedForecast == null)
            {
                return NotFound($"Forecast for city '{city}' not found");
            }

            return Ok(updatedForecast);
        }

        [HttpDelete("{city}")]
        public IActionResult DeleteForecast(string city)
        {
            var result = _weatherService.DeleteForecast(city);

            if (!result)
            {
                return NotFound($"Forecast for city '{city}' not found");
            }

            return NoContent();
        }
    }
}
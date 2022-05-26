using System.Collections.Generic;

namespace RestaurantAPI.Controllers
{
    public interface IWeatherForecastService
    {
        IEnumerable<WeatherForecast> Get();
    }
}
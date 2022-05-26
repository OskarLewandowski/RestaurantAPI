using System.Collections.Generic;

namespace RestaurantAPI.Controllers
{
    public interface IWeatherForecastService
    {
        IEnumerable<WeatherForecast> Get(int take, int minTemperature, int maxTemperature);
    }
}
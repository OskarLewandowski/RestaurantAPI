using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IWeatherForecastService _service;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IWeatherForecastService service)
        {
            _logger = logger;
            _service = service;
        }

        //[HttpGet]
        //public IEnumerable<WeatherForecast> Get()
        //{
        //    var result = _service.Get();

        //    return result;
        //}

        //[HttpGet("currentDay/{max}")]
        ////[Route("currentDay")]
        //public IEnumerable<WeatherForecast> Get2([FromQuery] int take, [FromRoute] int max)
        //{
        //    var result = _service.Get();

        //    return result;
        //}

        [HttpPost]
        public ActionResult<string> Hello([FromBody] string name)
        {
            //HttpContext.Response.StatusCode = 401;
            //return StatusCode(401, $"Hello {name}");

            return NotFound($"Hello {name}");
        }

        [HttpPost("generate")]
        public ActionResult<IEnumerable<WeatherForecast>> Generate([FromQuery] int take,
            [FromBody] TemperatureRequestModel request)
        {
            if (take < 0 || request.Max < request.Min)
            {
                return BadRequest();
            }

            var result = _service.Get(take, request.Min, request.Max);

            return Ok(result);
        }

        //[HttpPost]
        //public string Hello([FromBody] string name)
        //{
        //    return $"Hello {name}";
        //}
    }
}

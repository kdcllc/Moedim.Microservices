using Microsoft.AspNetCore.Mvc;

using Moedim.AspNetCore.Demo.Models.v1;

#pragma warning disable SA1300 // Element should begin with upper-case letter
namespace Moedim.AspNetCore.Demo.Controllers.v1
#pragma warning restore SA1300 // Element should begin with upper-case letter
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("[controller]")]
    [Route("v{version:apiVersion}/[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        // GET /WeatherForecast/ByRange?range=7/24/2022,07/26/2022
        [HttpGet("ByRange")]
        public ActionResult<IEnumerable<WeatherForecast>?> ByRange([FromQuery] DateRange range)
        {
            var weatherForecasts =
                Enumerable
                .Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                });

            weatherForecasts = weatherForecasts.Where(wf => wf.Date >= range.From
                            && wf.Date <= range.To);

            return Ok(weatherForecasts);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using QICore.ElasticSearchCore.WebApi.Dao;
using QICore.ElasticSearchCore.WebApi.OptionModel;

namespace QICore.ElasticSearchCore.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        public ITest _test;
        private readonly IOptions<ConnectionStringOption> _connStrings;
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ITest test, IOptions<ConnectionStringOption>  conn,ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
            _test = test;
            _connStrings = conn;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
        [HttpGet("test/{name}")]
        public OkObjectResult Test(string name)
        {
            var str = _test.GetTest(name);
            return Ok(new { Name=str, MySqlConnectionStrings = _connStrings .Value.MySqlConnectionStrings});
        }
    }
}

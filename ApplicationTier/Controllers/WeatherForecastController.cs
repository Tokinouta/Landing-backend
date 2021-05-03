using ApplicationTier.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationTier.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
        private readonly IHubContext<SimulationHub> _hub;
        private readonly ILogger<WeatherForecastController> _logger;

        readonly IConfiguration _configuration;
        readonly IServiceProvider _serviceProvider;

        public WeatherForecastController(
            IHubContext<SimulationHub> hub,
            ILogger<WeatherForecastController> logger,
            IConfiguration configuration,
            IServiceProvider serviceProvider)
        {
            _hub = hub;
            _logger = logger;
            _configuration = configuration;
            _serviceProvider = serviceProvider;
            _serviceProvider.StartHubConnection(_configuration["HubUrl"]);
            Console.WriteLine("constructor called");
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

        [HttpGet]
        [Route("startSimulation")]
        public OkResult Start()
        {
            Console.WriteLine("Simulation started.");
            _serviceProvider.StartSimulation();
            Console.WriteLine("Simulation Finished.");
            return Ok();
        }

        [HttpGet]
        [Route("test")]
        public OkObjectResult Test()
        {
            //Console.WriteLine("Simulation started.");
            //serviceProvider.StartSimulation();
            //Console.WriteLine("Simulation Finished.");
            return Ok("this is a test");
        }

        [HttpGet]
        [Route("reset")]
        public IActionResult Reset()
        {
            _serviceProvider.Reset();
            return Ok();
        }
    }
}

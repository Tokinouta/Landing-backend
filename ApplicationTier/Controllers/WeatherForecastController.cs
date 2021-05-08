using ApplicationTier.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ModelEntities;
using ModelEntities.Enumerations;
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

        [HttpGet("startSimulation")]
        public OkResult Start()
        {
            Console.WriteLine("Simulation started.");
            _serviceProvider.StartSimulation();
            Console.WriteLine("Simulation Finished.");
            return Ok();
        }

        [HttpGet("test")]
        public OkObjectResult Test()
        {
            //Console.WriteLine("Simulation started.");
            //serviceProvider.StartSimulation();
            //Console.WriteLine("Simulation Finished.");
            return Ok("this is a test");
        }

        [HttpGet("reset")]
        public IActionResult Reset()
        {
            _serviceProvider.Reset();
            return Ok();
        }

        [HttpPost("config")]
        public IActionResult SetConfig([FromBody] Configuration configuration)
        {
            return Ok(configuration);
        }

        [HttpGet("loadConfig")]
        public IActionResult LoadConfigurationOptions()
        {
            return Ok(new
            {
                GuidanceControllerOptions = new
                {
                    label = GetEnumNames<GuidanceConfig>(),
                    value = GetEnumValues<GuidanceConfig>()
                },
                AttitudeControllerOptions = new
                {
                    label = GetEnumNames<AttitudeConfig>(),
                    value = GetEnumValues<AttitudeConfig>()
                },
                AngularRateControllerOptions = new
                {
                    label = GetEnumNames<AngularRateConfig>(),
                    value = GetEnumValues<AngularRateConfig>()
                },
                DisturbanceObserverOptions = new
                {
                    label = GetEnumNames<DisturbanceObserverConfig>(),
                    value = GetEnumValues<DisturbanceObserverConfig>()
                },
                GuidanceFilterOptions = new
                {
                    label = GetEnumNames<GuidanceFilters>(),
                    value = GetEnumValues<GuidanceFilters>()
                },
                AttitudeFilterOptions = new
                {
                    label = GetEnumNames<AttitudeFilters>(),
                    value = GetEnumValues<AttitudeFilters>()
                },
                UseAttitudeTrackingDifferentiatorOptions = new
                {
                    label = new string[] { "true", "false" },
                    value = new bool[] { true, false }
                },
                TrajactoryConfigOptions = new
                {
                    label = GetEnumNames<TrajactoryType>(),
                    value = GetEnumValues<TrajactoryType>()
                },
                UseDisturbanceTypeIOptions = new
                {
                    label = new string[] { "true", "false" },
                    value = new bool[] { true, false }
                },
                IsWindEnabledOptions = new
                {
                    label = new string[] { "true", "false" },
                    value = new bool[] { true, false }
                },
                IsDeckCompensationEnabledOptions = new
                {
                    label = new string[] { "true", "false" },
                    value = new bool[] { true, false }
                },
                UseL1AdaptiveOptions = new
                {
                    label = new string[] { "true", "false" },
                    value = new bool[] { true, false }
                }
            });
        }

        [NonAction]
        public static List<TEnum> GetEnumValues<TEnum>() where TEnum : Enum
            => ((TEnum[])Enum.GetValues(typeof(TEnum))).ToList();

        [NonAction]
        public static string[] GetEnumNames<TEnum>() where TEnum : Enum
            => Enum.GetNames(typeof(TEnum));
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Polly.API.DAL;

namespace Polly.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private static bool breakerEnable { get; set; } = false;
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly PollyDBContext dBContext;
        public WeatherForecastController(ILogger<WeatherForecastController> logger, PollyDBContext pollyDBContext)
        {
            _logger = logger;
            dBContext = pollyDBContext;
        }

        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            if (breakerEnable)
            {
                throw new Exception("API Disabled");
            }
            var result = await dBContext.WeatherForecasts.ToListAsync();
            return result;
        }

        [HttpGet]
        [Route("{sleep}")]
        public async Task<IEnumerable<WeatherForecast>> Get(int sleep)
        {
            if (breakerEnable)
            {
                throw new Exception("API Disabled");
            }
            System.Threading.Thread.Sleep(sleep * 1000);
            var result = await dBContext.WeatherForecasts.ToListAsync();
            return result;
        }

        [HttpGet]
        [Route("breaker/{enable}")]
        public async Task<bool> Breaker(bool enable)
        {
            breakerEnable = enable;
            return await Task.Run(() => breakerEnable);

        }
    }
}

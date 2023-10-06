using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using RedLockNet;

namespace redislock.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IDistributedLockFactory _lockFactory;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, IDistributedLockFactory idf)
    {
        _logger = logger;
        _lockFactory = idf;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public async Task<IEnumerable<WeatherForecast>> Get()
    {
        await using var _lock = await _lockFactory
            .CreateLockAsync(typeof(WeatherForecastController).Name, TimeSpan.FromSeconds(60));
        
        if (_lock.IsAcquired)
        {
            await Task.Delay(TimeSpan.FromSeconds(10));
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            }).ToArray();
        }

        throw new InvalidOperationException("Resource is locked, try again later!");
    }
}

using Dapper;
using Keep.Redbox;
using Keep.Redbox.SqlServer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sample.Redbox.Demo1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IRedbox _redbox;
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IRedisClientsManagerAsync _redisMng;
        private readonly string _connString;

        public WeatherForecastController(
            ILogger<WeatherForecastController> logger,
            IConfiguration configuration,
            IRedbox redbox,
            IRedisClientsManagerAsync redisClientsManager)
        {
            _logger = logger;
            _connString = configuration.GetConnectionString("DefaultConnection");
            _redbox = redbox;
            _redisMng = redisClientsManager;
        }

        //[HttpGet("cities/{city}/avgtemp")]
        //public async Task<IActionResult> GetAvgTemperature(string city)
        //{
        //    var key = $"{city}_avg_temp".ToLower();
        //    await using (var redis = await _redisMng.GetClientAsync())
        //    {
        //        double avg;
        //        if (await redis.ContainsKeyAsync(key))
        //        {
        //            avg = await redis.GetAsync<double>(key);
        //            return Ok(avg);
        //        }
        //        using (var conn = new SqlConnection(_connString))
        //        {
        //            await Task.Delay(5000);
        //            avg = await conn.ExecuteScalarAsync<double>(
        //                "select avg(Temperature) from Record where City = @City",
        //                new { City = city });
        //        }
        //        await redis.SetAsync(key, avg);
        //        return Ok(Math.Round(avg,1));
        //    }
        //}

        [HttpGet("cities/{city}/avgtemp")]
        public async Task<IActionResult> GetAvgTemperature2(string city)
        {
            var key = $"{city}_avg_temp".ToLower();
            var avg = await _redbox.GetAsync(key,
                async () =>
                {
                    using (var conn = new SqlConnection(_connString))
                    {
                        await Task.Delay(5000);
                        var result = await conn.ExecuteScalarAsync<double?>(
                            "select avg(Temperature) from Record where City = @City",
                            new { City = city });
                        return result;
                    }
                });
            if (avg.HasValue)
            {
                return Ok(Math.Round(avg.Value, 1));
            }
            return NotFound();
        }

        [HttpPost("records")]
        public async Task<IActionResult> CreateRecord(Record record)
        {
            record.Id = Guid.NewGuid();
            var key = $"{record.City}_avg_temp".ToLower();
            var another = "this-is-another-redis-key";
            using (var conn = new SqlConnection(_connString))
            {
                using (var tx = conn.BeginTransaction(_redbox, true))
                {
                    await conn.ExecuteAsync(
                        "insert into Record values(@Id, @City, @Temperature)",
                        record,
                        tx);
                    _redbox.Remove(key, another);
                }
                return Created($"records/{record.Id}", record);
            }
        }
    }

    public class Record
    {
        public Guid Id { get; set; }
        public string City { get; set; }
        public double Temperature { get; set; }
    }
}

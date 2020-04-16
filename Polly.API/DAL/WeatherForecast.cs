using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Polly.API.DAL
{
    [Table("WeatherForecast")]
    public class WeatherForecast
    {
        [Key]
        public int ForecastId { get; set; }
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public string Summary { get; set; }
    }
}

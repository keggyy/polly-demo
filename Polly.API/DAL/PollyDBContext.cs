using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Polly.API.DAL
{
    public class PollyDBContext:DbContext
    {
        public DbSet<WeatherForecast> WeatherForecasts { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=.\\Demo.db", sqliteOptionsAction: opt =>{
                opt.ExecutionStrategy(context => new ResilientExecutionStrategy(context, 3, TimeSpan.FromSeconds(2)));
                opt.CommandTimeout(2);
            });
            optionsBuilder.AddInterceptors(new DelayInterceptor());
        }

        public override int SaveChanges()
        {
            return base.SaveChanges();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Polly.API.DAL;

namespace Polly.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<PollyDBContext>();
            services.AddCors(opt => opt.AddDefaultPolicy(p =>
            {
                p.AllowAnyHeader();
                p.AllowAnyMethod();
                p.AllowAnyOrigin();
            }));
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Polly API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Polly API V1");
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            /** INIT DATABASE **/
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var db = scope.ServiceProvider.GetService<PollyDBContext>();
                db.Database.EnsureCreated();
                if (db.WeatherForecasts.Count() == 0)
                {
                    var rng = new Random();
                    var summaries = new[]
                    {
                "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
            };
                    var entities = Enumerable.Range(1, 6).Select(index => new WeatherForecast
                    {
                        ForecastId = index,
                        Date = DateTime.Now.AddDays(index),
                        TemperatureC = rng.Next(-20, 55),
                        Summary = summaries[rng.Next(summaries.Length)]
                    })
                    .ToArray();

                    db.AddRangeAsync(entities);
                    db.SaveChanges();
                }
                DelayInterceptor.EnableInterceptor = true;
            }
        }
    }
}

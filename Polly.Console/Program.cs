using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly.Console.Classes;
using Polly.Console.Classes.Impl;
using Polly.Console.Classes.Policies;
using Polly.Console.NSwag;
using Polly.Extensions.Http;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Polly.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
            .ConfigureServices((hostContext, services) =>
            {

                services.AddSingleton<Logger>();
                services.AddScoped<TraceHandler>();
                services.AddScoped<ClassicExecutor>();
                services.AddScoped<WaitAndRetrayExecutor>();
                services.AddScoped<RetrayForeverExecutor>();
                services.AddScoped<CircuitBreakExecutor>();

                Action<HttpClient> clientOption = opt =>
                {
                    opt.BaseAddress = new Uri("https://localhost:44381/");
                    //Polly work inside this timeout. For example enable RetryForever in any case retry throw exception with client timeout
                    //opt.Timeout = TimeSpan.FromSeconds(15);
                };

                /**** Register multiple clients for use case *****/

                //Classic client
                services.AddHttpClient<DemoClient>("Classic", clientOption)
                .AddHttpMessageHandler<TraceHandler>();

                //Wait and Retry 5 times with delay waiting Async client
                services.AddHttpClient<DemoClient>("WaitAndRetry", clientOption)
                .AddHttpMessageHandler<TraceHandler>()
                .AddWaitAndRetryPolicy()
                .AddTimeoutPolicy();

                //Retry forever with 1 second of delay for each call 
                services.AddHttpClient<DemoClient>("RetryForever", clientOption)
                .AddHttpMessageHandler<TraceHandler>()
                .AddRetryForever()
                .AddTimeoutPolicy();

                //Circuit Break
                services.AddHttpClient<DemoClient>("CircuitBreak", clientOption)
                .AddHttpMessageHandler<TraceHandler>()
                .AddWaitAndRetryPolicy(5)
                .AddCircuitBreaker()
                .AddTimeoutPolicy();

            }).UseConsoleLifetime();

            var host = builder.Build();

            using (var serviceScope = host.Services.CreateScope())
            {
                var services = serviceScope.ServiceProvider;
                var logger = services.GetService<Logger>();
                var exit = false;
                do
                {
                    logger.WriteInfo("Select from available test:");
                    logger.WriteInfo("1 - Classic implementation API");
                    logger.WriteInfo("2 - 10 seconds for Timeout with 5 retry and increasing elapsed retry");
                    logger.WriteInfo("3 - 10 seconds for Timeout with forever retry and elapsed of 1 second for each request");
                    logger.WriteInfo("4 - Circuit breaker. First call disable API response and polling API until breaker resume");

                    logger.WriteInfo("Digit number test [1 - 4]:");
                    var key = System.Console.ReadKey();
                    IExecutor executor;
                    ICollection<WeatherForecast> res;
                    int count;
                    try
                    {
                        switch (key.KeyChar.ToString())
                        {
                            case "1":
                                executor = services.GetService<ClassicExecutor>();
                                res = await executor.Execute(5);
                                count = res.Count;
                                logger.WriteInfo($"Result Count {count}");
                                break;
                            case "2":
                                executor = services.GetService<WaitAndRetrayExecutor>();
                                res = await executor.Execute();
                                count = res.Count;
                                logger.WriteInfo($"Result Count {count}");
                                break;
                            case "3":
                                executor = services.GetService<RetrayForeverExecutor>();
                                res = await executor.Execute();
                                count = res.Count;
                                logger.WriteInfo($"Result Count {count}");
                                break;
                            case "4":
                                executor = services.GetService<CircuitBreakExecutor>();
                                var classicExecutor = services.GetService<ClassicExecutor>();
                                var breakApi = await classicExecutor.BreakApi(true);
                                logger.WriteInfo($"Api Disabled: {breakApi}");
                                if (breakApi)
                                {
                                    var start = DateTime.Now;
                                    var exitWhile = false;
                                    do
                                    {
                                        try
                                        {
                                            logger.WriteInfo("Begin new Request");
                                            var response = await executor.Execute();
                                            count = response.Count;
                                            logger.WriteInfo($"Result Count {count}");
                                            exitWhile = count > 0;
                                            System.Threading.Thread.Sleep(2000);
                                        }
                                        catch (Exception ex)
                                        {
                                            logger.WriteError(ex.Message);
                                        }

                                        if (DateTime.Now.Subtract(start).TotalSeconds > 25 && !exitWhile)
                                        {
                                            await classicExecutor.BreakApi(false);
                                            logger.WriteInfo("API Enabled");
                                        }
                                    } while (DateTime.Now.Subtract(start).TotalSeconds < 30 && !exitWhile);
                                }
                                else
                                {
                                    logger.WriteInfo("Test failed, retry.");
                                }
                                break;
                            default:
                                logger.WriteInfo("Test not available");
                                break;
                        }
                    }catch(Exception ex)
                    {
                        logger.WriteError(ex.ToString());
                    }

                    logger.WriteInfo("Do you want execute other test? (Y/N)");
                    var result = System.Console.ReadKey();
                    exit = result.KeyChar.ToString()?.ToLower() == "y";

                } while (exit);
            }
        }
    }
}

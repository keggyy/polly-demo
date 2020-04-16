using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Polly.Console.Classes.Policies
{
    public static class CircuitBreakerPolicy
    {
        public static IHttpClientBuilder AddCircuitBreaker(this IHttpClientBuilder builder, int numEventsBeforeBreak = 3)
        {
            var services = builder.Services.BuildServiceProvider();
            var logger = services.GetService<Logger>();
            var policy = Policy.HandleResult<HttpResponseMessage>(r =>
            {
                if (!r.IsSuccessStatusCode)
                {
                    logger.WriteError($"Circuit Breaker: Response status code not success =>  {r.StatusCode}");
                }
                return !r.IsSuccessStatusCode;
            }).Or<Exception>(r =>
            {
                logger.WriteError("Polly Circuit Breaker catch exception");
                logger.WriteError(r.Message);
                return true;
            })
            .CircuitBreakerAsync(numEventsBeforeBreak, TimeSpan.FromSeconds(30), (message, timeSpan) =>
            {
                logger.WriteInfo("Circuit break start");
            }, () => {
                logger.WriteInfo("Circuit break resume");
            });
            return builder.AddPolicyHandler(policy);
        }
    }
}

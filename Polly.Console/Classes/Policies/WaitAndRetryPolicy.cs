using Microsoft.Extensions.DependencyInjection;
using Polly.Extensions.Http;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Polly.Console.Classes.Policies
{
    public static class WaitAndRetryPolicy
    {
        public static IHttpClientBuilder AddWaitAndRetryPolicy(this IHttpClientBuilder builder, int retryCount = 5)
        {
            var services = builder.Services.BuildServiceProvider();
            var logger = services.GetService<Logger>();
            var policy = Policy.HandleResult<HttpResponseMessage>(r =>
            {
                if (!r.IsSuccessStatusCode)
                {
                    logger.WriteError($"Wait and Retry: Response status code not success =>  {r.StatusCode}");
                }
                return !r.IsSuccessStatusCode;
            }).Or<Exception>(r =>
            {
                logger.WriteError("Polly Waiting and Retry catch exception");
                logger.WriteError(r.Message);
                return true;
            })
            .WaitAndRetryAsync(retryCount, (input) => TimeSpan.FromSeconds(2 + input), (result, timeSpan, retryCount, context) =>
                    {
                        logger.WriteInfo($"Begin {retryCount} °th retry for correlation {context.CorrelationId} with {timeSpan.TotalSeconds} seconds of delay");
                    });
            return builder.AddPolicyHandler(policy);
        }
    }
}

using Microsoft.Extensions.DependencyInjection;
using Polly.Extensions.Http;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Polly.Console.Classes.Policies
{
    public static class RetryForeverPolicy
    {
        public static IHttpClientBuilder AddRetryForever(this IHttpClientBuilder builder, int elapse = 1)
        {
            var services = builder.Services.BuildServiceProvider();
            var logger = services.GetService<Logger>();
            var policy = Policy.HandleResult<HttpResponseMessage>(r =>
            {
                if (!r.IsSuccessStatusCode)
                {
                    logger.WriteError($"Retry Forever: Response status code not success =>  {r.StatusCode}");
                }
                return !r.IsSuccessStatusCode;
            }).Or<Exception>(r =>
            {
                logger.WriteError("Polly Retry Forever catch exception");
                logger.WriteError(r.Message);
                return true;
            })
            .WaitAndRetryForeverAsync((input, context) => TimeSpan.FromSeconds(elapse), onRetry: (message, retryCount, timeSpan, context) =>
                    {
                        logger.WriteInfo($"Begin {retryCount} °th retry for correlation {context.CorrelationId} with {timeSpan.Seconds} seconds of delay");
                    });
            return builder.AddPolicyHandler(policy);
        }
    }
}

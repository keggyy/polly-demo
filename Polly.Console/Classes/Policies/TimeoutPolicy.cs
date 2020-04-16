using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Polly.Console.Classes.Policies
{
    public static class TimeoutPolicy
    {
        public static IHttpClientBuilder AddTimeoutPolicy(this IHttpClientBuilder builder, int seconds = 5)
        {

            var policy = Policy.TimeoutAsync<HttpResponseMessage>(seconds);
            return builder.AddPolicyHandler(policy);
        }
    }
}

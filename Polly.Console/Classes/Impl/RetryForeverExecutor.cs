using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Polly.Console.Classes.Impl
{
    public class RetrayForeverExecutor : IExecutor
    {
        public RetrayForeverExecutor(IHttpClientFactory clientFactory) : base(clientFactory, "RetryForever")
        {
        }
    }
}

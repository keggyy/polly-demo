using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Polly.Console.Classes.Impl
{
    public class WaitAndRetrayExecutor : IExecutor
    {
        public WaitAndRetrayExecutor(IHttpClientFactory clientFactory) : base(clientFactory, "WaitAndRetry")
        {
        }
    }
}

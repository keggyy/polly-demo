using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Polly.Console.Classes.Impl
{
    public class CircuitBreakExecutor : IExecutor
    {
        public CircuitBreakExecutor(IHttpClientFactory clientFactory) : base(clientFactory, "CircuitBreak")
        {
        }
    }
}

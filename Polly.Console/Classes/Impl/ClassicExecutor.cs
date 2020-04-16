using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Polly.Console.Classes.Impl
{
    public class ClassicExecutor : IExecutor
    {

        public ClassicExecutor(IHttpClientFactory clientFactory) : base(clientFactory, "Classic")
        {

        }
    }
}

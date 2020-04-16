using Polly.Console.NSwag;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Polly.Console.Classes
{
    public abstract class IExecutor
    {
        internal DemoClient _client { get; set; }
        private HttpClient client { get; set; }
        public IExecutor(IHttpClientFactory clientFactory, string clientName)
        {
            var httpClient =  clientFactory.CreateClient(clientName);
            client = httpClient;
            _client = new DemoClient(httpClient.BaseAddress.ToString(), httpClient);
        }

        public virtual async Task<ICollection<WeatherForecast>> Execute()
        {
            var result = await _client.WeatherForecastAllAsync();
            return result;
        }

        public virtual async Task<ICollection<WeatherForecast>> Execute(int sleep)
        {
            var result = await _client.WeatherForecastAsync(sleep);
            return result;
        }

        public virtual async Task<bool> BreakApi(bool breakApi)
        {
            var result = await _client.BreakerAsync(breakApi);
            return result;
        }
    }
}

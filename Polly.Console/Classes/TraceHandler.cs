using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Polly.Console.Classes
{
    public class TraceHandler: DelegatingHandler
    {
        private readonly Logger logger;
        public TraceHandler(Logger _logger)
        {
            logger = _logger;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                logger.WriteInfo($"Begin Request => " + request.RequestUri);
                var response = base.SendAsync(request, cancellationToken);
                if (response.Result.IsSuccessStatusCode)
                {
                    logger.WriteInfo("Request Success");
                }
                return response;
            }catch(Exception ex)
            {
                logger.WriteError("Error: Request cancelled");
                throw;
            }
        }
    }
}

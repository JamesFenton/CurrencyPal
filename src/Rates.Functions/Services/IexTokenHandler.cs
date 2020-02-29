using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rates.Functions.Services
{
    public class IexTokenHandler : DelegatingHandler
    {
        private readonly string _token;

        public IexTokenHandler(string token)
        {
            _token = token;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // add token to query string
            request.RequestUri = new Uri(request.RequestUri.ToString() + $"token={_token}");
            return base.SendAsync(request, cancellationToken);
        }
    }
}

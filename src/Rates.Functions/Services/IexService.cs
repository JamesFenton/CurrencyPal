using Polly.Retry;
using Rates.Functions.WriteModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Rates.Functions.Services
{
    public class IexService : IRatesService
    {
        private readonly HttpClient _http = new HttpClient();
        private readonly string _token;
        private readonly RetryPolicy _retryPolicy;

        public Rate[] Rates => new[]
        {
            Rate.VOOUSD,
        };

        public IexService(Settings settings, RetryPolicy retryPolicy)
        {
            _token = settings.IexApiKey;
            _retryPolicy = retryPolicy;
        }
        
        public Task<IEnumerable<RateEntity>> GetRates()
        {
            return _retryPolicy.ExecuteAsync(() => Get());
        }

        private async Task<IEnumerable<RateEntity>> Get()
        {
            return await Task.WhenAll(Rates.Select(GetRate));
        }

        private async Task<RateEntity> GetRate(Rate rate)
        {
            var iexSymbol = GetIexSymbol(rate.Ticker);
            var url = $"https://cloud.iexapis.com/v1/stock/{iexSymbol}/price?token={_token}";
            var response = await _http.GetStringAsync(url);
            var value = double.Parse(response);
            return new RateEntity(rate.Ticker, DateTimeOffset.UtcNow, value);
        }

        private string GetIexSymbol(string ticker)
        {
            // VOOUSD -> VOO
            return ticker.Substring(0, 3);
        }
    }
}

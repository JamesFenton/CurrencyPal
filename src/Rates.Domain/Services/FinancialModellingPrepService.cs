using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Polly.Retry;
using Rates.Core.WriteModel;

namespace Rates.Domain.Services
{
    public class FinancialModellingPrepService : IRatesService
    {
        private readonly HttpClient _http = new HttpClient();
        private readonly RetryPolicy _retryPolicy;

        public FinancialModellingPrepService(RetryPolicy retryPolicy)
        {
            _retryPolicy = retryPolicy;
        }

        public async Task<List<Rate>> GetRates(IEnumerable<string> tickers)
        {
            var json = await _retryPolicy.ExecuteAsync(() => _http.GetStringAsync("https://financialmodelingprep.com/api/majors-indexes"));
            var response = JObject.Parse(json.Replace("<pre>", ""));

            var rates = tickers.Select(ticker =>
            {
                var symbol = GetSymbol(ticker);
                var rate = response[symbol]["Price"].Value<double>();
                return new Rate(ticker, DateTimeOffset.UtcNow, rate);
            });

            return rates.ToList();

            string GetSymbol(string ticker)
            {
                switch (ticker)
                {
                    case "SPXUSD":
                        return ".INX";
                    default:
                        throw new ArgumentException("Unsupported ticker: " + ticker);
                }
            }
        }
    }
}

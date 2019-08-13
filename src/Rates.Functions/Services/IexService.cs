using Rates.Functions.WriteModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Rates.Functions.Services
{
    public class IexService
    {
        private readonly HttpClient _http = new HttpClient
        {
            BaseAddress = new Uri("https://cloud.iexapis.com")
        };
        private readonly string _token;

        public IexService(Settings settings)
        {
            _token = settings.IexApiKey;
        }
        
        public async Task<IEnumerable<RateEntity>> GetRates(IEnumerable<Rate> rates)
        {
            return await Task.WhenAll(rates.Select(GetRate));
        }

        private async Task<RateEntity> GetRate(Rate rate)
        {
            var iexSymbol = rate.SourceSymbol;
            var url = $"/v1/stock/{iexSymbol}/quote/latestPrice?token={_token}";
            var response = await _http.GetStringAsync(url);
            var value = double.Parse(response);
            return new RateEntity(rate.Ticker, DateTimeOffset.UtcNow, value);
        }
    }
}

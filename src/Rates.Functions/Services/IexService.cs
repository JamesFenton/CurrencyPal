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

        public Rate[] Rates => new[]
        {
            Rate.VOOUSD,
        };
        
        public async Task<IEnumerable<RateEntity>> GetRates()
        {
            var rates = await Task.WhenAll(Rates.Select(GetRate));
            return rates.ToList();
        }

        private async Task<RateEntity> GetRate(Rate rate)
        {
            var iexSymbol = GetIexSymbol(rate.Ticker);
            var url = $"https://api.iextrading.com/1.0/stock/{iexSymbol}/price";
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

using CurrencyPal.Dto;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CurrencyPal.Services
{
    public class RateService
    {
        private readonly HttpClient _Client = new HttpClient();

        public async Task<RateDto> GetExchangeRate(string ticker)
        {
            var tickerToLookup = ticker.ToUpperInvariant();
            if (tickerToLookup == "XBTZAR")
            {
                var response = await _Client.GetStringAsync($"https://api.mybitx.com/api/1/ticker?pair={tickerToLookup}");
                var rate = JObject.Parse(response).Value<double>("last_trade");
                return new RateDto
                {
                    Ticker = tickerToLookup,
                    Rate = rate,
                    Source = "Luno"
                };
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}

using CurrencyPal.Dto;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CurrencyPal.Services
{
    public class RateService : IRateService
    {
        private readonly HttpClient _Client = new HttpClient();
        private readonly string _CurrencyLayerAppId = Environment.GetEnvironmentVariable("CURRENCYLAYER_APPID");
        private readonly string[] _Tickers = { "USDZAR", "GBPZAR", "ZARMUR", "BTCZAR", "BTCUSD" };

        public async Task<RatesDto> GetExchangeRates()
        {
            var bitcoin = await GetBitcoin();
            var sourceRates = await GetRates();
            
            var dto = new RatesDto();
            foreach (var ticker in _Tickers)
            {
                double rate = ticker == "BTCZAR" ? bitcoin : GetRate(ticker, sourceRates);
                
                dto.Rates.Add(new RateDto
                {
                    Rate = rate,
                    Ticker = ticker
                });
            }

            return dto;
        }

        private async Task<double> GetBitcoin()
        {
            var response = await _Client.GetStringAsync($"https://api.mybitx.com/api/1/ticker?pair=XBTZAR");
            var rate = JObject.Parse(response)["last_trade"].ToObject<double>();
            return rate;
        }

        private async Task<Dictionary<string, double>> GetRates()
        {
            var ratesResponse = await _Client.GetStringAsync($"https://openexchangerates.org/api/latest.json?app_id={_CurrencyLayerAppId}");
            var sourceRates = JObject.Parse(ratesResponse)["rates"].ToObject<Dictionary<string, double>>();
            return sourceRates;
        }

        private static double GetRate(string ticker, Dictionary<string, double> rates)
        {
            var source = ticker.Substring(0, 3);
            var dest = ticker.Substring(3, 3);

            var sourceRate = rates[source];
            var destRate = rates[dest];
            var crossRate = destRate / sourceRate;
            return crossRate;
        }
    }
}

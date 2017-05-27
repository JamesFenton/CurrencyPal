using Newtonsoft.Json.Linq;
using Rates.Web.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Rates.Web.Services
{
    public class RatesService : IRatesService
    {
        private readonly string[] _fiatTickers = { "USDZAR", "GBPZAR", "EURZAR", "ZARMUR" };

        private readonly string _openExchangeRatesKey;
        private readonly HttpClient _http;

        public RatesService(string openExchangeRatesKey, HttpClient http)
        {
            _openExchangeRatesKey = openExchangeRatesKey;
            _http = http;
        }

        public async Task<RatesDto> GetRates()
        {
            var getExchangeRates = GetExchangeRates();
            var getBitcoinZar = GetBitcoinZar();
            var getCryptoCurrencies = GetCryptoCurrencies();

            var exchangeRates = await getExchangeRates;
            var bitcoinZar = await getBitcoinZar;
            var cryptoCurrencies = await getCryptoCurrencies;

            var rates = new List<RateDto>();
            rates.AddRange(exchangeRates);
            rates.Add(bitcoinZar);
            rates.AddRange(cryptoCurrencies);

            return new RatesDto
            {
                Rates = rates,
                UpdateTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            };
        }

        private async Task<List<RateDto>> GetExchangeRates()
        {
            var response = await _http.GetStringAsync("https://openexchangerates.org/api/latest.json?app_id=" + _openExchangeRatesKey);
            var sourceRates = JObject.Parse(response)["rates"] as JObject;

            var rates = _fiatTickers.Select(t => ConvertRate(t)).ToList();
            return rates;

            RateDto ConvertRate(string ticker)
            {
                var source = ticker.Substring(0, 3);
                var dest = ticker.Substring(3, 3);

                var sourceRate = sourceRates[source].Value<double>();
                var destRate = sourceRates[dest].Value<double>();
                var crossRate = destRate / sourceRate;
                return new RateDto { Ticker = ticker, Rate = crossRate };
            }
        }
        
        private async Task<RateDto> GetBitcoinZar()
        {
            var response = await _http.GetStringAsync("https://api.mybitx.com/api/1/ticker?pair=XBTZAR");
            var responseObject = JObject.Parse(response) as JObject;
            var rate = responseObject["last_trade"].Value<double>();
            return new RateDto
            {
                Ticker = "BTCZAR",
                Rate = rate
            };
        }

        private async Task<List<RateDto>> GetCryptoCurrencies()
        {
            var getBitcoin = GetRate("BTCUSD");
            var getEthereum = GetRate("ETHUSD");

            var bitcoin = await getBitcoin;
            var ethereum = await getEthereum;

            return new List<RateDto>
            {
                bitcoin,
                ethereum
            };

            async Task<RateDto> GetRate(string ticker)
            {
                string coinmarketcapTicker;
                switch (ticker)
                {
                    case "BTCUSD":
                        coinmarketcapTicker = "bitcoin";
                        break;
                    case "ETHUSD":
                        coinmarketcapTicker = "ethereum";
                        break;
                    default:
                        throw new ArgumentException("Invalid ticker: " + ticker);
                }
                var response = await _http.GetStringAsync("https://api.coinmarketcap.com/v1/ticker/" + coinmarketcapTicker);
                var rateReponse = JArray.Parse(response).First();
                var rate = rateReponse["price_usd"].Value<double>();
                var change24h = rateReponse["percent_change_24h"].Value<double>();
                return new RateDto
                {
                    Ticker = ticker,
                    Rate = rate,
                    Change24h = change24h
                };
            }
        }
    }
}

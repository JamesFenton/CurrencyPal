using Rates.Core;
using Rates.Core.WriteModel;
using Rates.Fetcher.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rates.Fetcher
{
    public class RatesFetcher
    {
        private readonly OpenExchangeRatesService _openExchangeRatesService;
        private readonly CoinMarketCapService _coinMarketCapService;

        public RatesFetcher(
            OpenExchangeRatesService openExchangeRatesService,
            CoinMarketCapService coinMarketCapService)
        {
            _openExchangeRatesService = openExchangeRatesService;
            _coinMarketCapService = coinMarketCapService;
        }

        public async Task<List<Rate>> GetRates()
        {
            var openExchangeRateTickers = Constants.FiatTickers.Concat(Constants.MetalsTickers);
            var tasks = new List<Task<List<Rate>>>
            {
                _openExchangeRatesService.GetExchangeRates(openExchangeRateTickers),
                _coinMarketCapService.GetCryptoCurrencies(Constants.CryptoTickers),
            };

            await Task.WhenAll(tasks);

            var rates = tasks.SelectMany(t => t.Result).ToList();

            return rates;
        }
    }
}

using Rates.Functions.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.IO;
using Newtonsoft.Json;
using Rates.Functions.Models;

namespace Rates.Functions.Functions
{
    public class FetchSaveRates
    {
        private readonly Dictionary<RateSource, IRatesService> _ratesServices;

        public FetchSaveRates(
            CoinMarketCapService coinMarketCapService,
            IexService iexService,
            OpenExchangeRatesService openExchangeRatesService
        )
        {
            _ratesServices = new Dictionary<RateSource, IRatesService>
            {
                [RateSource.CoinMarketCap] = coinMarketCapService,
                [RateSource.Iex] = iexService,
                [RateSource.OpenExchangeRates] = openExchangeRatesService
            };
        }

        [FunctionName("FetchFromCoinMarketCap")]
        public async Task FetchFromCoinMarketCap(
            [TimerTrigger("0 0 * * * *")] TimerInfo myTimer,
            [Blob("lookups/rates.json", FileAccess.Read)] string rateDefinitionsJson,
            [Queue(Constants.RatesAddedQueue)] ICollector<RateEntity> queueCollector,
            ILogger logger
        )
        {
            await GetRatesAsync(RateSource.CoinMarketCap, rateDefinitionsJson, queueCollector, logger);
        }

        [FunctionName("FetchFromIex")]
        public async Task FetchFromIex(
            [TimerTrigger("0 0 * * * *")] TimerInfo myTimer,
            [Blob("lookups/rates.json", FileAccess.Read)] string rateDefinitionsJson,
            [Queue(Constants.RatesAddedQueue)] ICollector<RateEntity> queueCollector,
            ILogger logger
        )
        {
            await GetRatesAsync(RateSource.Iex, rateDefinitionsJson, queueCollector, logger);
        }

        [FunctionName("FetchFromOpenExchangeRates")]
        public async Task FetchFromOpenExchangeRates(
            [TimerTrigger("0 0 * * * *")] TimerInfo myTimer,
            [Blob("lookups/rates.json", FileAccess.Read)] string rateDefinitionsJson,
            [Queue(Constants.RatesAddedQueue)] ICollector<RateEntity> queueCollector,
            ILogger logger
        )
        {
            await GetRatesAsync(RateSource.OpenExchangeRates, rateDefinitionsJson, queueCollector, logger);
        }

        private async Task GetRatesAsync(
            RateSource rateSource,
            string rateDefinitionsJson,
            ICollector<RateEntity> queueCollector,
            ILogger logger
        )
        {
            var rateDefinitions = JsonConvert.DeserializeObject<List<Rate>>(rateDefinitionsJson);

            if (!_ratesServices.TryGetValue(rateSource, out var service))
                throw new ArgumentException($"No service available for rate source {rateSource}");

            // chose rates for this source
            var rateLookups = rateDefinitions.Where(r => r.Source == rateSource);

            // fetch rates
            var rates = await service.GetRates(rateLookups);

            // save to table storage & send queue message
            logger.LogInformation($"Got {rates.Count()} rates. Saving to storage.");
            foreach (var rate in rates)
            {
                await rate.SaveAsync();
                queueCollector.Add(rate);

                var rateHistory = new RateHistory(rate.Ticker, rate.Value);
                await rateHistory.SaveAsync();
            }
        }
    }
}

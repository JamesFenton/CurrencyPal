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
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace Rates.Functions.WriteModel
{
    public class FetchSaveRates
    {
        private readonly Dictionary<RateSource, IRatesService> _ratesServices;
        private readonly ILogger<FetchSaveRates> _logger;

        public FetchSaveRates(
            CoinMarketCapService coinMarketCapService,
            IexService iexService,
            OpenExchangeRatesService openExchangeRatesService,
            ILogger<FetchSaveRates> logger
        )
        {
            _ratesServices = new Dictionary<RateSource, IRatesService>
            {
                [RateSource.CoinMarketCap] = coinMarketCapService,
                [RateSource.Iex] = iexService,
                [RateSource.OpenExchangeRates] = openExchangeRatesService
            };
            _logger = logger;
        }

        [FunctionName("FetchFromCoinMarketCap")]
        public async Task FetchFromCoinMarketCap(
            [TimerTrigger("0 0 * * * *")] TimerInfo myTimer,
            [Blob("lookups/rates.json", FileAccess.Read)] string rateDefinitionsJson,
            [Table(Database.RatesTable)] CloudTable table,
            [Queue(Constants.RatesAddedQueue)] ICollector<RateEntity> queueCollector
        )
        {
            await GetRatesAsync(RateSource.CoinMarketCap, rateDefinitionsJson, table, queueCollector);
        }

        [FunctionName("FetchFromIex")]
        public async Task FetchFromIex(
            [TimerTrigger("0 0 * * * *")] TimerInfo myTimer,
            [Blob("lookups/rates.json", FileAccess.Read)] string rateDefinitionsJson,
            [Table(Database.RatesTable)] CloudTable table,
            [Queue(Constants.RatesAddedQueue)] ICollector<RateEntity> queueCollector
        )
        {
            await GetRatesAsync(RateSource.CoinMarketCap, rateDefinitionsJson, table, queueCollector);
        }

        [FunctionName("FetchFromOpenExchangeRates")]
        public async Task FetchFromOpenExchangeRates(
            [TimerTrigger("0 0 * * * *")] TimerInfo myTimer,
            [Blob("lookups/rates.json", FileAccess.Read)] string rateDefinitionsJson,
            [Table(Database.RatesTable)] CloudTable table,
            [Queue(Constants.RatesAddedQueue)] ICollector<RateEntity> queueCollector
        )
        {
            await GetRatesAsync(RateSource.CoinMarketCap, rateDefinitionsJson, table, queueCollector);
        }

        private async Task GetRatesAsync(
            RateSource rateSource,
            string rateDefinitionsJson,
            CloudTable table,
            ICollector<RateEntity> queueCollector
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
            _logger.LogInformation($"Got {rates.Count()} rates. Saving to storage.");
            foreach (var rate in rates)
            {
                var operation = TableOperation.InsertOrReplace(rate);
                await table.ExecuteAsync(operation);
                queueCollector.Add(rate);
            }
        }
    }
}

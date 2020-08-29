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

namespace Rates.Functions.WriteModel
{
    public class FetchSaveRates
    {
        private readonly Database _database;
        private readonly Dictionary<RateSource, IRatesService> _ratesServices;
        private readonly ILogger<FetchSaveRates> _logger;

        public FetchSaveRates(
            Database database,
            CoinMarketCapService coinMarketCapService,
            IexService iexService,
            OpenExchangeRatesService openExchangeRatesService,
            ILogger<FetchSaveRates> logger
        )
        {
            _database = database;
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
            [Blob("lookups/rates.json", FileAccess.Read)] List<Rate> rateDefinitions,
            [Table(Database.RatesTable)] CloudTable table,
            [Queue(Constants.RatesAddedQueue)] ICollector<RateEntity> queueCollector
        )
        {
            await GetRatesAsync(RateSource.CoinMarketCap, rateDefinitions, table, queueCollector);
        }

        [FunctionName("FetchFromIex")]
        public async Task FetchFromIex(
            [TimerTrigger("0 0 * * * *")] TimerInfo myTimer,
            [Blob("lookups/rates.json", FileAccess.Read)] List<Rate> rateDefinitions,
            [Table(Database.RatesTable)] CloudTable table,
            [Queue(Constants.RatesAddedQueue)] ICollector<RateEntity> queueCollector
        )
        {
            await GetRatesAsync(RateSource.CoinMarketCap, rateDefinitions, table, queueCollector);
        }

        [FunctionName("FetchFromOpenExchangeRates")]
        public async Task FetchFromOpenExchangeRates(
            [TimerTrigger("0 0 * * * *")] TimerInfo myTimer,
            [Blob("lookups/rates.json", FileAccess.Read)] List<Rate> rateDefinitions,
            [Table(Database.RatesTable)] CloudTable table,
            [Queue(Constants.RatesAddedQueue)] ICollector<RateEntity> queueCollector
        )
        {
            await GetRatesAsync(RateSource.CoinMarketCap, rateDefinitions, table, queueCollector);
        }

        private async Task GetRatesAsync(
            RateSource rateSource,
            List<Rate> rateDefinitions,
            CloudTable table,
            ICollector<RateEntity> queueCollector
        )
        {
            if (!_ratesServices.TryGetValue(rateSource, out var service))
                throw new ArgumentException($"No service available for rate source {rateSource}");

            // chose rates for this source
            var rateLookups = rateDefinitions.Where(r => r.Source == rateSource);

            // fetch rates
            var rates = await service.GetRates(rateLookups);

            // save to table storage & send queue message
            _logger.LogInformation($"Got {rates.Count()} rates. Saving to storage.");
            await Task.WhenAll(rates.Select(rate =>
            {
                var operation = TableOperation.InsertOrReplace(rate);
                return _database.Rates.ExecuteAsync(operation);
            }));

            foreach (var rate in rates)
                queueCollector.Add(rate);
        }
    }
}

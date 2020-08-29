using Rates.Functions.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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
        [return: Queue(Constants.RatesAddedQueue)]
        public async Task<string> FetchFromCoinMarketCap(
            [TimerTrigger("0 0 * * * *")] TimerInfo myTimer,
            [Blob("lookups/rates.json", FileAccess.Read)] string rateLookupsJson
        )
        {
            return await GetRatesAsync(RateSource.CoinMarketCap, rateLookupsJson);
        }

        [FunctionName("FetchFromIex")]
        [return: Queue(Constants.RatesAddedQueue)]
        public async Task<string> FetchFromIex(
            [TimerTrigger("0 0 * * * *")] TimerInfo myTimer,
            [Blob("lookups/rates.json", FileAccess.Read)] string rateLookupsJson
        )
        {
            return await GetRatesAsync(RateSource.Iex, rateLookupsJson);
        }

        [FunctionName("FetchFromOpenExchangeRates")]
        [return: Queue(Constants.RatesAddedQueue)]
        public async Task<string> FetchFromOpenExchangeRates(
            [TimerTrigger("0 0 * * * *")] TimerInfo myTimer,
            [Blob("lookups/rates.json", FileAccess.Read)] string rateLookupsJson
        )
        {
            return await GetRatesAsync(RateSource.OpenExchangeRates, rateLookupsJson);
        }

        private async Task<string> GetRatesAsync(RateSource rateSource, string rateLookupsJson)
        {
            if (!_ratesServices.TryGetValue(rateSource, out var service))
                throw new ArgumentException($"No service available for rate source {rateSource}");

            // chose rates for this source
            var rateLookups = JsonConvert.DeserializeObject<List<Rate>>(rateLookupsJson)
                .Where(r => r.Source == rateSource);

            // fetch rates
            var rates = await service.GetRates(rateLookups);

            // save to storage
            _logger.LogInformation($"Got {rates.Count()} rates. Saving to storage.");
            await Task.WhenAll(rates.Select(rate =>
            {
                var operation = TableOperation.InsertOrReplace(rate);
                return _database.Rates.ExecuteAsync(operation);
            }));

            // send queue message
            _logger.LogInformation($"Sending {rates.Count()} rates to {Constants.RatesAddedQueue} queue");
            return JsonConvert.SerializeObject(rates);
        }
    }
}

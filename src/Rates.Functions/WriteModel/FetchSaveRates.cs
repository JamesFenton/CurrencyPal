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
        private readonly IEnumerable<IRatesService> _ratesServices;

        public FetchSaveRates(
            IEnumerable<IRatesService> ratesServices, 
            Database database
        )
        {
            _ratesServices = ratesServices;
            _database = database;
        }

        [FunctionName("FetchSaveRates")]
        public async Task Run(
            [TimerTrigger("0 0 * * * *")]TimerInfo myTimer,
            [Blob("lookups/rates.json", FileAccess.Read)] string rateLookupsJson,
            [Queue(Constants.RatesAddedQueue)] ICollector<string> destinationQueue,
            ILogger log)
        {
            var rateLookups = JsonConvert.DeserializeObject<List<Rate>>(rateLookupsJson);

            // get rates
            log.LogInformation($"Getting {rateLookups.Count()} rates from {_ratesServices.Count()} services");
            var rates = (await Task.WhenAll(
                _ratesServices.Select(s => s.GetRates(rateLookups))
            ))
            .SelectMany(r => r)
            .ToList();

            // save to storage
            log.LogInformation($"Got {rates.Count()} rates. Saving to storage.");
            await Task.WhenAll(rates.Select(rate =>
            {
                var operation = TableOperation.InsertOrReplace(rate);
                return _database.Rates.ExecuteAsync(operation);
            }));

            // send queue message
            log.LogInformation($"Sending {rates.Count()} rates to {Constants.RatesAddedQueue} queue");
            var message = JsonConvert.SerializeObject(rates);
            destinationQueue.Add(message);
        }
    }
}

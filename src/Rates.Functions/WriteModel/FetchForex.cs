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

namespace Rates.Functions.WriteModel
{
    public class FetchForex
    {
        private readonly OpenExchangeRatesService _openExchangeRatesService;
        private readonly RateSaver _rateSaver;

        public FetchForex(OpenExchangeRatesService openExchangeRatesService, RateSaver rateSaver)
        {
            _openExchangeRatesService = openExchangeRatesService;
            _rateSaver = rateSaver;
        }

        [FunctionName("FetchForex")]
        public async Task Run(
            [TimerTrigger("0 0 * * * *")]TimerInfo myTimer,
            [Queue(Constants.RatesAddedQueue)] ICollector<string> destinationQueue,
            ILogger log)
        {
            var rates = await _openExchangeRatesService.GetRates();
            await _rateSaver.Save(rates);

            log.LogInformation($"Sending {rates.Count()} rates to {Constants.RatesAddedQueue} queue");
            foreach (var rate in rates)
            {
                var json = JsonConvert.SerializeObject(rate);
                destinationQueue.Add(json);
            }
        }
    }
}

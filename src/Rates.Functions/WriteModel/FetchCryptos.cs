using Rates.Functions.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Autofac;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Rates.Functions.WriteModel
{
    public class FetchCryptos
    {
        [FunctionName("FetchCryptos")]
        public static async Task Run(
            [TimerTrigger("0 0 * * * *")]TimerInfo myTimer,
            [Queue(Constants.RatesAddedQueue)] ICollector<string> destinationQueue,
            ILogger log)
        {
            var service = ContainerFactory.Container.ResolveNamed<IRatesService>("cryptos");
            var rateSaver = ContainerFactory.Container.Resolve<RateSaver>();
            
            var rates = await service.GetRates();
            await rateSaver.Save(rates);

            log.LogInformation($"Sending {rates.Count()} rates to {Constants.RatesAddedQueue} queue");
            foreach (var rate in rates)
            {
                var json = JsonConvert.SerializeObject(rate);
                destinationQueue.Add(json);
            }
        }
    }
}

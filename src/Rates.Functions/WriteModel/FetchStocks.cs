using MediatR;
using Microsoft.WindowsAzure.Storage.Table;
using Rates.Functions.Events;
using Rates.Functions.WriteModel;
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
    public class FetchStocks
    {
        [FunctionName("FetchStocks")]
        public static async Task Run(
            [TimerTrigger("0 0 * * * *")]TimerInfo myTimer,
            [Queue(Constants.RatesAddedQueue)] ICollector<string> destinationQueue,
            ILogger log)
        {
            var mediator = ContainerFactory.Container.Resolve<IMediator>();
            var service = ContainerFactory.Container.ResolveNamed<IRatesService>("stocks");

            var command = new FetchRates.Command { Service = service };
            var events = await mediator.Send(command);

            log.LogInformation($"Sending {events.Count()} events to {Constants.RatesAddedQueue} queue");
            foreach (var @event in events)
            {
                var json = JsonConvert.SerializeObject(@event, Constants.JsonSettings);
                destinationQueue.Add(json);
            }
        }
    }
}

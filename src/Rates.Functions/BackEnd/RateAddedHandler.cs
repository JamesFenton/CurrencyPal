using System;
using System.Threading.Tasks;
using Autofac;
using MediatR;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Rates.Core.Events;

namespace Rates.Functions.BackEnd
{
    public static class RateAddedHandler
    {
        [FunctionName("RateAddedHandler")]
        public static async Task Run(
            [QueueTrigger(Lookups.RatesAddedQueue)]string myQueueItem,
            ILogger log)
        {
            var rateAdded = JsonConvert.DeserializeObject<RateAdded>(myQueueItem, Lookups.JsonSettings);
            var mediator = ContainerFactory.Container.Resolve<IMediator>();
            
            await mediator.Send(rateAdded);
            log.LogInformation($"Rate {rateAdded.Ticker} {rateAdded.TimeKey} handled successfully");
        }
    }
}

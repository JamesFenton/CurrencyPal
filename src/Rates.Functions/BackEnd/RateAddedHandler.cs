using System;
using System.Threading.Tasks;
using Autofac;
using MediatR;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Rates.Fetcher.Commands;

namespace Rates.Functions.BackEnd
{
    public static class RateAddedHandler
    {
        [FunctionName("RateAddedHandler")]
        public static async Task Run(
            [QueueTrigger(Lookups.RatesAddedQueue)]string myQueueItem, 
            TraceWriter log)
        {
            var rateAdded = JsonConvert.DeserializeObject<RateAdded.Command>(myQueueItem, Lookups.JsonSettings);
            var mediator = ContainerFactory.Container.Resolve<IMediator>();

            await mediator.Send(rateAdded);
            log.Info($"Rate {rateAdded.Ticker} handled successfully");
        }
    }
}

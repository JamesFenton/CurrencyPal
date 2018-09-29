using System;
using System.Threading.Tasks;
using Autofac;
using MediatR;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Rates.Core.Events;

namespace Rates.Functions.BackEnd
{
    public static class RateAddedHandler
    {
        [FunctionName("RateAddedHandler")]
        public static async Task Run(
            [QueueTrigger(Lookups.RatesAddedQueue)]string myQueueItem, 
            TraceWriter log)
        {
            var rateAdded = JsonConvert.DeserializeObject<RateAdded>(myQueueItem, Lookups.JsonSettings);
            var mediator = ContainerFactory.Container.Resolve<IMediator>();

            var command = new Domain.Commands.RateAdded.Command
            {
                Id = rateAdded.Id,
                Ticker = rateAdded.Ticker,
                Timestamp = rateAdded.Timestamp,
                Value = rateAdded.Value,
            };
            await mediator.Send(command);
            log.Info($"Rate {rateAdded.Ticker} handled successfully");
        }
    }
}

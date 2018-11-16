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
    public class FetchRates
    {
        [FunctionName("FetchRates")]
        public static async Task Run(
            [TimerTrigger("0 0 * * * *")]TimerInfo myTimer,
            [Queue(Constants.RatesAddedQueue)] ICollector<string> destinationQueue,
            ILogger log)
        {
            var mediator = ContainerFactory.Container.Resolve<IMediator>();

            var events = await mediator.Send(new Command());

            log.LogInformation($"Sending {events.Count()} events to {Constants.RatesAddedQueue} queue");
            foreach (var @event in events)
            {
                var json = JsonConvert.SerializeObject(@event, Constants.JsonSettings);
                destinationQueue.Add(json);
            }
        }

        public class Command : IRequest<IEnumerable<Event>>
        {
        }

        public class Handler : IRequestHandler<Command, IEnumerable<Event>>
        {
            private readonly IEnumerable<IRatesService> _ratesServices;
            private readonly Database _database;

            public Handler(
                IEnumerable<IRatesService> ratesServices,
                Database database)
            {
                _ratesServices = ratesServices;
                _database = database;
            }

            public async Task<IEnumerable<Event>> Handle(Command request, CancellationToken cancellationToken)
            {
                var rates = await GetRates();

                foreach(var rate in rates)
                {
                    var operation = TableOperation.InsertOrReplace(rate);
                    await _database.Rates.ExecuteAsync(operation);
                }

                var events = rates.SelectMany(r => r.GetEvents());
                return events;
            }

            private async Task<IEnumerable<Rate>> GetRates()
            {
                var tasks = _ratesServices.Select(r => r.GetRates());
                var results = await Task.WhenAll(tasks);

                var rates = results.SelectMany(t => t);
                return rates;
            }
        }
    }
}

using MediatR;
using Microsoft.WindowsAzure.Storage.Table;
using Rates.Core;
using Rates.Core.Events;
using Rates.Core.WriteModel;
using Rates.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rates.Domain.WriteModel
{
    public class FetchRates
    {
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

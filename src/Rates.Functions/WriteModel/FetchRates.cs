using MediatR;
using Microsoft.WindowsAzure.Storage.Table;
using Rates.Functions.Events;
using Rates.Functions.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rates.Functions.WriteModel
{
    public class FetchRates
    {
        public class Command : IRequest<IEnumerable<Event>>
        {
            public IRatesService Service { get; set; }
        }

        public class Handler : IRequestHandler<Command, IEnumerable<Event>>
        {
            private readonly Database _database;

            public Handler(Database database)
            {
                _database = database;
            }

            public async Task<IEnumerable<Event>> Handle(Command request, CancellationToken cancellationToken)
            {
                var rates = await request.Service.GetRates();

                foreach(var rate in rates)
                {
                    var operation = TableOperation.InsertOrReplace(rate);
                    await _database.Rates.ExecuteAsync(operation);
                }

                var events = rates.SelectMany(r => r.GetEvents());
                return events;
            }
        }
    }
}

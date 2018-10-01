using MediatR;
using Microsoft.WindowsAzure.Storage.Table;
using Rates.Core;
using Rates.Core.ReadModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rates.Domain.ReadModel
{
    public class GetRates
    {
        public class Dto
        {
            public List<RateRm> Rates { get; set; }
            public long UpdateTime { get; set; }
        }

        public class Query : IRequest<Dto>
        {

        }

        public class Handler : IRequestHandler<Query, Dto>
        {
            private readonly Database _database;

            public Handler(Database database)
            {
                _database = database;
            }

            public Task<Dto> Handle(Query request, CancellationToken cancellationToken)
            {
                var query = new TableQuery<RateRm>();
                var rates = _database.RatesRm.ExecuteQuery(query);

                var orderedRates = Constants.FiatTickers
                    .Concat(Constants.MetalsTickers)
                    .Concat(Constants.CryptoTickers)
                    .Select(ticker => rates.FirstOrDefault(r => r.Ticker == ticker))
                    .Where(r => r != null)
                    .ToList();

                return Task.FromResult(new Dto
                {
                    Rates = orderedRates,
                    UpdateTime = ((DateTimeOffset)orderedRates.Min(r => r.Timestamp)).ToUnixTimeMilliseconds()
                });
            }
        }
    }
}

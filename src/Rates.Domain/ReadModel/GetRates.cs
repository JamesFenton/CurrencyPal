using MediatR;
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

            public async Task<Dto> Handle(Query request, CancellationToken cancellationToken)
            {                
                var rates = _database.Client.CreateDocumentQuery<RateRm>(_database.RatesRmUri).ToList();

                var orderedRates = Constants.FiatTickers
                    .Concat(Constants.MetalsTickers)
                    .Concat(Constants.CryptoTickers)
                    .Select(ticker => rates.FirstOrDefault(r => r.Ticker == ticker))
                    .Where(r => r != null)
                    .ToList();

                return new Dto
                {
                    Rates = orderedRates,
                    UpdateTime = orderedRates.Min(r => r.Timestamp).ToUnixTimeMilliseconds()
                };
            }
        }
    }
}

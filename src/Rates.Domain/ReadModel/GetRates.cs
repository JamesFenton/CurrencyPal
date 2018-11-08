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
        public class RateDto
        {
            public string Ticker { get; set; }
            public DateTimeOffset Timestamp { get; set; }
            public double Value { get; set; }
            public double? Change1Day { get; set; }
            public double? Change1Week { get; set; }
            public double? Change1Month { get; set; }
            public double? Change3Months { get; set; }
            public double? Change6Months { get; set; }
            public double? Change1Year { get; set; }
            public bool OutOfDate => Timestamp < DateTimeOffset.UtcNow.AddHours(-1);
        }

        public class Response
        {
            public List<RateDto> Rates { get; set; }
            public long UpdateTime { get; set; }
        }

        public class Query : IRequest<Response>
        {
        }

        public class Handler : IRequestHandler<Query, Response>
        {
            private readonly Database _database;

            public Handler(Database database)
            {
                _database = database;
            }

            public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
            {
                TableContinuationToken continuationToken = null;
                var query = new TableQuery<RateRm>();
                var rates = await _database.RatesRm.ExecuteQuerySegmentedAsync(query, continuationToken);

                var orderedRates = Constants.AllTickers
                    .Select(ticker => rates.FirstOrDefault(r => r.Ticker == ticker))
                    .Where(r => r != null)
                    .Select(r => new RateDto
                    {
                        Ticker = r.Ticker,
                        Timestamp = r.Timestamp,
                        Value = r.Value,
                        Change1Day = r.Change1Day,
                        Change1Week = r.Change1Week,
                        Change1Month = r.Change1Month,
                        Change3Months = r.Change3Months,
                        Change6Months = r.Change6Months,
                        Change1Year = r.Change1Year,
                    }).ToList();

                var updateTime = orderedRates.Min(r => r.Timestamp).ToUnixTimeMilliseconds();

                return new Response
                {
                    Rates = orderedRates,
                    UpdateTime = updateTime
                };
            }
        }
    }
}

using MediatR;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net;

namespace Rates.Functions.ReadModel
{
    public class GetRates
    {
        [FunctionName("GetRates")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]HttpRequestMessage req,
            ILogger log)
        {
            var mediator = ContainerFactory.Container.Resolve<IMediator>();

            var query = new Query();
            var response = await mediator.Send(query);

            return req.CreateResponse(HttpStatusCode.OK, response, Constants.JsonFormatter);
        }

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
            public string Name { get; set; }
            public string Href { get; set; }
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

                var orderedRates = Rate.All
                    .Select(rate => (rate: rate, entity: rates.FirstOrDefault(r => r.Ticker == rate.Ticker)))
                    .Where(r => r.entity != null)
                    .Select(r => new RateDto
                    {
                        Ticker = r.entity.Ticker,
                        Timestamp = r.entity.Timestamp,
                        Value = r.entity.Value,
                        Change1Day = r.entity.Change1Day,
                        Change1Week = r.entity.Change1Week,
                        Change1Month = r.entity.Change1Month,
                        Change3Months = r.entity.Change3Months,
                        Change6Months = r.entity.Change6Months,
                        Change1Year = r.entity.Change1Year,
                        Name = r.rate.Name,
                        Href = r.rate.Href,
                    }).ToList();

                var updateTime = orderedRates.Max(r => r.Timestamp).ToUnixTimeMilliseconds();

                return new Response
                {
                    Rates = orderedRates,
                    UpdateTime = updateTime
                };
            }
        }
    }
}

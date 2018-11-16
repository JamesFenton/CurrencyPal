using MediatR;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Rates.Functions.Events;
using Rates.Functions.WriteModel;

namespace Rates.Functions.ReadModel
{
    public class RateAddedHandler : IRequestHandler<RateAdded>
    {
        [FunctionName("RateAddedHandler")]
        public static async Task Run(
            [QueueTrigger(Constants.RatesAddedQueue)]string myQueueItem,
            ILogger log)
        {
            var rateAdded = JsonConvert.DeserializeObject<RateAdded>(myQueueItem, Constants.JsonSettings);
            var mediator = ContainerFactory.Container.Resolve<IMediator>();

            await mediator.Send(rateAdded);
            log.LogInformation($"Rate {rateAdded.Ticker} {rateAdded.TimeKey} handled successfully");
        }

        private readonly Database _database;

        public RateAddedHandler(Database database)
        {
            _database = database;
        }

        public async Task<Unit> Handle(RateAdded request, CancellationToken cancellationToken)
        {
            var e = request;
            var timeAdded = DateTimeOffset.Parse(request.TimeKey);

            var oneDayChange = await GetRateChange(request.Ticker, timeAdded.AddDays(-1), e.Value);
            var oneWeekChange = await GetRateChange(request.Ticker, timeAdded.AddDays(-7), e.Value);
            var oneMonthChange = await GetRateChange(request.Ticker, timeAdded.AddMonths(-1), e.Value);
            var threeMonthChange = await GetRateChange(request.Ticker, timeAdded.AddMonths(-3), e.Value);
            var sixMonthChange = await GetRateChange(request.Ticker, timeAdded.AddMonths(-6), e.Value);
            var oneYearChange = await GetRateChange(request.Ticker, timeAdded.AddYears(-1), e.Value);

            // Update the read model entry
            var updatedRate = new RateRm(
                ticker: e.Ticker,
                value: e.Value,
                change1Day: oneDayChange,
                change1Week: oneWeekChange,
                change1Month: oneMonthChange,
                change3Months: threeMonthChange,
                change6Months: sixMonthChange,
                change1Year: oneYearChange);

            var insertOrReplaceOperation = TableOperation.InsertOrReplace(updatedRate);
            await _database.RatesRm.ExecuteAsync(insertOrReplaceOperation);

            return Unit.Value;
        }

        private async Task<double?> GetRateChange(string ticker, DateTimeOffset time, double rateNow)
        {
            var operation = TableOperation.Retrieve<RateEntity>(ticker, time.ToString("o"));
            var tableResult = await _database.Rates.ExecuteAsync(operation);
            var rate = (RateEntity)tableResult.Result;

            var rateThen = rate?.Value;
            return GetChangePercent(rateNow, rateThen);
        }

        private double? GetChangePercent(double rateNow, double? rateThen)
        {
            if (!rateThen.HasValue)
                return null;
            var percentageChange = (rateNow - rateThen) / rateThen * 100;
            return percentageChange.HasValue ? Math.Round(percentageChange.Value, 2) : (double?)null;
        }
    }
}

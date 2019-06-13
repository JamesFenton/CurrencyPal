using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Rates.Functions.WriteModel;
using System.Linq;

namespace Rates.Functions.ReadModel
{
    public class RateAddedHandler
    {
        private readonly Database _database;

        public RateAddedHandler(Database database)
        {
            _database = database;
        }

        [FunctionName("RateAddedHandler")]
        public async Task Run(
            [QueueTrigger(Constants.RatesAddedQueue)]string myQueueItem,
            ILogger log)
        {
            var rate = JsonConvert.DeserializeObject<RateEntity>(myQueueItem);

            var rateModel = Rate.All.Single(r => r.Ticker == rate.Ticker);
            var timeAdded = DateTimeOffset.Parse(rate.TimeKey);

            // calculate changes
            var oneDayChange = await GetRateChange(rate.Ticker, timeAdded.AddDays(-1), rate.Value);
            var oneWeekChange = await GetRateChange(rate.Ticker, timeAdded.AddDays(-7), rate.Value);
            var oneMonthChange = await GetRateChange(rate.Ticker, timeAdded.AddMonths(-1), rate.Value);
            var threeMonthChange = await GetRateChange(rate.Ticker, timeAdded.AddMonths(-3), rate.Value);
            var sixMonthChange = await GetRateChange(rate.Ticker, timeAdded.AddMonths(-6), rate.Value);
            var oneYearChange = await GetRateChange(rate.Ticker, timeAdded.AddYears(-1), rate.Value);

            // Update the read model entry
            var updatedRate = new RateRm(
                ticker: rate.Ticker,
                name: rateModel.Name,
                href: rateModel.Href,
                value: rate.Value,
                change1Day: oneDayChange,
                change1Week: oneWeekChange,
                change1Month: oneMonthChange,
                change3Months: threeMonthChange,
                change6Months: sixMonthChange,
                change1Year: oneYearChange);

            // update read model entry
            var insertOrReplaceOperation = TableOperation.InsertOrReplace(updatedRate);
            await _database.RatesRm.ExecuteAsync(insertOrReplaceOperation);

            log.LogInformation($"Rate {rate.Ticker} {rate.TimeKey} handled successfully");
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

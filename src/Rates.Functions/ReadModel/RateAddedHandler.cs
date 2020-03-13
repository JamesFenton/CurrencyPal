using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Rates.Functions.WriteModel;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace Rates.Functions.ReadModel
{
    public class RateAddedHandler
    {
        private readonly Database _database;
        private readonly JsonSerializer _jsonSerializer;

        public RateAddedHandler(Database database, JsonSerializer jsonSerializer)
        {
            _database = database;
            _jsonSerializer = jsonSerializer;
        }

        [FunctionName("RateAddedHandler")]
        public async Task Run(
            [QueueTrigger(Constants.RatesAddedQueue)] string myQueueItem,
            [Blob("lookups/rates.json", FileAccess.Read)] string rateLookupsJson,
            [Blob("read-model/rates.json", FileAccess.Write)] TextWriter ratesBlob,
            ILogger log)
        {
            var rates = JsonConvert.DeserializeObject<List<RateEntity>>(myQueueItem);
            var rateDefinitions = JsonConvert.DeserializeObject<List<Rate>>(rateLookupsJson);

            log.LogInformation($"Handling {rates.Count} new rates");

            // create read model entities
            var readModelEntities = await Task.WhenAll(
                rates.Select(r => (rate: r, definition: rateDefinitions.Single(l => l.Ticker == r.Ticker)))
                     .Select(x => GetReadModelEntity(x.rate, x.definition))
            );

            log.LogInformation("Created read model entities");

            // order and create DTO
            var orderedRates = rateDefinitions
                .Select(rate => readModelEntities.FirstOrDefault(r => r.Ticker == rate.Ticker))
                .Where(r => r != null)
                .ToList();

            var updateTime = orderedRates.Max(r => r.Timestamp).ToUnixTimeMilliseconds();

            var dto = new GetRatesDto
            {
                Rates = orderedRates,
                UpdateTime = updateTime,
            };

            // save dto to storage
            _jsonSerializer.Serialize(ratesBlob, dto);
        }

        private async Task<RateRm> GetReadModelEntity(RateEntity rate, Rate rateDefinition)
        {
            var timeAdded = DateTimeOffset.Parse(rate.TimeKey);

            // calculate changes
            var oneDayChange = await GetRateChange(rate.Ticker, timeAdded.AddDays(-1), rate.Value);
            var oneWeekChange = await GetRateChange(rate.Ticker, timeAdded.AddDays(-7), rate.Value);
            var oneMonthChange = await GetRateChange(rate.Ticker, timeAdded.AddMonths(-1), rate.Value);
            var threeMonthChange = await GetRateChange(rate.Ticker, timeAdded.AddMonths(-3), rate.Value);
            var sixMonthChange = await GetRateChange(rate.Ticker, timeAdded.AddMonths(-6), rate.Value);
            var oneYearChange = await GetRateChange(rate.Ticker, timeAdded.AddYears(-1), rate.Value);

            // return the read model entry
            return new RateRm(
                ticker: rate.Ticker,
                name: rateDefinition.Name,
                href: rateDefinition.Href,
                value: rate.Value,
                change1Day: oneDayChange,
                change1Week: oneWeekChange,
                change1Month: oneMonthChange,
                change3Months: threeMonthChange,
                change6Months: sixMonthChange,
                change1Year: oneYearChange
            );
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

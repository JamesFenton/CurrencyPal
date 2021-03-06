﻿using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Rates.Functions.Models;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

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
            [QueueTrigger(Constants.RatesAddedQueue)] RateEntity rate,
            [Blob("lookups/rates.json", FileAccess.Read)] string rateDefinitionsJson,
            [Table(Database.RatesRmTable)] CloudTable readModelTable,
            ILogger logger
        )
        {
            logger.LogInformation($"Handling rate {rate.Ticker}");

            var rateDefinitions = JsonConvert.DeserializeObject<List<Rate>>(rateDefinitionsJson);
            var definition = rateDefinitions.Single(l => l.Ticker == rate.Ticker);
            var index = rateDefinitions.IndexOf(definition);

            var readModelEntity = await GetReadModelEntity(rate, definition, index);

            await SaveEntity(readModelTable, readModelEntity);
        }

        private async Task<RateRm> GetReadModelEntity(RateEntity rate, Rate rateDefinition, int order)
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
                order: order,
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

        private async Task SaveEntity(CloudTable table, RateRm entity)
        {
            var operation = TableOperation.InsertOrReplace(entity);
            await table.ExecuteAsync(operation);
        }
    }
}

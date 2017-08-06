using MongoDB.Driver;
using Rates.Core;
using Rates.Core.Events;
using Rates.Core.ReadModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rates.Fetcher
{
    public class RateAddedHandler
    {
        private readonly Database _database;

        public RateAddedHandler(Database database)
        {
            _database = database;
        }

        public void Handle(RateAdded e)
        {
            var ticker = e.Ticker;

            // add 2 minutes so we don't get a rate from 23 hours ago
            // since the rate might be saved a few seconds after the hour
            var now = DateTime.UtcNow.AddMinutes(2);

            // Get historical rates
            var rate1DayAgo = _database.Rates
                .AsQueryable()
                .Where(r => r.Ticker == ticker && now.AddDays(-1).AddHours(-1) <= r.Timestamp && r.Timestamp <= now.AddDays(-1))
                .OrderByDescending(r => r.Timestamp)
                .FirstOrDefault();
            var oneDayChange = GetChange(e.Value, rate1DayAgo?.Value);

            var rate1WeekAgo = _database.Rates
                .AsQueryable()
                .Where(r => r.Ticker == ticker && now.AddDays(-7).AddHours(-1) <= r.Timestamp && r.Timestamp <= now.AddDays(-7))
                .OrderByDescending(r => r.Timestamp)
                .FirstOrDefault();
            var oneWeekChange = GetChange(e.Value, rate1WeekAgo?.Value);

            var rate1MonthAgo = _database.Rates
                .AsQueryable()
                .Where(r => r.Ticker == ticker && now.AddMonths(-1).AddHours(-1) <= r.Timestamp && r.Timestamp <= now.AddMonths(-1))
                .OrderByDescending(r => r.Timestamp)
                .FirstOrDefault();
            var oneMonthChange = GetChange(e.Value, rate1MonthAgo?.Value);

            var rate1YearAgo = _database.Rates
                .AsQueryable()
                .Where(r => r.Ticker == ticker && now.AddYears(-1).AddHours(-1) <= r.Timestamp && r.Timestamp <= now.AddYears(-1))
                .OrderByDescending(r => r.Timestamp)
                .FirstOrDefault();
            var oneYearChange = GetChange(e.Value, rate1YearAgo?.Value);

            // fetch existing rate and create an updated version
            var existing = _database.RatesRm
                .AsQueryable()
                .FirstOrDefault(r => r.Ticker == e.Ticker);

            var updatedRate = new RateRm(
                    existing?.Id ?? Guid.NewGuid(),
                    e.Ticker,
                    e.Timestamp,
                    e.Value,
                    oneDayChange,
                    oneWeekChange,
                    oneMonthChange,
                    oneYearChange);

            if (existing == null)
            {
                _database.RatesRm.InsertOne(updatedRate);
            }
            else
            {
                _database.RatesRm.ReplaceOne(
                    Builders<RateRm>.Filter.Eq(r => r.Id, existing.Id),
                    updatedRate);
            }
        }

        private double? GetChange(double rateNow, double? rateThen)
        {
            if (!rateThen.HasValue)
                return null;
            return (rateNow - rateThen) / rateThen;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rates.Web.Dto;
using Rates.Core;
using MongoDB.Driver;

namespace Rates.Web.Services
{
    public class DatabaseRatesService : IRatesService
    {
        private readonly Database _database;

        public DatabaseRatesService(Database database)
        {
            _database = database;
        }

        public RatesDto GetRates()
        {
            var rates = Constants.FiatTickers
                .Concat(Constants.CryptoTickers)
                .Select(ticker => GetMostRecentRate(ticker))
                .ToList();

            return new RatesDto
            {
                Rates = rates,
                UpdateTime = rates.Min(r => r.Timestamp).ToUnixTimeMilliseconds()
            };
        }

        private RateDto GetMostRecentRate(string ticker)
        {
            // add 2 minutes so we don't get a rate from 23 hours ago
            // since the rate might be saved a few seconds after the hour
            var now = DateTime.UtcNow.AddMinutes(2);

            var rate = _database.Rates
                .AsQueryable()
                .Where(r => r.Ticker == ticker)
                .OrderByDescending(r => r.Timestamp)
                .FirstOrDefault();

            var rate1DayAgo = _database.Rates
                .AsQueryable()
                .Where(r => r.Ticker == ticker && r.Timestamp <= now.AddDays(-1))
                .OrderByDescending(r => r.Timestamp)
                .FirstOrDefault();

            var rate1WeekAgo = _database.Rates
                .AsQueryable()
                .Where(r => r.Ticker == ticker && r.Timestamp <= now.AddDays(-7))
                .OrderByDescending(r => r.Timestamp)
                .FirstOrDefault();

            var rate1MonthAgo = _database.Rates
                .AsQueryable()
                .Where(r => r.Ticker == ticker && r.Timestamp <= now.AddMonths(-1))
                .OrderByDescending(r => r.Timestamp)
                .FirstOrDefault();

            return new RateDto
            {
                Ticker = rate.Ticker,
                Timestamp = rate.Timestamp,
                Rate = rate.Value,
                Change1Day = GetChange(rate.Value, rate1DayAgo?.Value),
                Change1Week = GetChange(rate.Value, rate1WeekAgo?.Value),
                Change1Month = GetChange(rate.Value, rate1MonthAgo?.Value),
            };
        }

        private double? GetChange(double rateNow, double? rateThen)
        {
            if (!rateThen.HasValue)
                return null;
            return (rateNow - rateThen) / rateThen;
        }
    }
}

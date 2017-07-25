using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rates.Web.Dto;
using Rates.Core;
using MongoDB.Driver;

namespace Rates.Web.Services
{
    public class DatabaseRatesService
    {
        private readonly Database _database;

        public DatabaseRatesService(Database database)
        {
            _database = database;
        }

        public List<RateDto> GetRates()
        {
            var rates = Constants.FiatTickers
                .Concat(Constants.CryptoTickers)
                .Select(ticker => GetMostRecentRate(ticker))
                .ToList();
            return rates;
        }

        private RateDto GetMostRecentRate(string ticker)
        {
            var rate = _database.Rates
                .AsQueryable()
                .Where(r => r.Ticker == ticker)
                .OrderByDescending(r => r.Timestamp)
                .FirstOrDefault();

            var rate1DayAgo = _database.Rates
                .AsQueryable()
                .Where(r => r.Ticker == ticker && r.Timestamp <= DateTime.UtcNow.AddDays(-1))
                .OrderByDescending(r => r.Timestamp)
                .FirstOrDefault();

            var rate1WeekAgo = _database.Rates
                .AsQueryable()
                .Where(r => r.Ticker == ticker && r.Timestamp <= DateTime.UtcNow.AddDays(-7))
                .OrderByDescending(r => r.Timestamp)
                .FirstOrDefault();

            var rate1MonthAgo = _database.Rates
                .AsQueryable()
                .Where(r => r.Ticker == ticker && r.Timestamp <= DateTime.UtcNow.AddMonths(-1))
                .OrderByDescending(r => r.Timestamp)
                .FirstOrDefault();

            return new RateDto
            {
                Ticker = rate.Ticker,
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

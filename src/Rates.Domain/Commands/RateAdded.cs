using MediatR;
using MongoDB.Driver;
using Rates.Core;
using Rates.Core.Events;
using Rates.Core.ReadModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rates.Domain.Commands
{
    public class RateAdded
    {
        public class Command : IRequest
        {
            public Guid Id { get; set; }
            public string Ticker { get; set; }
            public DateTime Timestamp { get; set; }
            public double Value { get; set; }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly Database _database;

            public Handler(Database database)
            {
                _database = database;
            }

            public Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var e = request;
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
                var oneDayChange = GetChangePercent(e.Value, rate1DayAgo?.Value);

                var rate1WeekAgo = _database.Rates
                    .AsQueryable()
                    .Where(r => r.Ticker == ticker && now.AddDays(-7).AddHours(-1) <= r.Timestamp && r.Timestamp <= now.AddDays(-7))
                    .OrderByDescending(r => r.Timestamp)
                    .FirstOrDefault();
                var oneWeekChange = GetChangePercent(e.Value, rate1WeekAgo?.Value);

                var rate1MonthAgo = _database.Rates
                    .AsQueryable()
                    .Where(r => r.Ticker == ticker && now.AddMonths(-1).AddHours(-1) <= r.Timestamp && r.Timestamp <= now.AddMonths(-1))
                    .OrderByDescending(r => r.Timestamp)
                    .FirstOrDefault();
                var oneMonthChange = GetChangePercent(e.Value, rate1MonthAgo?.Value);

                var rate3MonthsAgo = _database.Rates
                    .AsQueryable()
                    .Where(r => r.Ticker == ticker && now.AddMonths(-3).AddHours(-1) <= r.Timestamp && r.Timestamp <= now.AddMonths(-3))
                    .OrderByDescending(r => r.Timestamp)
                    .FirstOrDefault();
                var threeMonthChange = GetChangePercent(e.Value, rate3MonthsAgo?.Value);

                var rate6MonthsAgo = _database.Rates
                    .AsQueryable()
                    .Where(r => r.Ticker == ticker && now.AddMonths(-6).AddHours(-1) <= r.Timestamp && r.Timestamp <= now.AddMonths(-6))
                    .OrderByDescending(r => r.Timestamp)
                    .FirstOrDefault();
                var sixMonthChange = GetChangePercent(e.Value, rate6MonthsAgo?.Value);

                var rate1YearAgo = _database.Rates
                    .AsQueryable()
                    .Where(r => r.Ticker == ticker && now.AddYears(-1).AddHours(-1) <= r.Timestamp && r.Timestamp <= now.AddYears(-1))
                    .OrderByDescending(r => r.Timestamp)
                    .FirstOrDefault();
                var oneYearChange = GetChangePercent(e.Value, rate1YearAgo?.Value);

                // fetch existing rate and create an updated version
                var existing = _database.RatesRm
                    .AsQueryable()
                    .FirstOrDefault(r => r.Ticker == e.Ticker);

                var updatedRate = new RateRm(
                        id: existing?.Id ?? Guid.NewGuid(),
                        ticker: e.Ticker,
                        timestamp: e.Timestamp,
                        value: e.Value,
                        change1Day: oneDayChange,
                        change1Week: oneWeekChange,
                        change1Month: oneMonthChange,
                        change3Months: threeMonthChange,
                        change6Months: sixMonthChange,
                        change1Year: oneYearChange);

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

                return Unit.Task;
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
}

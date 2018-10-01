using MediatR;
using Microsoft.WindowsAzure.Storage.Table;
using Rates.Core;
using Rates.Core.ReadModel;
using Rates.Core.WriteModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rates.Domain.WriteModel
{
    public class RateAdded
    {
        public class Handler : IRequestHandler<Core.Events.RateAdded>
        {
            private readonly Database _database;

            public Handler(Database database)
            {
                _database = database;
            }

            public Task<Unit> Handle(Core.Events.RateAdded request, CancellationToken cancellationToken)
            {
                var e = request;
                var timeAdded = DateTimeOffset.Parse(request.TimeKey);

                // Get historical rates
                var oneDayAgo = TableOperation.Retrieve<Rate>(request.Ticker, timeAdded.AddDays(-1).ToString("o"));
                var rate1DayAgo = (Rate)_database.Rates.Execute(oneDayAgo).Result;
                var oneDayChange = GetChangePercent(e.Value, rate1DayAgo?.Value);

                var oneWeekAgo = TableOperation.Retrieve<Rate>(request.Ticker, timeAdded.AddDays(-7).ToString("o"));
                var rate1WeekAgo = (Rate)_database.Rates.Execute(oneWeekAgo).Result;
                var oneWeekChange = GetChangePercent(e.Value, rate1WeekAgo?.Value);

                var oneMonthAgo = TableOperation.Retrieve<Rate>(request.Ticker, timeAdded.AddMonths(-1).ToString("o"));
                var rate1MonthAgo = (Rate)_database.Rates.Execute(oneMonthAgo).Result;
                var oneMonthChange = GetChangePercent(e.Value, rate1MonthAgo?.Value);

                var threeMonthsAgo = TableOperation.Retrieve<Rate>(request.Ticker, timeAdded.AddMonths(-3).ToString("o"));
                var rate3MonthsAgo = (Rate)_database.Rates.Execute(threeMonthsAgo).Result;
                var threeMonthChange = GetChangePercent(e.Value, rate3MonthsAgo?.Value);

                var sixMonthsAgo = TableOperation.Retrieve<Rate>(request.Ticker, timeAdded.AddMonths(-6).ToString("o"));
                var rate6MonthsAgo = (Rate)_database.Rates.Execute(sixMonthsAgo).Result;
                var sixMonthChange = GetChangePercent(e.Value, rate6MonthsAgo?.Value);

                var oneYearAgo = TableOperation.Retrieve<Rate>(request.Ticker, timeAdded.AddYears(-1).ToString("o"));
                var rate1YearAgo = (Rate)_database.Rates.Execute(oneYearAgo).Result;
                var oneYearChange = GetChangePercent(e.Value, rate1YearAgo?.Value);

                // fetch existing rate and create an updated version
                var fetchExistingOperation = TableOperation.Retrieve<RateRm>(RateRm.PartitionKeyLabel, request.Ticker);
                var existing = (RateRm)_database.RatesRm.Execute(fetchExistingOperation).Result;


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
                _database.RatesRm.Execute(insertOrReplaceOperation);

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

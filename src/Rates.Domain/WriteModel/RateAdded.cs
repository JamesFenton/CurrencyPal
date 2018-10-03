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

            public async Task<Unit> Handle(Core.Events.RateAdded request, CancellationToken cancellationToken)
            {
                var e = request;
                var timeAdded = DateTimeOffset.Parse(request.TimeKey);

                // Get historical rates table operations
                var oneDayAgo = TableOperation.Retrieve<Rate>(request.Ticker, timeAdded.AddDays(-1).ToString("o"));
                var oneWeekAgo = TableOperation.Retrieve<Rate>(request.Ticker, timeAdded.AddDays(-7).ToString("o"));
                var oneMonthAgo = TableOperation.Retrieve<Rate>(request.Ticker, timeAdded.AddMonths(-1).ToString("o"));
                var threeMonthsAgo = TableOperation.Retrieve<Rate>(request.Ticker, timeAdded.AddMonths(-3).ToString("o"));
                var sixMonthsAgo = TableOperation.Retrieve<Rate>(request.Ticker, timeAdded.AddMonths(-6).ToString("o"));
                var oneYearAgo = TableOperation.Retrieve<Rate>(request.Ticker, timeAdded.AddYears(-1).ToString("o"));

                // execute all the operations in parallel
                var tasks = new[]
                {
                    oneDayAgo,
                    oneWeekAgo,
                    oneMonthAgo,
                    threeMonthsAgo,
                    sixMonthsAgo,
                    oneMonthAgo,
                }
                .Select(operation => _database.Rates.ExecuteAsync(operation));

                // wait for all operations to complete
                await Task.WhenAll(tasks);

                // get the rates from the operation results
                var rates = tasks.Select(t => t.Result)
                    .Select(tableResult => (Rate)tableResult.Result)
                    .ToArray();

                var oneDayChange = GetChangePercent(e.Value, rates[0]?.Value);
                var oneWeekChange = GetChangePercent(e.Value, rates[1]?.Value);
                var oneMonthChange = GetChangePercent(e.Value, rates[2]?.Value);
                var threeMonthChange = GetChangePercent(e.Value, rates[3]?.Value);
                var sixMonthChange = GetChangePercent(e.Value, rates[4]?.Value);
                var oneYearChange = GetChangePercent(e.Value, rates[5]?.Value);

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

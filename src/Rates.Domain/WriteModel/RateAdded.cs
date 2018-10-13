using MediatR;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Rates.Core;
using Rates.Core.ReadModel;
using Rates.Core.WriteModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Database = Rates.Core.Database;

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
                
                await _database.Client.UpsertDocumentAsync(_database.RatesRmUri, updatedRate);

                return Unit.Value;
            }

            private async Task<double?> GetRateChange(string ticker, DateTimeOffset time, double rateNow)
            {
                Rate rate = null;
                try
                {
                    var id = Rate.CreateId(ticker, time.ToString("o"));
                    var uri = UriFactory.CreateDocumentUri(_database.DatabaseName, _database.RatesCollection, id);
                    var response = await _database.Client.ReadDocumentAsync<Rate>(uri);
                    rate = response.Document;
                }
                catch (DocumentClientException e)
                {
                    if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                        return null;
                    else
                        throw;
                }

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
}

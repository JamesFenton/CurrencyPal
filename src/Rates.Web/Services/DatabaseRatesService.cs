using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rates.Web.Dto;
using Rates.Core;
using MongoDB.Driver;
using Rates.Core.ReadModel;

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
            var rates = _database.RatesRm.AsQueryable().ToList();

            var orderedRates = Constants.FiatTickers
                .Concat(Constants.CryptoTickers)
                .Select(ticker => rates.FirstOrDefault(r => r.Ticker == ticker))
                .Where(r => r != null)
                .ToList();
            
            return new RatesDto
            {
                Rates = rates,
                UpdateTime = ((DateTimeOffset)rates.Min(r => r.Timestamp)).ToUnixTimeMilliseconds()
            };
        }
    }
}

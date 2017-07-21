using Quartz;
using Rates.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Rates.Fetcher
{
    class RateFetcherJob : IJob
    {
        private readonly RatesService _ratesService;
        private readonly Database _database;

        public RateFetcherJob(Database database, RatesService ratesService)
        {
            _database = database;
            _ratesService = ratesService;
        }

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                var rates = _ratesService.GetRates().Result;
                _database.Rates.InsertMany(rates);
            }
            catch (Exception) { }
        }
    }
}

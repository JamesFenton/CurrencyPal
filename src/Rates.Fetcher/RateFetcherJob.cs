using Quartz;
using Rates.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rates.Fetcher
{
    class RateFetcherJob : IJob
    {
        private readonly RatesFetcher _ratesService;
        private readonly Database _database;
        private readonly Mediator _mediator;

        public RateFetcherJob(
            Database database, 
            RatesFetcher ratesService,
            Mediator mediator)
        {
            _database = database;
            _ratesService = ratesService;
            _mediator = mediator;
        }

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                var rates = _ratesService.GetRates().Result;
                _database.Rates.InsertMany(rates);

                rates.SelectMany(r => r.GetEvents())
                    .ToList()
                    .ForEach(e => _mediator.Send(e));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}

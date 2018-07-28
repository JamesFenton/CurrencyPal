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

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var rates = await _ratesService.GetRates();
                _database.Rates.InsertMany(rates);

                var tasks = rates.SelectMany(r => r.GetEvents())
                                 .Select(e => _mediator.Send(e));
                await Task.WhenAll(tasks);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}

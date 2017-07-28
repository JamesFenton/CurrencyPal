using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using Rates.Core;

namespace Rates.Fetcher
{
    public class JobFactory : IJobFactory
    {
        private readonly RatesFetcher _ratesService;
        private readonly Database _database;
        private readonly Mediator _mediator;

        public JobFactory(RatesFetcher ratesService, Database database, Mediator mediator)
        {
            _ratesService = ratesService;
            _database = database;
            _mediator = mediator;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            var jobDetail = bundle.JobDetail;
            Type jobType = jobDetail.JobType;

            if (jobType == typeof(RateFetcherJob))
            {
                var job = new RateFetcherJob(_database, _ratesService, _mediator);
                return job;
            }
            else
            {
                throw new Exception($"Job type {jobType.Name} is not supported");
            }
        }

        public void ReturnJob(IJob job)
        {
            
        }
    }
}

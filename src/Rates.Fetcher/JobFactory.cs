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
        private readonly RatesService _ratesService;
        private readonly Database _database;

        public JobFactory(RatesService ratesService, Database database)
        {
            _ratesService = ratesService;
            _database = database;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            var jobDetail = bundle.JobDetail;
            Type jobType = jobDetail.JobType;

            if (jobType == typeof(RateFetcherJob))
            {
                var job = new RateFetcherJob(_database, _ratesService);
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

using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Linq;
using Quartz;
using Autofac;

namespace Rates.Fetcher
{
    public class JobFactory : IJobFactory
    {
        private readonly IContainer _container;

        public JobFactory(IContainer container)
        {
            _container = container;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            var jobDetail = bundle.JobDetail;
            Type jobType = jobDetail.JobType;

            using(var scope = _container.BeginLifetimeScope())
            {
                var job = (IJob)_container.Resolve(jobType);
                return job;
            }
        }

        public void ReturnJob(IJob job)
        {
            
        }
    }
}

using MongoDB.Driver;
using Quartz;
using Quartz.Impl;
using Rates.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Rates.Fetcher
{
    public class Service : ServiceBase
    {
        private readonly bool _developerMode;
        private readonly IScheduler _scheduler;

        public Service(bool developerMode)
        {
            _developerMode = developerMode;

            // Grab the Scheduler instance from the Factory 
            _scheduler = StdSchedulerFactory.GetDefaultScheduler();

            var database = new Database(new MongoClient(Constants.ConnectionString), Constants.Database);
            var ratesService = new RatesService(Constants.OpenExchangeRatesAppId, new HttpClient());
            _scheduler.JobFactory = new JobFactory(ratesService, database);
        }

        public void Start()
        {
            OnStart(new string[0]);
        }

        protected override void OnStart(string[] args)
        {
            // and start it off
            _scheduler.Start();

            // define the job and tie it to our HelloJob class
            IJobDetail job = JobBuilder.Create<RateFetcherJob>()
                .WithIdentity("job1", "group1")
                .Build();

            // Trigger the job
            ITrigger trigger;
            if (_developerMode)
            {
                trigger = TriggerBuilder.Create()
                .WithIdentity("trigger1", "group1")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInMinutes(1)
                    .RepeatForever())
                .Build();
            }
            else
            {
                trigger = TriggerBuilder.Create()
                .WithIdentity("trigger1", "group1")
                .StartAt(DateBuilder.EvenHourDateAfterNow())
                .WithSimpleSchedule(x => x
                    .WithIntervalInHours(1)
                    .RepeatForever())
                .Build();
            }

            // Tell quartz to schedule the job using our trigger
            _scheduler.ScheduleJob(job, trigger);
        }

        protected override void OnStop()
        {
            _scheduler.Shutdown();
        }
    }
}

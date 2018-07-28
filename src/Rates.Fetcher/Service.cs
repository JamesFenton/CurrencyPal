using Autofac;
using MongoDB.Driver;
using Quartz;
using Quartz.Impl;
using Rates.Core;
using Rates.Fetcher.Services;
using System.Net.Http;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace Rates.Fetcher
{
    public class Service : ServiceBase
    {
        private readonly bool _developerMode;
        private IScheduler _scheduler;
        private Task _task;

        public Service(bool developerMode)
        {
            _developerMode = developerMode;
        }

        public void Start()
        {
            OnStart(new string[0]);
        }

        protected override void OnStart(string[] args)
        {
            _task = Task.Run(async () =>
            {
                _scheduler = await GetScheduler(_developerMode);
                // and start it off
                await _scheduler.Start();

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
                await _scheduler.ScheduleJob(job, trigger);
            });
        }

        protected override void OnStop()
        {
            _scheduler.Shutdown();
        }
        
        private static async Task<IScheduler> GetScheduler(bool developerMode)
        {
            // Grab the Scheduler instance from the Factory 
            var scheduler = await StdSchedulerFactory.GetDefaultScheduler();

            var builder = new ContainerBuilder();

            builder.RegisterInstance(new MongoClient(Constants.ConnectionString));
            builder.Register(c => new Database(c.Resolve<MongoClient>(), Constants.Database))
                   .SingleInstance();
            builder.RegisterType<HttpClient>()
                   .SingleInstance();
            builder.RegisterType<CoinMarketCapService>()
                   .InstancePerLifetimeScope();
            builder.Register(c => new OpenExchangeRatesService(Constants.OpenExchangeRatesAppId, c.Resolve<HttpClient>()))
                   .InstancePerLifetimeScope();
            builder.RegisterType<RatesFetcher>()
                   .InstancePerLifetimeScope();
            builder.RegisterType<RateAddedHandler>()
                   .AsImplementedInterfaces()
                   .InstancePerLifetimeScope();
            builder.RegisterType<Mediator>()
                   .InstancePerLifetimeScope();

            builder.RegisterType<RateFetcherJob>()
                   .InstancePerLifetimeScope();

            var container = builder.Build();

            scheduler.JobFactory = new JobFactory(container);
            return scheduler;
        }
    }
}

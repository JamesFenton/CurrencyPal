using Autofac;
using MediatR;
using Polly;
using Polly.Retry;
using Rates.Core;
using Rates.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rates.Domain
{
    public static class ContainerBuilderExtensions
    {
        public static ContainerBuilder AddMediator(this ContainerBuilder builder)
        {
            builder
              .RegisterType<Mediator>()
              .As<IMediator>()
              .InstancePerLifetimeScope();

            builder.Register<ServiceFactory>(ctx =>
            {
                var c = ctx.Resolve<IComponentContext>();
                return t => c.TryResolve(t, out var o) ? o : null;
            }).InstancePerLifetimeScope();

            return builder;
        }

        public static ContainerBuilder AddFetcher(this ContainerBuilder builder, Settings settings)
        {
            builder.RegisterInstance(settings);

            builder.RegisterType<Database>().SingleInstance();

            builder.RegisterType<CoinMarketCapService>().SingleInstance();
            builder.RegisterType<FinancialModellingPrepService>().SingleInstance();
            builder.RegisterType<OpenExchangeRatesService>().SingleInstance();

            // handlers
            builder.RegisterAssemblyTypes(typeof(ContainerBuilderExtensions).Assembly)
                   .AsClosedTypesOf(typeof(IRequestHandler<,>))
                   .AsImplementedInterfaces();

            return builder;
        }

        public static ContainerBuilder AddRetryPolicy(this ContainerBuilder builder)
        {
            builder.Register(c => Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(4),
            }))
            .As<RetryPolicy>()
            .InstancePerDependency();

            return builder;
        }
    }
}

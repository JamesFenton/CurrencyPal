using Autofac;
using MediatR;
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

        public static ContainerBuilder AddFetcher(this ContainerBuilder builder, 
            string databaseConnectionString,
            string coinMarketCapApiKey,
            string openExchangeRatesAppId)
        {
            builder.Register(c => new Database(databaseConnectionString))
                   .SingleInstance();
            builder.Register(c => new CoinMarketCapService(coinMarketCapApiKey))
                   .SingleInstance();
            builder.RegisterType<FinancialModellingPrepService>()
                   .SingleInstance();
            builder.Register(c => new OpenExchangeRatesService(openExchangeRatesAppId))
                   .SingleInstance();

            // handlers
            builder.RegisterAssemblyTypes(typeof(ContainerBuilderExtensions).Assembly)
                   .AsClosedTypesOf(typeof(IRequestHandler<,>))
                   .AsImplementedInterfaces();

            return builder;
        }
    }
}

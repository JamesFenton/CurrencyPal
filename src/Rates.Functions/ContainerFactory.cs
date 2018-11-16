using Autofac;
using MediatR;
using Rates.Functions.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rates.Functions
{
    static class ContainerFactory
    {
        private static IContainer _container;

        internal static IContainer Container => _container ?? (_container = GetContainer());

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

            // services
            builder.RegisterType<CoinMarketCapService>().Named<IRatesService>("cmc").SingleInstance();
            builder.RegisterDecorator<IRatesService>(
                (c, inner) => new ErrorHandlingRatesService(inner),
                fromKey: "cmc",
                toKey: "cryptos");

            builder.RegisterType<FinancialModellingPrepService>().Named<IRatesService>("fmp").SingleInstance();
            builder.RegisterDecorator<IRatesService>(
                (c, inner) => new ErrorHandlingRatesService(inner),
                fromKey: "fmp",
                toKey: "stocks");

            builder.RegisterType<OpenExchangeRatesService>().Named<IRatesService>("oer").SingleInstance();
            builder.RegisterDecorator<IRatesService>(
                (c, inner) => new ErrorHandlingRatesService(inner),
                fromKey: "oer",
                toKey: "forex");

            // handlers
            builder.RegisterAssemblyTypes(typeof(ContainerFactory).Assembly)
                   .AsClosedTypesOf(typeof(IRequestHandler<,>))
                   .AsImplementedInterfaces();

            return builder;
        }

        private static IContainer GetContainer()
        {
            var settings = new Settings
            {
                DatabaseConnectionString = Environment.GetEnvironmentVariable("RATES_DB_CONNECTIONSTRING"),
                CoinMarketCapApiKey = Environment.GetEnvironmentVariable("CMC_API_KEY"),
                OpenExchangeRatesApiKey = Environment.GetEnvironmentVariable("OPENEXCHANGERATES_APPID"),
            };

            var container = new ContainerBuilder()
                .AddMediator()
                .AddFetcher(settings)
                .Build();

            return container;
        }
    }
}

using Autofac;
using MediatR;
using MongoDB.Driver;
using Rates.Core;
using Rates.Fetcher.Commands;
using Rates.Fetcher.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Rates.Fetcher
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

        public static ContainerBuilder AddFetcher(this ContainerBuilder builder, string databaseConnectionString, string databaseName)
        {
            builder.RegisterInstance(new MongoClient(databaseConnectionString));
            builder.Register(c => new Database(c.Resolve<MongoClient>(), databaseName))
                   .SingleInstance();
            builder.RegisterType<HttpClient>()
                   .SingleInstance();
            builder.RegisterType<CoinMarketCapService>()
                   .InstancePerLifetimeScope();
            builder.Register(c => new OpenExchangeRatesService(Constants.OpenExchangeRatesAppId, c.Resolve<HttpClient>()))
                   .InstancePerLifetimeScope();

            // handlers
            builder.RegisterAssemblyTypes(typeof(ContainerBuilderExtensions).Assembly)
                   .AsClosedTypesOf(typeof(IRequestHandler<,>))
                   .AsImplementedInterfaces();

            return builder;
        }
    }
}

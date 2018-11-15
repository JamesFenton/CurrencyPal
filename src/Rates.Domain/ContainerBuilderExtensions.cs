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

            // services
            builder.RegisterType<CoinMarketCapService>().SingleInstance().Named<IRatesService>("service");
            builder.RegisterType<FinancialModellingPrepService>().SingleInstance().Named<IRatesService>("service");
            builder.RegisterType<OpenExchangeRatesService>().SingleInstance().Named<IRatesService>("service");

            builder.RegisterDecorator<IRatesService>((c, inner) => new ErrorHandlingRatesService(inner), fromKey: "service");

            // handlers
            builder.RegisterAssemblyTypes(typeof(ContainerBuilderExtensions).Assembly)
                   .AsClosedTypesOf(typeof(IRequestHandler<,>))
                   .AsImplementedInterfaces();

            return builder;
        }
    }
}

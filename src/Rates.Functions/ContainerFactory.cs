﻿using Autofac;
using Rates.Core;
using Rates.Domain;
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

        private static IContainer GetContainer()
        {
            var container = new ContainerBuilder()
                .AddMediator()
                .AddFetcher(Constants.DatabaseConnectionString, Constants.DatabaseAccessKey)
                .Build();

            var database = container.Resolve<Database>();
            database.Initialise().GetAwaiter().GetResult();

            return container;
        }
    }
}

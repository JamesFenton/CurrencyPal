﻿using MediatR;
using Microsoft.WindowsAzure.Storage.Table;
using Rates.Core;
using Rates.Core.Events;
using Rates.Core.WriteModel;
using Rates.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rates.Domain.WriteModel
{
    public class FetchRates
    {
        public class Command : IRequest<IEnumerable<Event>>
        {
        }

        public class Handler : IRequestHandler<Command, IEnumerable<Event>>
        {
            private readonly OpenExchangeRatesService _openExchangeRatesService;
            private readonly CoinMarketCapService _coinMarketCapService;
            private readonly Database _database;

            public Handler(
                OpenExchangeRatesService openExchangeRatesService,
                CoinMarketCapService coinMarketCapService,
                Database database)
            {
                _openExchangeRatesService = openExchangeRatesService;
                _coinMarketCapService = coinMarketCapService;
                _database = database;
            }

            public async Task<IEnumerable<Event>> Handle(Command request, CancellationToken cancellationToken)
            {
                var rates = await GetRates();

                foreach(var rate in rates)
                {
                    var operation = TableOperation.InsertOrReplace(rate);
                    await _database.Rates.ExecuteAsync(operation);
                }

                var events = rates.SelectMany(r => r.GetEvents());
                return events;
            }

            private async Task<List<Rate>> GetRates()
            {
                var openExchangeRateTickers = Constants.FiatTickers.Concat(Constants.MetalsTickers);
                var tasks = new List<Task<List<Rate>>>
                {
                    _openExchangeRatesService.GetExchangeRates(openExchangeRateTickers),
                    _coinMarketCapService.GetCryptoCurrencies(Constants.CryptoTickers),
                };

                var results = await Task.WhenAll(tasks);

                var rates = results.SelectMany(t => t).ToList();

                return rates;
            }
        }
    }
}

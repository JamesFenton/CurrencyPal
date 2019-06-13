using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Retry;
using Rates.Functions.Services;
using Rates.Functions.WriteModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rates.Functions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var services = builder.Services;

            var settings = new Settings
            {
                DatabaseConnectionString = Environment.GetEnvironmentVariable("RATES_DB_CONNECTIONSTRING"),
                CoinMarketCapApiKey = Environment.GetEnvironmentVariable("CMC_API_KEY"),
                OpenExchangeRatesApiKey = Environment.GetEnvironmentVariable("OPENEXCHANGERATES_APPID"),
                IexApiKey = Environment.GetEnvironmentVariable("IEX_TOKEN"),
            };

            services.AddSingleton(settings);
            services.AddSingleton<Database>();
            services.AddSingleton<RateSaver>();

            // retry policy
            var retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(4),
            });
            services.AddSingleton<RetryPolicy>(retryPolicy);

            // services
            services.AddSingleton<CoinMarketCapService>();
            services.AddSingleton<IexService>();
            services.AddSingleton<OpenExchangeRatesService>();

        }
    }
}

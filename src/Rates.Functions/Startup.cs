using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Rates.Functions.Services;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

[assembly: FunctionsStartup(typeof(Rates.Functions.Startup))]

namespace Rates.Functions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var services = builder.Services;
            
            services.AddSingleton(c => new Database(Environment.GetEnvironmentVariable("AzureWebJobsStorage")));
            
            // external services
            var retryTimeouts = new[]
            {
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(10),
            };

            // coinmarketcap
            services.AddHttpClient<CoinMarketCapService>(client =>
            {
                client.BaseAddress = new Uri("https://pro-api.coinmarketcap.com");
                client.DefaultRequestHeaders.Add("X-CMC_PRO_API_KEY", Environment.GetEnvironmentVariable("CMC_API_KEY"));
            })
            .AddTransientHttpErrorPolicy(builder => builder.WaitAndRetryAsync(retryTimeouts));

            // IEX
            services.AddHttpClient<IexService>(client =>
            {
                client.BaseAddress = new Uri("https://cloud.iexapis.com");
            })
            .AddTransientHttpErrorPolicy(builder => builder.WaitAndRetryAsync(retryTimeouts))
            .AddHttpMessageHandler(c => new IexTokenHandler(Environment.GetEnvironmentVariable("IEX_TOKEN")));

            // open exchange rates
            services.AddHttpClient<OpenExchangeRatesService>(client =>
            {
                client.BaseAddress = new Uri("https://openexchangerates.org");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", Environment.GetEnvironmentVariable("OPENEXCHANGERATES_APPID"));
            })
            .AddTransientHttpErrorPolicy(builder => builder.WaitAndRetryAsync(retryTimeouts));
        }
    }
}

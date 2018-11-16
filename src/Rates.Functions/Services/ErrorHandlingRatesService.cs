using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Polly;
using Rates.Functions.WriteModel;

namespace Rates.Functions.Services
{
    public class ErrorHandlingRatesService : IRatesService
    {
        private readonly IRatesService _inner;

        public Rate[] Rates => new Rate[0];

        public ErrorHandlingRatesService(IRatesService inner)
        {
            _inner = inner;
        }

        public async Task<IEnumerable<RateEntity>> GetRates()
        {
            var policy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(4),
            });

            return await policy.ExecuteAsync(() => _inner.GetRates());
        }
    }
}

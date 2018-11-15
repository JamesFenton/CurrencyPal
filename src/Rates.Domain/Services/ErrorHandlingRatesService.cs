using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Polly;
using Rates.Core.WriteModel;

namespace Rates.Domain.Services
{
    public class ErrorHandlingRatesService : IRatesService
    {
        private readonly IRatesService _inner;

        public string[] Tickers => new string[0];

        public ErrorHandlingRatesService(IRatesService inner)
        {
            _inner = inner;
        }

        public async Task<List<Rate>> GetRates()
        {
            var policy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(4),
            });

            try
            {
                return await policy.ExecuteAsync(() => _inner.GetRates());
            }
            catch(Exception)
            {
                // return an empty list to prevent this failure killing the other services
                return new List<Rate>();
            }
        }
    }
}

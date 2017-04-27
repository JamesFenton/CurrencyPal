using CurrencyPal.Dto;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyPal.Services
{
    public class CachingRateService : IRateService
    {
        private const string RatesKey = "Rates";

        private readonly RateService _RateService;
        private readonly IMemoryCache _MemoryCache;

        public CachingRateService(RateService rateService, IMemoryCache memoryCache)
        {
            _RateService = rateService;
            _MemoryCache = memoryCache;
        }

        public async Task<RatesDto> GetExchangeRates()
        {
            RatesDto rates;
            if (!_MemoryCache.TryGetValue(RatesKey, out rates))
            {
                rates = await _RateService.GetExchangeRates();

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(1));

                _MemoryCache.Set(RatesKey, rates, cacheOptions);
            }

            return rates;
        }
    }
}

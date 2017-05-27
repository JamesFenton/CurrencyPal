using Rates.Web.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rates.Web.Services
{
    public class CachingRatesService : IRatesService
    {
        private RatesDto _cachedRates;
        private long _cacheExpiryTime;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromHours(1);

        private readonly RatesService _ratesService;

        public CachingRatesService(RatesService ratesService)
        {
            _ratesService = ratesService;
        }

        public async Task<RatesDto> GetRates()
        {
            var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var cacheExpired = now > _cacheExpiryTime;
            if (_cachedRates == null || cacheExpired)
            {
                _cachedRates = await _ratesService.GetRates();
                _cacheExpiryTime = DateTimeOffset.UtcNow.Add(_cacheDuration).ToUnixTimeMilliseconds();
                _cachedRates.NextUpdateTime = _cacheExpiryTime;
            }
            return _cachedRates;
        }
    }
}

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

        private readonly DatabaseRatesService _ratesService;

        public CachingRatesService(DatabaseRatesService ratesService)
        {
            _ratesService = ratesService;
        }

        public RatesDto GetRates()
        {
            var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var cacheExpired = now > _cacheExpiryTime;
            if (_cachedRates == null || cacheExpired)
            {
                var rates = _ratesService.GetRates();
                _cacheExpiryTime = DateTimeOffset.UtcNow.Add(_cacheDuration).ToUnixTimeMilliseconds();
                _cachedRates = new RatesDto
                {
                    Rates = rates,
                    UpdateTime = now,
                    NextUpdateTime = _cacheExpiryTime
                };
            }
            return _cachedRates;
        }
    }
}

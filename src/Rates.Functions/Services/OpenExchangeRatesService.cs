﻿using Newtonsoft.Json.Linq;
using Rates.Functions.WriteModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Rates.Functions.Services
{
    public class OpenExchangeRatesService
    {
        private readonly HttpClient _http;

        public OpenExchangeRatesService(HttpClient http)
        {
            _http = http;
        }

        public async Task<IEnumerable<RateEntity>> GetRates(IEnumerable<Rate> rates)
        {
            var json = await _http.GetStringAsync("/api/latest.json");
            var sourceRates = JObject.Parse(json)["rates"] as JObject;

            var values = rates.Select(t => ConvertRate(t));
            return values;

            RateEntity ConvertRate(Rate rate)
            {
                var ticker = rate.Ticker;
                var source = ticker.Substring(0, 3);
                var dest = ticker.Substring(3, 3);

                var sourceRate = sourceRates[source].Value<double>();
                var destRate = sourceRates[dest].Value<double>();
                var crossRate = destRate / sourceRate;
                return new RateEntity(ticker, DateTimeOffset.UtcNow, crossRate);
            }
        }
    }
}

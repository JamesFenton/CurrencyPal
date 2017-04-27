using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CurrencyPal.Services;
using CurrencyPal.Dto;

namespace CurrencyPal.Controllers
{
    [Route("api/[controller]")]
    public class RatesController : Controller
    {
        private readonly RateService _ExchangeRateService;

        public RatesController(RateService exchangeRateService)
        {
            _ExchangeRateService = exchangeRateService;
        }

        [HttpGet("{ticker}")]
        public async Task<RateDto> Get(string ticker)
        {
            return await _ExchangeRateService.GetExchangeRate(ticker);
        }
    }
}

using CurrencyPal.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyPal.Services
{
    public interface IRateService
    {
        Task<RatesDto> GetExchangeRates();
    }
}

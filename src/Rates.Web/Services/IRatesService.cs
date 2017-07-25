using Rates.Web.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rates.Web.Services
{
    public interface IRatesService
    {
        RatesDto GetRates();
    }
}

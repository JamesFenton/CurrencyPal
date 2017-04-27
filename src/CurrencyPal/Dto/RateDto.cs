using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyPal.Dto
{
    public class RatesDto
    {
        public List<RateDto> Rates { get; set; } = new List<RateDto>();
    }

    public class RateDto
    {
        public string Ticker { get; set; }
        public double Rate { get; set; }
        public string Source { get; set; }
    }
}

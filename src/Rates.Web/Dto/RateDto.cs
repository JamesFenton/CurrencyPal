using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rates.Web.Dto
{
    public class RateDto
    {
        public string Ticker { get; set; }
        public double Rate { get; set; }
        public double? Change24h { get; set; }
        public double? Change7d { get; set; }
    }
}

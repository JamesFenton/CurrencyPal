using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rates.Web.Dto
{
    public class RateDto
    {
        public string Ticker { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public double Rate { get; set; }
        public double? Change1Day { get; set; }
        public double? Change1Week { get; set; }
        public double? Change1Month { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Rates.Functions.ReadModel
{
    public class GetRatesDto
    {
        public List<RateRm> Rates { get; set; }
        public long UpdateTime { get; set; }
    }
}

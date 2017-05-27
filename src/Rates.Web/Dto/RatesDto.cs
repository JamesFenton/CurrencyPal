using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rates.Web.Dto
{
    public class RatesDto
    {
        public List<RateDto> Rates { get; set; } = new List<RateDto>();
        public long UpdateTime { get; set; }
        public long NextUpdateTime { get; set; }
    }
}

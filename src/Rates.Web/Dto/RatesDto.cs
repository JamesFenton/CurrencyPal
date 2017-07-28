using Rates.Core.ReadModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rates.Web.Dto
{
    public class RatesDto
    {
        public List<RateRm> Rates { get; set; }
        public long UpdateTime { get; set; }
    }
}

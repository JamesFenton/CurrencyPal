using Rates.Core.WriteModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Rates.Domain.Services
{
    public interface IRatesService
    {
        Task<List<Rate>> GetRates(IEnumerable<string> tickers);
    }
}

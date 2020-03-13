using Rates.Functions.WriteModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rates.Functions.Services
{
    public interface IRatesService
    {
        Task<IEnumerable<RateEntity>> GetRates(IEnumerable<Rate> rates);
    }
}
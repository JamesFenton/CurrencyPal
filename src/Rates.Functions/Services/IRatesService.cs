using Rates.Functions.WriteModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Rates.Functions.Services
{
    public interface IRatesService
    {
        string[] Tickers { get; }
        Task<List<Rate>> GetRates();
    }
}

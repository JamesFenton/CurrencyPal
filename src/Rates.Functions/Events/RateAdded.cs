using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rates.Functions.Events
{
    public class RateAdded : Event
    {
        public string Ticker { get; }
        public string TimeKey { get; }
        public double Value { get; }

        public RateAdded(string ticker, string timeKey, double value)
        {
            Ticker = ticker;
            TimeKey = timeKey;
            Value = value;
        }
    }
}

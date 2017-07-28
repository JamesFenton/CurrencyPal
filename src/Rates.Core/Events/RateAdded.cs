using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rates.Core.Events
{
    public class RateAdded : Event
    {
        public Guid Id { get; }
        public string Ticker { get; }
        public DateTime Timestamp { get; }
        public double Value { get; }

        public RateAdded(Guid id, string ticker, DateTime timestamp, double value)
        {
            Id = id;
            Ticker = ticker;
            Timestamp = timestamp;
            Value = value;
        }
    }
}

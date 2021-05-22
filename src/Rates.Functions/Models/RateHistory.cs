using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rates.Functions.Models
{
    public class RateHistory : Entity
    {
        public string Ticker => PartitionKey;
        public string Date => RowKey;
        public double Value { get; set; }

        public RateHistory() { }

        public RateHistory(string ticker, double value)
        {
            PartitionKey = ticker;
            RowKey = DateTimeOffset.UtcNow.GetNearestHour().ToString("yyyy-MM-dd");
            Value = value;
        }
    }
}

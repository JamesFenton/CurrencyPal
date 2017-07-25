using System;
using System.Collections.Generic;
using System.Text;

namespace Rates.Core.Models
{
    public class Rate
    {
        public Guid Id { get; }
        public string Ticker { get; }
        public DateTime Timestamp { get; }
        public double Value { get; }
        
        public Rate(Guid id, string ticker, DateTime timestamp, double value)
        {
            Id = id;
            Ticker = ticker;
            Timestamp = timestamp;
            Value = value;
        }
    }
}

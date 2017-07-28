using Rates.Core.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rates.Core.WriteModel
{
    public class Rate : Model
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

            AddEvent(new RateAdded(Id, Ticker, Timestamp, Value));
        }
    }
}

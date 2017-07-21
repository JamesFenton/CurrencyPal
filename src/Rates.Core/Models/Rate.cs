using System;
using System.Collections.Generic;
using System.Text;

namespace Rates.Core.Models
{
    public class Rate
    {
        public Guid Id { get; }
        public string Identifier { get; }
        public DateTime Timestamp { get; }
        public double Value { get; }
        
        public Rate(Guid id, string identifier, DateTime timestamp, double value)
        {
            Id = id;
            Identifier = identifier;
            Timestamp = timestamp;
            Value = value;
        }
    }
}

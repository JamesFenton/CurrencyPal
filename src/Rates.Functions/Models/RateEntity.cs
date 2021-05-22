using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rates.Functions.Models
{
    /// <summary>
    /// The value of a rate at a certain point in time
    /// </summary>
    public class RateEntity : Entity
    {
        /// <summary>
        /// Unique indentifier to the rate, eg USDZAR
        /// </summary>
        public string Ticker => PartitionKey;

        /// <summary>
        /// The time the rate was added (rounded to the nearest hour) in ISO format
        /// </summary>
        public string TimeKey => RowKey;

        /// <summary>
        /// The actual rate
        /// </summary>
        public double Value { get; set; }

        public RateEntity() { }

        public RateEntity(string ticker, DateTimeOffset now, double value)
        {
            PartitionKey = ticker;
            RowKey = now.GetNearestHour().ToString("o"); // ISO format
            Value = value;
        }
    }
}

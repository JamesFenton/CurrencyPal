using Rates.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rates.Core.WriteModel
{
    public class Rate : Model
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

        public Rate() { }

        public Rate(string ticker, DateTimeOffset now, double value)
        {
            PartitionKey = ticker;
            RowKey = GetNearestHour(now).ToString("o"); // ISO format
            Value = value;

            AddEvent(new RateAdded(Ticker, TimeKey, Value));
        }

        private static DateTimeOffset GetNearestHour(DateTimeOffset now)
        {
            var currentHour = new DateTimeOffset(now.Year, now.Month, now.Day, now.Hour, 0, 0, TimeSpan.FromHours(0));
            var previousHour = currentHour.AddHours(-1);
            var nextHour = currentHour.AddHours(1);

            return new[]
            {
                previousHour,
                currentHour,
                nextHour,
            }.Select(t => new Tuple<DateTimeOffset, double>(t, Math.Abs((now - t).TotalMilliseconds))) // (time, TimeSpan from now)
            .OrderBy(t => t.Item2) // take the time that is closest to now
            .First()
            .Item1;
        }
    }
}

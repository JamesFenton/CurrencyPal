using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rates.Core.ReadModel
{
    public class RateRm
    {
        public Guid Id { get; }
        public string Ticker { get; }
        public DateTime Timestamp { get; }
        public double Value { get; }

        public double? Change1Day { get; }
        public double? Change1Week { get; }
        public double? Change1Month { get; }
        public double? Change3Months { get; }
        public double? Change6Months { get; }
        public double? Change1Year { get; }

        public RateRm(
            Guid id,
            string ticker,
            DateTime timestamp,
            double value,
            double? change1Day,
            double? change1Week,
            double? change1Month,
            double? change3Months,
            double? change6Months,
            double? change1Year)
        {
            Id = id;
            Ticker = ticker;
            Timestamp = timestamp;
            Value = value;
            Change1Day = change1Day;
            Change1Week = change1Week;
            Change1Month = change1Month;
            Change3Months = change3Months;
            Change6Months = change6Months;
            Change1Year = change1Year;
        }
    }
}

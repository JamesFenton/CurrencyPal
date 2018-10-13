using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rates.Core.ReadModel
{
    public class RateRm : CosmosModel
    {
        public string Ticker { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public double Value { get; set; }

        public double? Change1Day { get; set; }
        public double? Change1Week { get; set; }
        public double? Change1Month { get; set; }
        public double? Change3Months { get; set; }
        public double? Change6Months { get; set; }
        public double? Change1Year { get; set; }

        // used by the front-end
        public bool OutOfDate => Timestamp < DateTimeOffset.UtcNow.AddHours(-1);

        public RateRm() { }

        public RateRm(
            string ticker,
            double value,
            double? change1Day,
            double? change1Week,
            double? change1Month,
            double? change3Months,
            double? change6Months,
            double? change1Year)
        {
            Id = ticker.ToString();
            Ticker = ticker;
            Timestamp = DateTimeOffset.UtcNow;
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

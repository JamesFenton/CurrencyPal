using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rates.Functions.ReadModel
{
    public class RateRm : TableEntity
    {
        public string Ticker { get; set; }
        public string Name { get; set; }
        public string Href { get; set; }
        public double Value { get; set; }
        public DateTimeOffset Timestamp { get; set; }

        public double? Change1Day { get; set; }
        public double? Change1Week { get; set; }
        public double? Change1Month { get; set; }
        public double? Change3Months { get; set; }
        public double? Change6Months { get; set; }
        public double? Change1Year { get; set; }

        public RateRm() { }

        public RateRm(
            string ticker,
            string name,
            string href,
            double value,
            double? change1Day,
            double? change1Week,
            double? change1Month,
            double? change3Months,
            double? change6Months,
            double? change1Year
        )
        {
            PartitionKey = "readmodel";
            RowKey = ticker;
            Ticker = ticker;
            Name = name;
            Href = href;
            Value = value;
            Timestamp = DateTimeOffset.UtcNow;
            Change1Day = change1Day;
            Change1Week = change1Week;
            Change1Month = change1Month;
            Change3Months = change3Months;
            Change6Months = change6Months;
            Change1Year = change1Year;
        }
    }
}

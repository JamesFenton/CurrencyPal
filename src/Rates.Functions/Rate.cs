using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rates.Functions
{
    public class Rate
    {
        public string Ticker { get; set; }
        public string Name { get; set; }
        public string Href { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public RateSource Source { get; set; }
        public string SourceSymbol { get; set; }
    }
}

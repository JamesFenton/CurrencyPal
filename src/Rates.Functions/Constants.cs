using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Text;

namespace Rates.Functions
{
    public static class Constants
    {
        internal const string RatesAddedQueue = "rates-added";

        public static readonly string[] FiatTickers = 
        {
            "USDZAR",
            "GBPZAR",
            "EURZAR",
            "ZARMUR"
        };

        public static readonly string[] MetalsTickers = 
        {
            "XAUUSD",
            "XAGUSD"
        };

        public static readonly string[] CryptoTickers =
        {
            "BTCUSD",
            "ETHUSD",
            "ZECUSD",
            "LTCUSD",
            "NEOUSD",
            "XLMUSD"
        };

        public static readonly string[] StockTickers =
        {
            "SPXUSD",
        };

        public static readonly string[] AllTickers = FiatTickers
            .Concat(StockTickers)
            .Concat(MetalsTickers)
            .Concat(CryptoTickers)
            .ToArray();


        internal readonly static JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            TypeNameHandling = TypeNameHandling.Auto,
        };

        internal readonly static JsonMediaTypeFormatter JsonFormatter = new JsonMediaTypeFormatter
        {
            SerializerSettings = JsonSettings
        };
    }
}

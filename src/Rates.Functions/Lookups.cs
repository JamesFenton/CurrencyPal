using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;

namespace Rates.Functions
{
    internal static class Lookups
    {
        internal const string RatesAddedQueue = "rates-added";

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

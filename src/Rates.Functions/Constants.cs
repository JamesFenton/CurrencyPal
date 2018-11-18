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

        internal readonly static JsonMediaTypeFormatter JsonFormatter = new JsonMediaTypeFormatter
        {
            SerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
            }
        };
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace Rates.Functions
{
    public static class Constants
    {
        internal const string RatesAddedQueue = "rates-added";
        internal static string StorageConnectionString => Environment.GetEnvironmentVariable("AzureWebJobsStorage");
    }
}

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rates.Core
{
    public class Database
    {
        private readonly CloudTableClient _client;

        public CloudTable Rates { get; }

        public CloudTable RatesRm { get; }

        public Database(string connectionString)
        {
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            _client = storageAccount.CreateCloudTableClient();

            Rates = _client.GetTableReference("rates");
            RatesRm = _client.GetTableReference("ratesrm");
        }
    }
}

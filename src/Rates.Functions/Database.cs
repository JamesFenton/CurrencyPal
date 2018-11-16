using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Rates.Functions
{
    public class Database
    {
        private readonly CloudTableClient _client;

        public CloudTable Rates { get; }

        public CloudTable RatesRm { get; }

        public Database(Settings settings)
        {
            var storageAccount = CloudStorageAccount.Parse(settings.DatabaseConnectionString);
            _client = storageAccount.CreateCloudTableClient();

            Rates = _client.GetTableReference("rates");
            RatesRm = _client.GetTableReference("ratesrm");
        }
    }
}

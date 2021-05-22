using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Rates.Functions.Models
{
    public abstract class Entity : TableEntity
    {
        private static readonly CloudTableClient _client = CloudStorageAccount.Parse(Constants.StorageConnectionString).CreateCloudTableClient();
        protected CloudTable _table => _client.GetTableReference(GetTableName());

        public async Task SaveAsync()
        {
            var operation = TableOperation.InsertOrReplace(this);
            await _table.ExecuteAsync(operation);
        }

        private string GetTableName()
        {
            if (this is RateEntity)
                return "rates";
            else if (this is RateRm)
                return"ratesrm";
            else
                return GetType().Name.ToLower();
        }
    }
}

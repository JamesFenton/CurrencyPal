using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rates.Functions.WriteModel
{
    public class RateSaver
    {
        private readonly Database _database;

        public RateSaver(Database database)
        {
            _database = database;
        }

        public async Task Save(IEnumerable<RateEntity> rates)
        {
            foreach (var rate in rates)
            {
                var operation = TableOperation.InsertOrReplace(rate);
                await _database.Rates.ExecuteAsync(operation);
            }
        }
    }
}

using MongoDB.Driver;
using Rates.Core.ReadModel;
using Rates.Core.WriteModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rates.Core
{
    public class Database
    {
        private readonly IMongoDatabase _database;

        public IMongoCollection<Rate> Rates => _database.GetCollection<Rate>("rates");

        public IMongoCollection<RateRm> RatesRm => _database.GetCollection<RateRm>("ratesRm");

        public Database(MongoClient client, string database)
        {
            _database = client.GetDatabase(database);

            Rates.Indexes.CreateOne(Builders<Rate>.IndexKeys.Combine(
                Builders<Rate>.IndexKeys.Ascending(r => r.Ticker),
                Builders<Rate>.IndexKeys.Ascending(r => r.Timestamp)
            ));

            RatesRm.Indexes.CreateOne(Builders<RateRm>.IndexKeys.Ascending(
                    r => r.Ticker
                ));
        }
    }
}

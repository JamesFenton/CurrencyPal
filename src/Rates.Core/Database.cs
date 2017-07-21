using MongoDB.Driver;
using Rates.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rates.Core
{
    public class Database
    {
        private readonly IMongoDatabase _database;

        public IMongoCollection<Rate> Rates => _database.GetCollection<Rate>("rates");

        public Database(MongoClient client, string database)
        {
            _database = client.GetDatabase(database);
            Rates.Indexes.CreateOne(Builders<Rate>.IndexKeys.Combine(
                Builders<Rate>.IndexKeys.Ascending(r => r.Identifier),
                Builders<Rate>.IndexKeys.Ascending(r => r.Timestamp)
            ));
        }
    }
}

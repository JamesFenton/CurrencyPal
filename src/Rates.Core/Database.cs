using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Rates.Core
{
    public class Database
    {
        public DocumentClient Client { get; }

        public readonly string DatabaseName = "rates";
        public readonly string RatesCollection = "rates";
        public readonly string RatesReadModelCollection = "rates-readmodel";
        
        public Uri RatesUri => UriFactory.CreateDocumentCollectionUri(DatabaseName, RatesCollection);
        public Uri RatesRmUri => UriFactory.CreateDocumentCollectionUri(DatabaseName, RatesReadModelCollection);

        public Database(string databaseUrl, string key)
        {
            Client = new DocumentClient(new Uri(databaseUrl), key);
        }

        public async Task Initialise()
        {
            await Client.CreateDatabaseIfNotExistsAsync(new Microsoft.Azure.Documents.Database { Id = DatabaseName });

            await Client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri(DatabaseName), new DocumentCollection { Id = RatesCollection });
            await Client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri(DatabaseName), new DocumentCollection { Id = RatesReadModelCollection });
        }
    }
}

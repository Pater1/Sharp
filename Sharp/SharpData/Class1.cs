using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpData
{
    public class Class1
    {
        protected static IMongoClient _client;
        protected static IMongoDatabase _database;
        
        public void connectToDB()
        {
            MongoClientSettings settings = new MongoClientSettings();
            //Enter the ip address of the mongo installation you wish to connect to inside the quotes below.
            string IPAddress = "10.10.14.66";
            settings.Server = new MongoServerAddress(IPAddress);

            _client = new MongoClient(settings);
            //Console.WriteLine(_client.ListDatabases().ToList().ElementAt(0));
            //Console.WriteLine(_client.ListDatabases().ToList().ElementAt(1));
            //Console.WriteLine(_client.ListDatabases().ToList().ElementAt(2));
            string databaseName = "";
            _database = _client.GetDatabase($"{databaseName}");
            //_database.CreateCollection("testCollection");
            //Console.WriteLine(_database.ListCollections().ToList().ElementAt(0));
            //Console.WriteLine(_database.ListCollections().ToList().ElementAt(1));
            string collectionName = "";
            var rest = _database.GetCollection<BsonDocument>($"{collectionName}");
        }
    }
}

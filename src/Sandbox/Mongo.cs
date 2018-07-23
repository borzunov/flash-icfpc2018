using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using NUnit.Framework;
using MongoDB.Driver.GridFS;

namespace Sandbox
{
    public class Lol
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("lols_numbers")]
        public List<int> LolsNumbers { get; set; }
    }

    [Explicit("It's examples")]
    public class Mongo
    {
        private IMongoDatabase db;

        [OneTimeSetUp]
        public void CreateDatabaseConnection()
        {
            const string connectionString = "mongodb://admin:sw8k83ng01bw5@vm-dev-cont2:27017";
            var client = new MongoClient(connectionString);

            db = client.GetDatabase("my_lols");
        }

        [Test]
        public void UploadBlob()
        {
            var bucket = new GridFSBucket(db);
            var id = bucket.UploadFromBytes("test_blob", new byte[]{1,2,3});

            Console.WriteLine(id.ToString());
        }

        [Test]
        public void DownloadBlob()
        {
            var bucket = new GridFSBucket(db);
            ObjectId.TryParse("5b550d43ad3ef0083c6e3ca7", out var id);
            var data = bucket.DownloadAsBytes(id);
        }

        [Test]
        public async Task CreateLols()
        {
            db.CreateCollection("big_lols");

            var lols = db.GetCollection<Lol>("big_lols");

            await lols.InsertManyAsync(new[]
            {
                new Lol
                {
                    Name = "Best lol",
                    LolsNumbers = new List<int>() {1, 2, 3}
                },
                new Lol
                {
                    Name = "Not lol",
                    LolsNumbers = new List<int>() {5, 6, 7, 8}
                },
                new Lol
                {
                    Name = "Lol^2",
                    LolsNumbers = new List<int>() {9, 10, 11, 12, 13}
                }
            });
        }

        [Test]
        public async Task SearchLols()
        {
            var collection = db.GetCollection<Lol>("big_lols");

            var filter = new BsonDocument("$or", new BsonArray{
                new BsonDocument("name", "Best lol"),
                new BsonDocument("lols_numbers", new BsonDocument("$size", 5))
            });
            var bestLols = await collection.FindAsync(filter);

            var lols = await bestLols.ToListAsync();

            foreach (var lol in lols)
            {
                Console.WriteLine($"Id = {lol.Id}\nName = {lol.Name}\n");
            }
        }

        [Test]
        public void DropLols()
        {
            db.DropCollection("big_lols");
        }
    }
}

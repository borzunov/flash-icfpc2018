using System;
using MongoDB.Driver;

namespace Flash.Infrastructure.Models
{
    public interface IJsonWriter
    {
        void Write(object obj);
    }

    public class ConsoleJsonWriter : IJsonWriter
    {
        public void Write(object obj)
        {
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(obj));
        }
    }

    public class MongoJsonWriter : IJsonWriter
    {
        private readonly string collectionName;

        public MongoJsonWriter(string collectionName="logs")
        {
            this.collectionName = collectionName;
        }
        public void Write(object obj)
        {
            var client = new MongoClient("mongodb://admin:sw8k83ng01bw5@vm-dev-cont2:27017");

            var db = client.GetDatabase("local");

            var logs = db.GetCollection<object>(collectionName);

            logs.InsertOne(obj);
        }
    }
}
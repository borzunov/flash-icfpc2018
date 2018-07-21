﻿using MongoDB.Driver;

namespace Flash.Infrastructure.Models
{
    public interface IJsonWriter
    {
        void Write(object obj);
    }

    public class MongoJsonWriter : IJsonWriter
    {
        public void Write(object obj)
        {
            var client = new MongoClient("mongodb://admin:sw8k83ng01bw5@vm-dev-cont2:27017");

            var db = client.GetDatabase("local");

            var logs = db.GetCollection<object>("logs");

            logs.InsertOne(obj);
        }
    }
}
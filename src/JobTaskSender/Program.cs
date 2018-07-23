using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JobsCommon;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace JobTaskSender
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = Jobs.CreateFactory();

            var client = new MongoClient(Jobs.MongoConnectionString);
            var db = client.GetDatabase(Jobs.MongoBlobsDbName);
            var bucket = new GridFSBucket(db);
            var blobId = bucket.UploadFromBytes("test_blob", File.ReadAllBytes(@"C:\Users\yuryev\source\repos\Run\Run\bin\Debug\Debug.zip"));
            Console.WriteLine(blobId.ToString());

            Send(factory, Jobs.QueueName, blobId.ToString());
            

        }

        private static void Send(ConnectionFactory factory, string queueName, string zipMongoBlobId)
        {
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: queueName,
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);

                    

                    for (int i = 0; i < 256; i++)
                    {
                        var message = JsonConvert.SerializeObject(new Message
                        {
                            ZipMongoBlobId = zipMongoBlobId,
                            FileNameNoRun = "run.exe",
                            Arguments = i.ToString()
                        });

                        var body = Encoding.UTF8.GetBytes(message);

                        channel.BasicPublish(exchange: "",
                            routingKey: queueName,
                            basicProperties: null,
                            body: body);

                        Console.WriteLine(" [x] Sent {0}", message);
                    }
                }
            }
        }
    }
}

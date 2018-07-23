using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
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
        static string problemsDirectory = @"\\vm-dev-cont1\data\problemsF";

        static void Main(string[] args)
        {
            var strategyName = args[0];
            var pathToRunDirectory = @"..\..\Run\bin";
            var pathToZip = "task.zip";
            if(File.Exists(pathToZip))
                File.Delete(pathToZip);
            ZipFile.CreateFromDirectory(pathToRunDirectory, pathToZip, CompressionLevel.Fastest, false);
            var name = strategyName + new Random().Next(50000);
            Console.WriteLine(name);
            SendZip(pathToZip, name);

        }

        private static void SendZip(string pathToZip, string name)
        {
            var factory = Jobs.CreateFactory();
            var client = new MongoClient(Jobs.MongoConnectionString);
            var db = client.GetDatabase(Jobs.MongoBlobsDbName);
            var bucket = new GridFSBucket(db);
            var blobId = bucket.UploadFromBytes(Path.GetFileName(pathToZip), File.ReadAllBytes(pathToZip));
            Console.WriteLine(blobId.ToString());

            var outDir = Path.Combine(@"\\vm-dev-cont1\TRACES", name);
            if (!Directory.Exists(outDir))
                Directory.CreateDirectory(outDir);
            var tasks = Directory.EnumerateFiles(problemsDirectory)
                .Where(x => Path.GetFileName(x).StartsWith("FA"))
                .Select(x =>
                {
                    var outTracePath = Path.Combine(outDir, $"{Path.GetFileName(x).Substring(0, 5)}.nbt");
                    var msg = new Message
                    {
                        ZipMongoBlobId = blobId.ToString(),
                        FileNameNoRun = "run.exe",
                        Arguments = $"--tgt={x.ToString()} --trace={outTracePath}"
                    };
                    var msgJson = JsonConvert.SerializeObject(msg);

                    var body = Encoding.UTF8.GetBytes(msgJson);

                    return body;
                }).ToList();

            Send(factory, Jobs.QueueName, tasks);
        }

        private static void Send(ConnectionFactory factory, string queueName, IEnumerable<byte[]> tasks)
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

                    foreach (var task in tasks)
                    {
                        channel.BasicPublish(exchange: "",
                            routingKey: queueName,
                            basicProperties: null,
                            body: task);
                    }
                }
            }
        }
    }
}

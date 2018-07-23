using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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

        public static int TimeoutMilliseconds = (int)TimeSpan.FromMinutes(10).TotalMilliseconds;

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

            var regex = new Regex(@"(?<key>\w{2}\d{3})_(?<type>\w{3}).mdl", RegexOptions.Compiled);

            var tasks = Directory.EnumerateFiles(problemsDirectory)
                .Select(path => new {path,  regex = regex.Match(Path.GetFileName(path))})
                .Where(x => x.regex.Success)
                .GroupBy(x => x.regex.Groups["key"].Value)
                //.Where(x => Path.GetFileName(x).StartsWith("FA"))
                //.Take(10)
                .Select(x =>
                {

                    var srcPath = x.FirstOrDefault(y => y.regex.Groups["type"].Value == "src")?.path;
                    var tgtPath = x.FirstOrDefault(y => y.regex.Groups["type"].Value == "tgt")?.path;
                    
                    var outTracePath = Path.Combine(outDir, $"{x.Key}.nbt");
                    var msg = new Message
                    {
                        ZipMongoBlobId = blobId.ToString(),
                        FileNameNoRun = "run.exe",
                        Arguments = GetArgsString(srcPath, tgtPath, outTracePath),
                        TimeoutMilliseconds = TimeoutMilliseconds
                    };
                    var msgJson = JsonConvert.SerializeObject(msg);

                    var body = Encoding.UTF8.GetBytes(msgJson);

                    return body;
                }).ToList();

            Send(factory, Jobs.QueueName, tasks);
        }

        private static string GetArgsString(string srcPath, string tgtPath, string outTracePath)
        {
            var args = new List<string>();
            if (!string.IsNullOrEmpty(srcPath))
                args.Add($"--src={srcPath}");
            if (!string.IsNullOrEmpty(tgtPath))
                args.Add($"--tgt={tgtPath}");
            if (!string.IsNullOrEmpty(outTracePath))
                args.Add($"--trace={outTracePath}");

            return string.Join(" ", args);
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

using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading;
using JobsCommon;
using log4net;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace JobExecutor
{
    public class MessageProcessor
    {
        private static readonly object GlobalLock = new object();
        private readonly ILog log;

        public MessageProcessor(ILog log)
        {
            this.log = log;
        }

        public void Process(Message message)
        {
            var executionId = Guid.NewGuid();
            log.Info($"[{executionId}] Start processing {message}, {executionId}");
            ProcessInternal(message);
            log.Info($"[{executionId}] End processing {message}, {executionId}");
        }

        private void ProcessInternal(Message message)
        {
            var workPath = PrepareEnv(message);
            RunCode(message, workPath);
        }

        private static string PrepareEnv(Message message)
        {
            lock (GlobalLock)
            {
                if (!Directory.Exists("work"))
                    Directory.CreateDirectory("work");

                //todo: wrap in lock
                var workPath = Path.Combine("work", message.ZipMongoBlobId);
                if (!Directory.Exists(workPath))
                {
                    Directory.CreateDirectory(workPath);


                    var client = new MongoClient(Jobs.MongoConnectionString);
                    var db = client.GetDatabase(Jobs.MongoBlobsDbName);
                    var bucket = new GridFSBucket(db);

                    if (!ObjectId.TryParse(message.ZipMongoBlobId, out var blobId))
                        throw new InvalidOperationException($"Can't parse `{message.ZipMongoBlobId}` as ObjectId");

                    var bytes = bucket.DownloadAsBytes(blobId);
                    var zipFileName = $"{message.ZipMongoBlobId}.zip";
                    File.WriteAllBytes(zipFileName, bytes);
                    ZipFile.ExtractToDirectory(zipFileName, workPath);
                }

                return workPath;
            }
        }

        private void RunCode(Message message, string workPath)
        {
            var process = new Process
            {
                StartInfo =
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    FileName = Path.Combine(workPath, message.FileNameNoRun),
                    Arguments = message.Arguments
                }
            };
            process.OutputDataReceived += (sender, args) =>
            {
                if (args.Data != null)
                    log.Info(args.Data);
            };
            process.Start();
            process.BeginOutputReadLine();
            process.WaitForExit();
        }
    }
}
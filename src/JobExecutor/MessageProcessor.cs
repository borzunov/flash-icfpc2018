using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
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
        private readonly MongoClient mongoClient;

        public MessageProcessor(ILog log, MongoClient mongoClient)
        {
            this.log = log;
            this.mongoClient = mongoClient;
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
            try
            {
                var processResult = ProcessResult.FromMessage(message, Environment.MachineName);
                var workPath = PrepareEnv(message);
                RunCode(message, workPath);
                processResult.IsSuccess = true;
                DumpProcessResult(processResult);

            }
            catch (Exception e)
            {
                var processResult = ProcessResult.FromMessage(message, Environment.MachineName);
                processResult.IsSuccess = false;
                processResult.ErrorMessage = e.ToString();
                DumpProcessResult(processResult);
            }
        }

        private void DumpProcessResult(ProcessResult processResult)
        {
            var db = mongoClient.GetDatabase(Jobs.MongoJobsDbName);
            var results = db.GetCollection<ProcessResult>("results");
            results.InsertOne(processResult);
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
                {
                    log.Info(args.Data);
                }
            };
            process.Start();
            process.BeginOutputReadLine();

            process.WaitForExit(message.TimeoutMilliseconds);

            if (!process.HasExited)
            {
                process.Kill();
                throw new TimeoutException($"Timeout: {message.TimeoutMilliseconds}ms");
            }

            if(process.ExitCode != 0)
            {
                throw new NonZeroExitCodeException($"Exit code: {process.ExitCode}");
            }


        }

        private class NonZeroExitCodeException : Exception
        {
            public NonZeroExitCodeException(string msg) : base(msg)
            {

            }
        }


        private class TimeoutException : Exception
        {
            public TimeoutException(string msg) : base(msg)
            {
                
            }
        }
    }
}
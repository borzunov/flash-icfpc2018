using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JobsCommon;
using log4net;
using Newtonsoft.Json;

namespace JobExecutor
{
    public class Orchestrator
    {
        private readonly ILog log;
        private readonly int maxConcurency = Environment.ProcessorCount;
        private readonly int maxQueueSize = 2;
        private int currentRunningTasksCount = 0;
        private readonly object lockObj = new object();
        private BlockingCollection<Message> blockingCollection = new BlockingCollection<Message>();

        public Orchestrator(log4net.ILog log)
        {
            this.log = log;
        }

        public void Start()
        {
            var factory = Jobs.CreateFactory();
            var queueName = Jobs.QueueName;


            var workers = Enumerable.Range(0, maxConcurency)
                .Select(i => Task.Run(() => DoWork()))
                .ToList();


            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {

                channel.QueueDeclare(queue: queueName,
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                while (true)
                {
                    lock (lockObj)
                    {
                        if (currentRunningTasksCount >= maxConcurency || blockingCollection.Count >= maxQueueSize)
                        {
                            Thread.Sleep(100);
                            continue;
                        }
                    }

                    var basicGetResult = channel.BasicGet(queueName, true);
                    if (basicGetResult == null)
                    {
                        Thread.Sleep(100);
                        continue;
                    }

                    try
                    {
                        var message =
                            JsonConvert.DeserializeObject<Message>(Encoding.UTF8.GetString(basicGetResult.Body));
                        blockingCollection.Add(message);
                    }
                    catch (Exception e)
                    {
                        log.Error("Enqueue error", e);
                    }
                }
            }
        }

        private void DoWork()
        {
            var messageProcessor = new MessageProcessor(log);
            while (true)
            {
                foreach (var message in blockingCollection.GetConsumingEnumerable())
                {
                    try
                    {
                        lock (lockObj)
                            currentRunningTasksCount++;

                        messageProcessor.Process(message);
                    }
                    finally
                    {
                        lock (lockObj)
                            currentRunningTasksCount--;
                    }

                }
            }
        }
    }
}
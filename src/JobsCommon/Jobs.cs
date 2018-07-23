using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace JobsCommon
{
    public class Jobs
    {
        public static string QueueName = "cool_queue";
        public const string MongoConnectionString = "mongodb://admin:sw8k83ng01bw5@vm-dev-cont2:27017";
        public const string MongoBlobsDbName = "blobs";
        public const string MongoJobsDbName = "jobs";
        public const string MongoJobsResultsCollectionName = "results";

        public static ConnectionFactory CreateFactory()
        {
            var factory = new ConnectionFactory()
            {
                HostName = "vm-dev-cont1",
                UserName = "test",
                Password = "test",
            };

            return factory;
        }
    }
}

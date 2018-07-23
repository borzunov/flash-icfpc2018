using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace JobsCommon
{
    public class ProcessResult
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("friendlyName")]
        public string FriendlyName { get; set; }

        [BsonElement("zipMongoBlobId")]
        public string ZipMongoBlobId { get; set; }

        [BsonElement("fileNameNoRun")]
        public string FileNameNoRun { get; set; }

        [BsonElement("arguments")]
        public string Arguments { get; set; }

        [BsonElement("executor")]
        public string Executor { get; set; }

        [BsonElement("isSuccess")]
        public bool IsSuccess { get; set; }

        [BsonElement("errorMessage")]
        public string ErrorMessage { get; set; }

        [BsonElement("timeoutMilliseconds")]
        public int TimeoutMilliseconds { get; set; }

        public static ProcessResult FromMessage(Message message, string executor)
        {
            return new ProcessResult()
            {
                FriendlyName =  message.FriendlyName,
                ZipMongoBlobId = message.ZipMongoBlobId,
                FileNameNoRun = message.FileNameNoRun,
                Arguments = message.Arguments,
                TimeoutMilliseconds = message.TimeoutMilliseconds,
                Executor = executor
            };
        }
    }
}
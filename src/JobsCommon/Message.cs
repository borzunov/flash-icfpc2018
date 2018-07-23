using System;
using Newtonsoft.Json;

namespace JobsCommon
{
    public class Message
    {
        public string ZipMongoBlobId { get; set; }
        public string FileNameNoRun { get; set; }
        public string Arguments { get; set; }

        public override string ToString()
        {
            return $"{nameof(ZipMongoBlobId)}: {ZipMongoBlobId}, {nameof(FileNameNoRun)}: {FileNameNoRun}, {nameof(Arguments)}: {Arguments}";
        }
    }
}

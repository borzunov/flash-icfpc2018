namespace JobsCommon
{
    public class Message
    {
        public string FriendlyName { get; set; }
        public string ZipMongoBlobId { get; set; }
        public string FileNameNoRun { get; set; }
        public string Arguments { get; set; }

        public override string ToString()
        {
            return $"{nameof(FriendlyName)}: {FriendlyName}, {nameof(ZipMongoBlobId)}: {ZipMongoBlobId}, {nameof(FileNameNoRun)}: {FileNameNoRun}, {nameof(Arguments)}: {Arguments}";
        }
    }
}

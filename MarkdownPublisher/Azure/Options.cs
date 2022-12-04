namespace MarkdownPublisher.Azure
{
    internal class Options
    {
        public string ConnectionString { get; set; } = default!;
        public string ContainerName { get; set; } = default!;
        public string BlobPrefix { get; set; } = default!;
    }
}

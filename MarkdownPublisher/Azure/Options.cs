using MarkdownPublisher.Interfaces;

namespace MarkdownPublisher.Azure
{
    internal class Options : IProjectOptions
    {
        public string ConnectionString { get; set; } = default!;
        public string ContainerName { get; set; } = default!;
        public string BlobPrefix { get; set; } = default!;
        public string HostRoutePrefix { get; set; } = default!;
    }
}

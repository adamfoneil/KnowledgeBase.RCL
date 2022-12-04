using MarkdownPublisher.Abstract;

namespace MarkdownPublisher.Azure
{
    internal class Publisher : PublisherBase<Options>
    {
        public Publisher(string json) : base(json)
        {
        }

        protected override Options BuildConfiguration(string json)
        {
            throw new NotImplementedException();
        }

        protected override Task PublishFileAsync(string localFile)
        {
            throw new NotImplementedException();
        }
    }
}

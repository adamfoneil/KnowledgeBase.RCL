namespace MarkdownPublisher.Interfaces
{
    internal interface IPublisher
    {
        Task PublishAsync(string sourcePath);        
    }
}
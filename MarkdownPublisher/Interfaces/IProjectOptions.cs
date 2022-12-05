namespace MarkdownPublisher.Interfaces
{
    internal interface IProjectOptions
    {
        /// <summary>
        /// since your content is accessed inside another web app, you need to 
        /// indicate the route to your content in the context of the application
        /// </summary>
        string HostRoutePrefix { get; }
    }
}

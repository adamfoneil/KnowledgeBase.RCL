namespace MarkdownPublisher.Interfaces
{
    internal interface IProjectOptions
    {
        /// <summary>
        /// if your publish destination is shared by other apps, 
        /// then you may want to prepend a folder name to keep app-specific content isolated.
        /// Should end with slash
        /// </summary>
        string BlobPrefix { get; }
        /// <summary>
        /// since your content is accessed inside another web app, you need to 
        /// indicate the route to your content in the context of the application.
        /// Should start and end with slash
        /// </summary>
        string HostRoutePrefix { get; }
    }
}

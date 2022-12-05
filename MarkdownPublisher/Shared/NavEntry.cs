namespace Knowledgebase.Shared
{
    public class NavEntry
    {
        /// <summary>
        /// builds tree view in reader UI,
        /// last part is the title
        /// </summary>
        public string Route { get; set; } = default!;
        /// <summary>
        /// link to file as the host application sees it (with IProjectOptions.HostRoutePrefix)
        /// </summary>
        public string HostHref { get; set; } = default!;
        /// <summary>
        /// link to file has an http client sees it (no prefix)
        /// </summary>
        public string Href { get; set; } = default!;
    }
}

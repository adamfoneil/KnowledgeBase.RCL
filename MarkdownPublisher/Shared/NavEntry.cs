namespace KnowledgeBase.Shared
{
    public class NavEntry
    {
        /// <summary>
        /// builds tree view in reader UI,
        /// last part is the title
        /// </summary>
        public string Route { get; set; } = default!;
        /// <summary>
        /// link to the physical file
        /// </summary>
        public string Href { get; set; } = default!;
    }
}

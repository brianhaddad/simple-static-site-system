namespace SSClasses
{
    [Serializable]
    public class PageDefinition
    {
        public bool IsIndex { get; set; }
        public string FileName { get; set; }
        public string PageTitle { get; set; }
        public string PageSubdirectory { get; set; }
        public string PageLayoutTemplate { get; set; }
        public List<string> PageStylesheets { get; set; }
        public int NavMenuSortIndex { get; set; }
    }
}

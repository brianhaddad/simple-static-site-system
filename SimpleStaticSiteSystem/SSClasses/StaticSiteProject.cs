namespace SSClasses
{
    [Serializable]
    public class StaticSiteProject
    {
        public Dictionary<string, string> GlobalProjectValues { get; set; }
        public List<PageDefinition> PageDefinitions { get; set; }
        public Dictionary<string , string> SiteBuildTargets { get; set; }
    }
}
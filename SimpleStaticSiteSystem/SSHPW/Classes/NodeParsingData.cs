namespace SSHPW.Classes
{
    public class NodeParsingData
    {
        public string TagName { get; set; }
        public Dictionary<string, string> Attributes { get; set; }
        public bool IsSelfClosing { get; set; }
    }
}

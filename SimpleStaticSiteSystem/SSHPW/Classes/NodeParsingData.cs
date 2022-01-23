using SSHPW.Classes.Enums;

namespace SSHPW.Classes
{
    public class NodeParsingData
    {
        public ParsingDataType ParsedDataType { get; set; }
        public string TagName { get; set; }
        public IEnumerable<string[]> Attributes { get; set; }
        public bool ContainsCloser { get; set; }
        public bool IsClosingTag { get; set; }
        public string Text { get; set; }
    }
}

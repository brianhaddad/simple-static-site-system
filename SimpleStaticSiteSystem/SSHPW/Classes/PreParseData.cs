namespace SSHPW.Classes
{
    public class PreParseData
    {
        public bool ContainsDocTypeDeclaration { get; set; }
        public List<string> DocTypeValues { get; set; }
        public List<NodeParsingData> Data { get; set; }
    }
}

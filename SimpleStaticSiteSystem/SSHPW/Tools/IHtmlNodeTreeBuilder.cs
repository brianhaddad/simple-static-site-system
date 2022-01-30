using SSClasses;
using SSHPW.Classes;

namespace SSHPW.Tools
{
    public interface IHtmlNodeTreeBuilder
    {
        bool ContainsDocTypeDeclaration { get; }
        List<string> DocTypeValues { get; }

        HtmlNode BuildNodeTree();
        HtmlNode BuildNodeTree(List<NodeParsingData> data);
        void SetData(List<NodeParsingData> data);
    }
}
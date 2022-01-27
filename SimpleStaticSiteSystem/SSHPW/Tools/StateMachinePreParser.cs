using SSHPW.Classes;
using SSHPW.Extensions;

namespace SSHPW.Tools
{
    public enum ParserState
    {
        SearchingForTag,
        ReadingTagName,
        ReadingTagAttribute,
        ReadingAttributeValue,
        ReadingHtmlTextNodeContent,
        ReadingSpecialFormattedTextContent,
        ReadingHtmlCommentText,
    }

    public class StateMachinePreParser : IHtmlPreParser
    {
        private ParserState _state = ParserState.SearchingForTag;
        private string _textBuffer = string.Empty;
        private int _currentLine = 0;
        private int _currentCharacter = 0;
        private string[] _lines;

        public List<NodeParsingData> GetParsedSymbols(string text) => GetParsedSymbols(text.SplitByNewline());

        public List<NodeParsingData> GetParsedSymbols(string[] lines)
        {
            _currentLine = 0;
            _currentCharacter = 0;
            _lines = lines;
            var result = new List<NodeParsingData>();
            
            return result;
        }
    }
}

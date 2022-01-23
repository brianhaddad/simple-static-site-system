using SSHPW.Classes;
using SSHPW.Classes.Enums;
using SSHPW.Exceptions;

namespace SSHPW
{
    public class HtmlPreParser
    {
        private const string DOCTYPE = "<!DOCTYPE";
        private const string SPACE = " ";
        private const string OPEN = "<";
        private const string CLOSE = ">";
        private const string EQUALS = "=";
        private const string TAG_CLOSER = "/";
        private readonly string NEWLINE = Environment.NewLine;

        private string SanitizedText;
        private string Text;
        private int CurrentParsingPosition => SanitizedText.IndexOf(Text);

        public PreParseData GetParsingData(string text)
        {
            SanitizedText = text;
            Text = text;
            var result = new PreParseData
            {
                ContainsDocTypeDeclaration = Text.BeginsWith(DOCTYPE),
                Data = new List<NodeParsingData>(),
            };

            if (result.ContainsDocTypeDeclaration)
            {
                result.DocTypeValues = new List<string>();
                if (Eat(DOCTYPE))
                {
                    var grab = TextUpToNext(CLOSE);
                    var values = SplitAttributes(grab);
                    foreach (var value in values)
                    {
                        if (value.Length > 0 && value != SPACE)
                        {
                            result.DocTypeValues.Add(value);
                        }
                    }
                    Eat(grab + CLOSE);
                }
                Eat(NEWLINE);
            }

            var parts = Text.Split(OPEN).Skip(1);
            foreach (var part in parts)
            {
                Eat(OPEN);
                var tagText = TextUpToNext(CLOSE);
                var nodeData = new NodeParsingData
                {
                    ParsedDataType = ParsingDataType.Tag,
                    ContainsCloser = tagText.Contains(TAG_CLOSER),
                    IsClosingTag = tagText.BeginsWith(TAG_CLOSER),
                };
                if (tagText.Length == 0)
                {
                    throw new HtmlParsingErrorException($"Expected to find a closing tag after position {CurrentParsingPosition}.");
                }
                var tagParts = tagText.Split(SPACE);
                nodeData.TagName = tagParts[0].ReplaceAll(TAG_CLOSER, "");
                if (tagParts.Length > 1)
                {
                    nodeData.Attributes = tagParts.Skip(1).Where(x => !x.Contains(TAG_CLOSER)).Select(x => x.Split(EQUALS));
                }
                result.Data.Add(nodeData);
                var remainder = Nibble(part, tagText + CLOSE).ReplaceAll(NEWLINE + SPACE, NEWLINE);
                if (remainder.Length > 0 && remainder != NEWLINE)
                {
                    var textNodeData = new NodeParsingData
                    {
                        ParsedDataType = ParsingDataType.Text,
                        Text = remainder,
                    };
                    result.Data.Add(textNodeData);
                }
                Eat(part);
            }
            if (Text.Length > 0)
            {
                throw new HtmlParsingErrorException($"Unexpected error occurred while parsing the text: {SanitizedText}{NEWLINE}Remaining text: {Text}");
            }
            return result;
        }

        private bool CanEat(string next) => Text.BeginsWith(next, true);
        private bool Eat(string next)
        {
            if (CanEat(next))
            {
                Text = Text.Substring(next.Length);
                return true;
            }
            return false;
        }
        private string TextUpToNext(string search)
            => Text.IndexOf(search) > 0 ? Text.Substring(0, Text.IndexOf(search)) : "";

        private string[] SplitAttributes(string text) => text.Replace(NEWLINE, SPACE).Split(SPACE);
        private string Nibble(string text, string search)
            => text.BeginsWith(search) ? text.Substring(search.Length) : "";
    }
}

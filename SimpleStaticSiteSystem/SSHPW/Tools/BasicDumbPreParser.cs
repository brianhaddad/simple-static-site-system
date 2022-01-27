using SSHPW.Classes;
using SSHPW.Classes.Enums;
using SSHPW.Exceptions;
using SSHPW.Extensions;

namespace SSHPW.Tools
{
    //TODO: rewrite this as a letter by letter state machine pre-processor.
    //Features: text buffer to collect current data, states to handle special cases, attributes, values, etc...
    //Outputs the same thing as this class does, but does not take in sanitized text.
    //Able to maintain a concept of where in the text it is parsing so error can output exact line and character for the user.
    //On trigger character can look ahead if necessary (like for new line or comment tag) but generally just goes one character at a time.
    public class BasicDumbPreParser : IHtmlPreParser
    {
        private readonly HtmlStringSanitizer _sanitizer;
        private const string SPACE = " ";
        private const string OPEN = "<";
        private const string CLOSE = ">";
        private const string EQUALS = "=";
        private const string TAG_CLOSER = "/";
        private const string COMMENT_TAG = "!--";
        private readonly string NEWLINE = Environment.NewLine;
        private readonly string[] SPECIAL_CASES = new[]
        {
            "script",
            "pre",
            "textarea",
        };

        private string SanitizedText;
        private string Text;
        private int CurrentParsingPosition => SanitizedText.IndexOf(Text);

        public BasicDumbPreParser(HtmlStringSanitizer sanitizer)
        {
            _sanitizer = sanitizer ?? throw new ArgumentNullException(nameof(sanitizer));
        }

        public List<NodeParsingData> GetParsedSymbols(string[] lines)
            => GetParsedSymbols(lines.Join(Environment.NewLine));

        public List<NodeParsingData> GetParsedSymbols(string text)
        {
            SanitizedText = _sanitizer.Sanitize(text);
            Text = SanitizedText;
            var result = new List<NodeParsingData>();

            var parts = Text.Split(OPEN).Skip(1);
            foreach (var part in parts)
            {
                Eat(OPEN);
                if (CanEat(part))
                {
                    var tagText = TextUpToNext(CLOSE);
                    var nodeData = new NodeParsingData
                    {
                        ParsedDataType = ParsingDataType.Tag,
                        IsSelfClosing = tagText.EndsWith(TAG_CLOSER, true),
                        IsClosingTag = tagText.BeginsWith(TAG_CLOSER),
                    };
                    if (tagText.Length == 0)
                    {
                        throw new HtmlParsingErrorException($"Expected to find a closing tag after position {CurrentParsingPosition}.");
                    }
                    var tagParts = tagText.Split(SPACE);
                    nodeData.TagName = tagParts[0].ReplaceAll(TAG_CLOSER, "");
                    if (nodeData.TagName == COMMENT_TAG)
                    {
                        nodeData.ParsedDataType = ParsingDataType.Comment;
                        nodeData.IsSelfClosing = true;
                    }
                    if (tagParts.Length > 1)
                    {
                        nodeData.Attributes = tagParts.Skip(1).Where(x => x != TAG_CLOSER).Select(x => x.Split(EQUALS));
                    }
                    result.Add(nodeData);

                    if (!nodeData.IsClosingTag && SPECIAL_CASES.Any(x => nodeData.TagName.Contains(x, StringComparison.CurrentCultureIgnoreCase)))
                    {
                        //TODO: all whitespace formatting within the special case has been obliterated by the sanitizer.
                        //Can we recover the original data? Maybe during the final step making use of the find ignoring whitespace?
                        //Would it even be possible to account for the parent tag indentation and just preserve the intended internal indentations?
                        var appetizer = TextUpToNext(CLOSE);
                        Eat(appetizer + CLOSE);
                        var closingTag = OPEN + TAG_CLOSER + nodeData.TagName + CLOSE;
                        var contents = TextUpToNext(closingTag);
                        if (contents.Length > 0)
                        {
                            var specialText = new NodeParsingData
                            {
                                ParsedDataType = ParsingDataType.Text,
                                Text = CleanUpSpecialTextContent(contents),
                            };
                            result.Add(specialText);
                            Eat(contents);
                        }
                    }
                    else
                    {
                        var remainder = Nibble(part, tagText + CLOSE).ReplaceAll(NEWLINE + SPACE, NEWLINE);
                        if (remainder.Length > 0 && remainder != NEWLINE)
                        {
                            var textNodeData = new NodeParsingData
                            {
                                ParsedDataType = ParsingDataType.Text,
                                Text = remainder,
                            };
                            result.Add(textNodeData);
                        }
                        Eat(part);
                    }
                }
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

        private string CleanUpSpecialTextContent(string text)
        {
            var result = text.Trim();
            if (result.BeginsWith(NEWLINE))
            {
                result = result.Substring(NEWLINE.Length);
            }
            if (result.EndsWith(NEWLINE))
            {
                result = result.Substring(0, result.Length - NEWLINE.Length);
            }
            return result.ReplaceAll(NEWLINE + " ", NEWLINE).Trim();
        }
    }
}

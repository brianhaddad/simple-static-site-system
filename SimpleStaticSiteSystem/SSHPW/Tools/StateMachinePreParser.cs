using SSHPW.Classes;
using SSHPW.Classes.Enums;
using SSHPW.Exceptions;
using SSHPW.Extensions;
//TODO: throw more errors in this class.
//Detect things that shouldn't be, and boom, error.
namespace SSHPW.Tools
{
    public enum ParserState
    {
        SearchingForTag,
        ReadingTagName,
        SeekingEndOfTag,
        ReadingTagAttribute,
        ReadingAttributeValue,
        ReadingSpecialFormattedTextContent,
        ReadingHtmlCommentText,
    }

    public class StateMachinePreParser : IHtmlPreParser
    {
        private const string FORWARD_SLASH = "/";
        private const string ASTERISK = "*";
        private const string ESCAPE_CHARACTER = @"\";
        private const string TAG_CLOSE = ">";
        private const string TAG_OPEN = "<";
        private const string SPACE = " ";
        private const string TAB = "	";
        private const string QUOTATION = "\"";
        private const string EQUALS = "=";
        private readonly string EOL = Environment.NewLine;
        private readonly string[] SPECIAL_CASES = new[]
        {
            "SCRIPT",
            "PRE",
            "TEXTAREA",
        };
        private readonly string[] SCRIPT_QUOTE_MARKS = new string[]
        {
            "\"",
            "'",
            "`",
        };

        private ParserState _state = ParserState.SearchingForTag;
        private string _textBuffer = string.Empty;
        private int _currentLine = 0;
        private int _currentCharacter = 0;
        private bool _inQuotes = false;
        private string _previousCharacter = string.Empty;
        private string _lastQuoteMarkSeen = string.Empty;
        private bool _inScriptSingleLineComment = false;
        private bool _inScriptMultiLineComment = false;
        private bool InScriptComment => _inScriptSingleLineComment || _inScriptMultiLineComment;
        private bool InScriptQuotes => _lastQuoteMarkSeen != string.Empty;

        private NodeParsingData _nextNode;
        private string[] _lines;
        private List<NodeParsingData> _result;

        private string NextCharacter => _currentCharacter < _lines[_currentLine].Length
            ? _lines[_currentLine].Substring(_currentCharacter, 1)
            : EOL;
        private bool NextCharacterIsWhitespace => NextCharacter == SPACE || NextCharacter == TAB || NextCharacter == EOL;
        private bool PreviousCharacterIsWhitespace => _previousCharacter == SPACE || _previousCharacter == TAB || _previousCharacter == EOL;
        private bool CurrentlyEscaped => _previousCharacter == ESCAPE_CHARACTER;
        private bool EndOfQuotes => _inQuotes && !CurrentlyEscaped && NextCharacter == QUOTATION;
        private bool BufferHasContent => _textBuffer != string.Empty && _textBuffer.Length > 0;
        private bool BufferHasNonWhitespaceContent => BufferHasContent && _textBuffer.RemoveWhitespace().Length > 0;

        public List<NodeParsingData> GetParsedSymbols(string text) => GetParsedSymbols(text.SplitByNewline());

        public List<NodeParsingData> GetParsedSymbols(string[] lines)
        {
            ResetStateMachine();
            _lines = lines;
            _result = new List<NodeParsingData>();

            do
            {
                switch (_state)
                {
                    case ParserState.SearchingForTag:
                        SearchingForTag();
                        break;
                    case ParserState.ReadingTagName:
                        ReadingTagName();
                        break;
                    case ParserState.SeekingEndOfTag:
                        SeekingEndOfTag();
                        break;
                    case ParserState.ReadingTagAttribute:
                        ReadingTagAttribute();
                        break;
                    case ParserState.ReadingAttributeValue:
                        ReadingAttributeValue();
                        break;
                    case ParserState.ReadingSpecialFormattedTextContent:
                        ReadingSpecialFormattedTextContent();
                        break;
                    case ParserState.ReadingHtmlCommentText:
                        ReadingHtmlCommentText();
                        break;
                    default:
                        Error($"The statemachine entered an unhandled state {_state}.");
                        break;
                }
            } while (_currentLine < _lines.Length && _currentCharacter <= _lines[_currentLine].Length);
            
            return _result;
        }

        private void SearchingForTag()
        {
            if (NextCharacter == TAG_OPEN)
            {
                if (BufferHasNonWhitespaceContent)
                {
                    _nextNode = new NodeParsingData
                    {
                        Text = _textBuffer,
                        ParsedDataType = ParsingDataType.Text
                    };
                    _result.Add(_nextNode);
                }
                _nextNode = new NodeParsingData
                {
                    ParsedDataType = ParsingDataType.Tag,
                };
                _state = ParserState.ReadingTagName;
                _textBuffer = string.Empty;
            }
            else
            {
                AddNextCharacterToBuffer();
            }
            AdvanceCharacter();
        }

        private void ReadingTagName()
        {
            if (NextCharacter == FORWARD_SLASH && !BufferHasNonWhitespaceContent)
            {
                _nextNode.IsClosingTag = true;
                AdvanceCharacter();
                _textBuffer = string.Empty;
            }
            if (NextCharacter == FORWARD_SLASH
                || NextCharacter == TAG_CLOSE
                || (BufferHasNonWhitespaceContent & NextCharacterIsWhitespace)
                || NextCharacter == ESCAPE_CHARACTER)
            {
                _nextNode.TagName = _textBuffer.Trim();
                if (_nextNode.TagName == "!--")
                {
                    _nextNode.ParsedDataType = ParsingDataType.Comment;
                    _state = ParserState.ReadingHtmlCommentText;
                }
                else if (_nextNode.TagName == string.Empty || _nextNode.TagName.Length < 1)
                {
                    Error("An empty tag was encountered.");
                }
                else
                {
                    _state = ParserState.SeekingEndOfTag;
                }
                _textBuffer = string.Empty;
            }
            else
            {
                AddNextCharacterToBuffer();
                AdvanceCharacter();
            }
        }

        private void SeekingEndOfTag()
        {
            if (NextCharacter == TAG_CLOSE)
            {
                if (SPECIAL_CASES.Contains(_nextNode.TagName.ToUpper()))
                {
                    _state = ParserState.ReadingSpecialFormattedTextContent;
                }
                else
                {
                    _state = ParserState.SearchingForTag;
                }
                _result.Add(_nextNode);
            }
            else if (NextCharacter == FORWARD_SLASH)
            {
                _nextNode.IsSelfClosing = true;
            }
            else if (PreviousCharacterIsWhitespace
                && !NextCharacterIsWhitespace
                && NextCharacter != FORWARD_SLASH
                && NextCharacter != TAG_CLOSE)
            {
                if (_nextNode.Attributes is null)
                {
                    _nextNode.Attributes = new List<string[]>();
                }
                _state = ParserState.ReadingTagAttribute;
                return;
            }
            AdvanceCharacter();
        }

        private void ReadingTagAttribute()
        {
            if (BufferHasNonWhitespaceContent
                && ((NextCharacter == TAG_CLOSE || NextCharacter == FORWARD_SLASH)
                || (PreviousCharacterIsWhitespace
                && !NextCharacterIsWhitespace
                && NextCharacter != ESCAPE_CHARACTER
                && NextCharacter != EQUALS)))
            {
                _nextNode.Attributes.Add(new[] { _textBuffer.Trim() });
                _textBuffer = string.Empty;
                _state = ParserState.SeekingEndOfTag;
            }
            else if (BufferHasNonWhitespaceContent && NextCharacter == EQUALS)
            {
                _nextNode.Attributes.Add(new[] { _textBuffer.Trim(), "" });
                _state = ParserState.ReadingAttributeValue;
                _textBuffer = string.Empty;
                _inQuotes = false;
                AdvanceCharacter();
            }
            else
            {
                AddNextCharacterToBuffer();
                AdvanceCharacter();
            }
        }

        private void ReadingAttributeValue()
        {
            if (!BufferHasNonWhitespaceContent && NextCharacter == QUOTATION)
            {
                _inQuotes = true;
            }
            else if (EndOfQuotes
                || (!_inQuotes && BufferHasNonWhitespaceContent
                && (NextCharacterIsWhitespace || NextCharacter == TAG_CLOSE || NextCharacter == FORWARD_SLASH)))
            {
                if (_inQuotes)
                {
                    _textBuffer = $"\"{_textBuffer}\"";
                }
                AddValueToLastAttribute();
                _state = ParserState.SeekingEndOfTag;
                if (!_inQuotes)
                {
                    return;
                }
                _inQuotes = false;
            }
            else
            {
                AddNextCharacterToBuffer();
            }
            AdvanceCharacter();
        }

        private void AddValueToLastAttribute()
        {
            var current = _nextNode.Attributes.Last();
            current[1] = _textBuffer.Trim();
            _textBuffer = string.Empty;
        }

        private void ReadingSpecialFormattedTextContent()
        {
            var originalTagName = _nextNode.TagName;
            var seekingTag = $"</{originalTagName}>";
            var isScript = originalTagName.ToUpper() == "SCRIPT";
            var ignoreTag = isScript && (InScriptQuotes || InScriptComment);
            if (!ignoreTag && _textBuffer.EndsWith(seekingTag, true) && !(_textBuffer.EndsWith(EOL) || _textBuffer.EndsWith(SPACE)))
            {
                var lines = _textBuffer.Substring(0, _textBuffer.LastIndexOf(TAG_OPEN)).SplitByNewline();
                if (isScript)
                {
                    //TODO: normalize indent on smallest amount of whitespace
                    //also, currently this portion appears to be producing garbage...
                    //weird empty lines before and after the text...
                }

                // Create and add text node:
                _nextNode = new NodeParsingData
                {
                    ParsedDataType = ParsingDataType.Text,
                    Text = lines.Join(EOL),
                };
                _result.Add(_nextNode);

                // Create and add close tag:
                _nextNode = new NodeParsingData
                {
                    ParsedDataType = ParsingDataType.Tag,
                    TagName = originalTagName,
                    IsClosingTag = true,
                };
                _result.Add(_nextNode);

                _textBuffer = string.Empty;
                _state = ParserState.SearchingForTag;
            }
            else
            {
                if (isScript)
                {
                    if (!InScriptComment
                        && !CurrentlyEscaped
                        && NextCharacter == _lastQuoteMarkSeen)
                    {
                        _lastQuoteMarkSeen = string.Empty;
                    }
                    else if (!InScriptComment
                        && !CurrentlyEscaped
                        && SCRIPT_QUOTE_MARKS.Contains(NextCharacter))
                    {
                        _lastQuoteMarkSeen = NextCharacter;
                    }
                    else if (!InScriptQuotes
                        && !InScriptComment
                        && _lastQuoteMarkSeen == string.Empty
                        && _previousCharacter == FORWARD_SLASH
                        && NextCharacter == FORWARD_SLASH)
                    {
                        _inScriptSingleLineComment = true; //TODO: write tests for the comment stuff
                    }
                    else if (!InScriptQuotes
                        && _inScriptSingleLineComment
                        && NextCharacter == EOL)
                    {
                        _inScriptSingleLineComment = false;
                    }
                    else if (!InScriptQuotes
                        && !InScriptComment
                        && _previousCharacter == FORWARD_SLASH
                        && NextCharacter == ASTERISK)
                    {
                        _inScriptMultiLineComment = true;
                    }
                    else if (!InScriptQuotes
                        && _inScriptMultiLineComment
                        && _previousCharacter == ASTERISK
                        && NextCharacter == FORWARD_SLASH)
                    {
                        _inScriptMultiLineComment = false;
                    }
                }
                AddNextCharacterToBuffer();
                AdvanceCharacter();
            }
        }

        private void ReadingHtmlCommentText()
        {
            if (_previousCharacter == "-" && NextCharacter == TAG_CLOSE && _textBuffer.EndsWith("--"))
            {
                _nextNode.Text = _textBuffer.Substring(0, _textBuffer.Length - 2);
                _result.Add(_nextNode);
                _textBuffer = string.Empty;
                _state = ParserState.SearchingForTag;
            }
            else
            {
                AddNextCharacterToBuffer();
            }
            AdvanceCharacter();
        }

        private void AddNextCharacterToBuffer()
        {
            _textBuffer += NextCharacter;
        }

        private void AdvanceCharacter()
        {
            _previousCharacter = NextCharacter;
            _currentCharacter++;
            if (_currentCharacter > _lines[_currentLine].Length)
            {
                _currentCharacter = 0;
                _currentLine++;
            }
        }

        private void Error(string message)
            => throw new HtmlParsingErrorException($"Parsing error on line: {_currentLine}, column: {_currentCharacter}{Environment.NewLine}{message}");

        private void ResetStateMachine()
        {
            _state = ParserState.SearchingForTag;
            _textBuffer = string.Empty;
            _currentLine = 0;
            _currentCharacter = 0;
            _previousCharacter = string.Empty;
            _inQuotes = false;
        }
    }
}

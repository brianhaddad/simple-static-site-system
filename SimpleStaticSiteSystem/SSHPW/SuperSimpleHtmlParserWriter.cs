using SSHPW.Classes;
using SSHPW.Classes.Enums;
using SSHPW.Tools;

namespace SSHPW
{
    public class SuperSimpleHtmlParserWriter : ISuperSimpleHtmlParserWriter
    {
        private readonly HtmlNodeStringifier stringifier;
        private readonly HtmlParser parser;
        private readonly HtmlStringificationOptions stringifierOptions;

        public SuperSimpleHtmlParserWriter()
        {
            stringifierOptions = new HtmlStringificationOptions
            {
                IndentString = "    ",
                TagCaseBehavior = TagCaseOptions.UpperCase,
            };
            stringifier = new HtmlNodeStringifier(stringifierOptions);
            parser = new HtmlParser(new HtmlStringSanitizer(), new HtmlPreParser());
        }

        public SuperSimpleHtmlParserWriter(HtmlStringificationOptions options)
        {
            stringifierOptions = options;
            stringifier = new HtmlNodeStringifier(stringifierOptions);
            parser = new HtmlParser(new HtmlStringSanitizer(), new HtmlPreParser());
        }

        public string[] Stringify(HtmlDocument htmlDoc) => stringifier.Stringify(htmlDoc);
        public HtmlDocument Parse(string[] documentLines) => parser.Parse(documentLines);
        public HtmlDocument Parse(string documentText) => parser.Parse(documentText);
    }
}

using SSHPW.Classes;
using SSHPW.Classes.Enums;
using SSHPW.Tools;

namespace SSHPW
{
    public class SuperSimpleHtmlParserWriter : ISuperSimpleHtmlParserWriter
    {
        private readonly IHtmlNodeStringifier stringifier;
        private readonly IHtmlParser parser;
        private readonly HtmlStringificationOptions stringifierOptions;

        public SuperSimpleHtmlParserWriter()
        {
            stringifierOptions = new HtmlStringificationOptions
            {
                IndentString = "    ",
                TagCaseBehavior = TagCaseOptions.UpperCase,
            };
            stringifier = new HtmlNodeStringifier(stringifierOptions);
            parser = new HtmlParser(new StateMachinePreParser(), new HtmlNodeTreeBuilder());
        }

        public SuperSimpleHtmlParserWriter(HtmlStringificationOptions options)
        {
            stringifierOptions = options;
            stringifier = new HtmlNodeStringifier(stringifierOptions);
            parser = new HtmlParser(new StateMachinePreParser(), new HtmlNodeTreeBuilder());
        }

        public string[] Stringify(HtmlDocument htmlDoc) => stringifier.Stringify(htmlDoc);
        public HtmlDocument Parse(string[] documentLines) => parser.Parse(documentLines);
        public HtmlDocument Parse(string documentText) => parser.Parse(documentText);
    }
}

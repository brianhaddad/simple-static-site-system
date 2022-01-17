using SSHPW.Classes;

namespace SSHPW
{
    public class HtmlParser
    {
        private readonly HtmlStringSanitizer _sanitizer;

        public HtmlParser(HtmlStringSanitizer sanitizer)
        {
            _sanitizer = sanitizer ?? throw new ArgumentNullException(nameof(sanitizer));
        }

        public ParsedHtmlNodeTree Parse(string[] lines)
        {
            throw new NotImplementedException();
        }

        public ParsedHtmlNodeTree Parse(string text)
        {
            throw new NotImplementedException();
        }
    }
}
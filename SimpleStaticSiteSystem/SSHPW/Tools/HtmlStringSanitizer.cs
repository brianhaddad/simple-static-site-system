using SSHPW.Extensions;

namespace SSHPW.Tools
{
    // NOTE:
    // This whole concept will be defunct once the new parser is written.
    //TODO: delete this whole shebang.
    public class HtmlStringSanitizer
    {
        public string Sanitize(string[] lines) => Sanitize(lines.Join(Environment.NewLine));

        public string Sanitize(string text)
        {
            //Leaving new lines. The parser will just have to treat newlines and spaces separately.
            //Eventually I'd love to just remove newlines and learn to treat some spaces as legitimate and others as removable,
            //But that will take some practice/time.
            //Once that process is fully developed and working, perhaps we can adjust the sanitizer.
            text = StandardizeNewLines(text);
            text = RemoveDoubleNewLines(text);
            text = ReplaceTabWithSpace(text);
            text = RemoveDoubleSpaces(text);
            text = CleanUpTags(text);
            return text;
        }

        private string StandardizeNewLines(string text) => text.RegexReplace(@"(\r\n|\n|\r)", Environment.NewLine);
        private string RemoveDoubleNewLines(string text) => text.ReplaceAll(Environment.NewLine + Environment.NewLine, Environment.NewLine);
        private string RemoveDoubleSpaces(string text) => text.ReplaceAll("  ", " ");
        private string ReplaceTabWithSpace(string text) => text.ReplaceAll("	", " ");
        private string CleanUpTags(string text)
        {
            text = text.ReplaceAll("< ", "<");
            text = text.ReplaceAll(" >", ">");
            text = text.ReplaceAll("/ ", "/");
            text = text.ReplaceAll($"<{Environment.NewLine}", "<");
            text = text.ReplaceAll($"{Environment.NewLine}>", ">");
            text = text.ReplaceAll($"/{Environment.NewLine}", "/");
            return text;
        }
    }
}

namespace SSHPW
{
    public class HtmlStringSanitizer
    {
        public string Sanitize(string[] lines) => Sanitize(lines.Join(" "));

        public string Sanitize(string text)
        {
            text = RemoveNewLines(text);
            text = ReplaceTabWithSpace(text);
            text = RemoveDoubleSpaces(text);
            text = CleanUpTags(text);
            return text;
        }

        private string RemoveNewLines(string text) => text.ReplaceAll(Environment.NewLine, " ");
        private string RemoveDoubleSpaces(string text) => text.ReplaceAll("  ", " ");
        private string ReplaceTabWithSpace(string text) => text.ReplaceAll("	", " ");
        private string CleanUpTags(string text)
        {
            text = text.ReplaceAll("< ", "<");
            text = text.ReplaceAll(" >", ">");
            text = text.ReplaceAll("/ ", "/");
            return text;
        }
    }
}

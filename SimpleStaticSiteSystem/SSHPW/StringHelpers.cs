using System.Text.RegularExpressions;

namespace SSHPW
{
    public static class StringHelpers
    {
        public static string ReplaceAll(this string text, string find, string replacement)
        {
            while (text.Contains(find))
            {
                text = text.Replace(find, replacement);
            }
            return text;
        }

        public static string RegexReplace(this string text, string regex, string replacement) => Regex.Replace(text, regex, replacement);

        public static string Join(this string[] lines, string joiner) => string.Join(joiner, lines);
        public static string Join(this List<string> lines, string joiner) => lines.ToArray().Join(joiner);

        public static string Repeat(this string text, int count)
        {
            var result = "";
            for (var i = 0; i < count; i++)
            {
                result += text;
            }
            return result;
        }

        public static bool BeginsWith(this string text, string find, bool ignoreWhitespace = false)
            => ignoreWhitespace
                ? text.RemoveWhitespace().BeginsWith(find.RemoveWhitespace())
                : text.StartsWith(find, StringComparison.CurrentCultureIgnoreCase);

        public static string RemoveWhitespace(this string text)
            => text.RegexReplace(@"(\r\n|\n|\r)", "").ReplaceAll(" ", "");
    }
}

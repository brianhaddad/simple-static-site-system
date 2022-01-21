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
    }
}

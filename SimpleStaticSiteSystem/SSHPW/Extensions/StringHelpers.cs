using System.Text.RegularExpressions;

namespace SSHPW.Extensions
{
    public static class StringHelpers
    {
        public const string NewlineRegexText = @"(\r\n|\n|\r)";
        public static string ReplaceAll(this string text, string find, string replacement)
        {
            while (text.Contains(find))
            {
                text = text.Replace(find, replacement);
            }
            return text;
        }

        public static string RegexReplace(this string text, string regex, string replacement) => Regex.Replace(text, regex, replacement);
        public static bool RegexContains(this string text, string regex) => Regex.IsMatch(text, regex);
        public static bool ContainsNewline(this string text) => text.RegexContains(NewlineRegexText);

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
                ? text.RemoveWhitespace().BeginsWith(find.RemoveWhitespace(), false)
                : text.StartsWith(find, StringComparison.CurrentCultureIgnoreCase);

        public static bool EndsWith(this string text, string find, bool ignoreWhitespace = false)
            => ignoreWhitespace
                ? text.RemoveWhitespace().EndsWith(find.RemoveWhitespace(), false)
                : text.EndsWith(find, StringComparison.CurrentCultureIgnoreCase);

        public static bool Contains(this string text, string find, bool ignoreWhitespace = false)
            => ignoreWhitespace
                ? text.RemoveWhitespace().Contains(find.RemoveWhitespace(), false)
                : text.Contains(find, StringComparison.CurrentCultureIgnoreCase);

        public static string RemoveWhitespace(this string text)
            => text.RegexReplace(NewlineRegexText, "").ReplaceAll(" ", "").ReplaceAll("	", "");

        public static string[] SplitByNewline(this string text)
            => text.RegexReplace(NewlineRegexText, "[-breakHere-]").Split("[-breakHere-]");

        public static bool IsNullEmptyOrWhiteSpace(this string text)
            => string.IsNullOrEmpty(text) || string.IsNullOrWhiteSpace(text);

        public static bool IsInvalidFileName(this string text)
            => Path.GetInvalidFileNameChars().Any(c => text.Contains(c));

        public static bool IsInvalidFilePath(this string text)
            => Path.GetInvalidPathChars().Any(c => text.Contains(c));
    }
}

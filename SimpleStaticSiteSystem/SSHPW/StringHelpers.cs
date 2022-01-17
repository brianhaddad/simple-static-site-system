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

        public static string Join(this string[] lines, string joiner) => string.Join(joiner, lines);
    }
}

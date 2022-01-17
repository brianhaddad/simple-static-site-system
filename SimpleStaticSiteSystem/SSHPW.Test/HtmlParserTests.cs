using Microsoft.VisualStudio.TestTools.UnitTesting;
using SSHPW.Classes;

namespace SSHPW.Test
{
    [TestClass]
    public class HtmlParserTests
    {
        private HtmlParser _parser;
        private string[] _testLinesWithSpaceIndents => new[]
            {
                "<!doctype HTML>",
                "<html>",
                "    <head>",
                "        <title>My Test Page</title>",
                "    </head>",
                "    <body>",
                "        <p>This is some <em>weird</em> text.</p>",
                "        <hr />",
                "    </body>",
                "</html>",
            };

        [TestInitialize]
        public void Setup()
        {
            _parser = new HtmlParser(new HtmlStringSanitizer());
        }

        [TestMethod]
        public void LinesParseIntoCorrectType()
        {
            // Arrange
            var lines = _testLinesWithSpaceIndents;

            // Act
            var result = _parser.Parse(lines);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ParsedHtmlNodeTree));
        }
    }
}
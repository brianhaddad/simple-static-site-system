using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace SSHPW.Test
{
    [TestClass]
    public class HtmlStringSanitizerTests
    {
        private HtmlStringSanitizer _sanitizer;
        private const char TAB_CHARACTER = '	';
        private const string DOUBLE_SPACE = "  ";
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
        private string[] _testLinesWithWeirdStuff => new[]
            {
                "<!doctype HTML>",
                "<html>",
                "    <head>",
                "    	< title>My Test " + Environment.NewLine + " Page</title",
                ">",
                "    </head >",
                "    <",
                "body",
                ">",
                "        <p>This     is some <em >weird</ em > text.< / p>",
                "	     <hr  / >",
                "    <",
                "/",
                "body> ",
                "</html>",
            };

        [TestInitialize]
        public void Setup()
        {
            _sanitizer = new HtmlStringSanitizer();
        }

        [TestMethod]
        public void SanitizeCombinesLines()
        {
            // Arrange
            var lines = _testLinesWithSpaceIndents;

            // Act
            var sanitized = _sanitizer.Sanitize(lines);

            // Assert
            Assert.IsInstanceOfType(sanitized, typeof(string));
        }

        [TestMethod]
        public void SanitizeMaintainsMultiLineAttributeSeparationInLines()
        {
            // Arrange
            var expected = @"<tag
attribute=true
disabled />";
            var lines = new[]
            {
                "<tag",
                "attribute=true",
                "disabled />",
            };

            // Act
            var sanitized = _sanitizer.Sanitize(lines);

            // Assert
            Assert.AreEqual(expected, sanitized);
        }

        [TestMethod]
        public void SanitizeRemovesTabs()
        {
            // Arrange
            var lines = new[]
                {
                "<!doctype HTML>",
                "<html>",
                "	<head>",
                "		<title>My Test Page</title>",
                "	</head>",
                "	<body>",
                "		<p>This is some <em>weird</em> text.</p>",
                "		<hr />",
                "	</body>",
                "</html>",
            };

            // Act
            var sanitized = _sanitizer.Sanitize(lines);

            // Assert
            Assert.IsFalse(sanitized.Contains(TAB_CHARACTER));
        }

        [TestMethod]
        public void SanitizeRemovesDoubleNewLineCharacters()
        {
            // Arrange
        var text = @"<!doctype HTML>
<html>
    <head>
        <title>My Test Page</title>

    </head>
    <body>
        <p>This is some <em>weird</em> text.</p>


        <hr />
    </body>
</html>";

            // Act
            var sanitized = _sanitizer.Sanitize(text);

            // Assert
            Assert.IsFalse(sanitized.Contains(Environment.NewLine + Environment.NewLine));

        }

        [TestMethod]
        public void SanitizeRemovesDoubleSpaces()
        {
            // Arrange
            var lines = _testLinesWithSpaceIndents;

            // Act
            var sanitized = _sanitizer.Sanitize(lines);

            // Assert
            Assert.IsFalse(sanitized.Contains(DOUBLE_SPACE));
        }

        [TestMethod]
        public void SanitizeCleansUpInsideTags()
        {
            // Arrange
            var lines = _testLinesWithWeirdStuff;

            // Act
            var sanitized = _sanitizer.Sanitize(lines);

            // Assert
            Assert.IsFalse(sanitized.Contains("< "));
            Assert.IsFalse(sanitized.Contains(" >"));
            Assert.IsFalse(sanitized.Contains("/ "));
        }

        [TestMethod]
        public void SanitizeCleansUpMixedProblems()
        {
            // Arrange
            var lines = _testLinesWithWeirdStuff;

            // Act
            var sanitized = _sanitizer.Sanitize(lines);

            // Assert
            Assert.IsFalse(sanitized.Contains(TAB_CHARACTER));
            Assert.IsFalse(sanitized.Contains(DOUBLE_SPACE));
            Assert.IsFalse(sanitized.Contains(TAB_CHARACTER));
        }
    }
}

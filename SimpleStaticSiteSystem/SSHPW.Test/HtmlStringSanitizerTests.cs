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
        private const string MULTILINE_TAG_EXPECTED = "<tag attribute=true disabled />";
        private string _testMultiLineTagText => @"<tag
attribute=true
disabled />";
        private string[] _testMultiLineTagLines => new[]
        {
            "<tag",
            "attribute=true",
            "disabled />",
        };
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
                "    	< title>My Test " + Environment.NewLine + " Page</title>",
                "    </head >",
                "    <body>",
                "        <p>This     is some <em >weird</ em > text.< / p>",
                "	     <hr  / >",
                "    </body> ",
                "</html>",
            };
        private string[] _testLinesWithTabIndents => new[]
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
        private string _testTextWithNewLines => @"<!doctype HTML>
<html>
    <head>
        <title>My Test Page</title>
    </head>
    <body>
        <p>This is some <em>weird</em> text.</p>
        <hr />
    </body>
</html>";

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
        public void SanitizeMaintainsMultiLineAttributeSeparationInText()
        {
            // Arrange
            var lines = _testMultiLineTagText;

            // Act
            var sanitized = _sanitizer.Sanitize(lines);

            // Assert
            Assert.AreEqual(MULTILINE_TAG_EXPECTED, sanitized);
        }

        [TestMethod]
        public void SanitizeMaintainsMultiLineAttributeSeparationInLines()
        {
            // Arrange
            var lines = _testMultiLineTagLines;

            // Act
            var sanitized = _sanitizer.Sanitize(lines);

            // Assert
            Assert.AreEqual(MULTILINE_TAG_EXPECTED, sanitized);
        }

        [TestMethod]
        public void SanitizeRemovesTabs()
        {
            // Arrange
            var lines = _testLinesWithTabIndents;

            // Act
            var sanitized = _sanitizer.Sanitize(lines);

            // Assert
            Assert.IsFalse(sanitized.Contains(TAB_CHARACTER));
        }

        [TestMethod]
        public void SanitizeRemovesNewLineCharacters()
        {
            // Arrange
            var text = _testTextWithNewLines;

            // Act
            var sanitized = _sanitizer.Sanitize(text);

            // Assert
            Assert.IsFalse(sanitized.Contains(Environment.NewLine));

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

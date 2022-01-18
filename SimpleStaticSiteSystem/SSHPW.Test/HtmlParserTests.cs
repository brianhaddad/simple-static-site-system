using Microsoft.VisualStudio.TestTools.UnitTesting;
using SSHPW.Classes;
using SSHPW.Exceptions;
using System.Linq;

namespace SSHPW.Test
{
    [TestClass]
    public class HtmlParserTests
    {
        private HtmlParser _parser;
        private string[] _correctTestLines => new[]
            {
                "<!DOCTYPE html>",
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
        private string[] _testLinesMissingOpenTag => new[]
            {
                "<!DOCTYPE html>",
                "<html>",
                "        <title>My Test Page</title>",
                "    </head>",
                "    <body>",
                "        <p>This is some <em>weird</em> text.</p>",
                "        <hr />",
                "    </body>",
                "</html>",
            };
        private string[] _testLinesMissingCloseTag => new[]
            {
                "<!DOCTYPE html>",
                "<html>",
                "    <head>",
                "        <title>My Test Page</title>",
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
        [ExpectedException(typeof(HtmlParsingErrorException))]
        public void MissingOpeningTagThrowsException()
        {
            // Arrange
            var lines = _testLinesMissingOpenTag;

            // Act
            var result = _parser.Parse(lines);

            // Assert
        }

        [TestMethod]
        [ExpectedException(typeof(HtmlParsingErrorException))]
        public void MissingClosingTagThrowsException()
        {
            // Arrange
            var lines = _testLinesMissingCloseTag;

            // Act
            var result = _parser.Parse(lines);

            // Assert
        }

        [TestMethod]
        public void LinesParseIntoCorrectData()
        {
            // Arrange
            var lines = _correctTestLines;

            // Act
            var result = _parser.Parse(lines);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ParsedHtmlNodeTree));
            Assert.IsTrue(result.ContainsDocTypeDeclaration);
            Assert.AreEqual(1, result.DocTypeValues.Count);
            Assert.AreEqual("html", result.DocTypeValues.First());

            var html = VerifyHtmlNode(result.RootNode, "html", 0, 2, false);
            var head = VerifyHtmlNode(html.Children.First(), "head", 0, 1, false);
            var title = VerifyHtmlNode(head.Children.First(), "title", 0, 1, false);
            VerifyTextContent(title.Children.First(), "My Test Page");

            var body = VerifyHtmlNode(html.Children.Last(), "body", 0, 2, false);

            Assert.IsInstanceOfType(body.Children.First(), typeof(HtmlNode));
            var p = VerifyHtmlNode(body.Children.First(), "p", 0, 3, false);
            VerifyTextContent(p.Children.First(), "This is some ");
            var em = VerifyHtmlNode(p.Children.ElementAt(1), "em", 0, 1, false);
            VerifyTextContent(em.Children.First(), "weird");
            VerifyTextContent(p.Children.Last(), " text.");

            VerifyHtmlNode(body.Children.Last(), "hr", 0, 0, true);
        }

        private HtmlNode VerifyHtmlNode(INode node, string expectedTagName, int expectedAttributeCount, int expectedChildCount, bool expectedIsSelfClosing)
        {
            Assert.IsInstanceOfType(node, typeof(HtmlNode));
            var htmlNode = (HtmlNode)node;
            Assert.IsNotNull(htmlNode);
            Assert.AreEqual(expectedTagName, htmlNode.TagName);
            Assert.AreEqual(expectedAttributeCount, htmlNode.Attributes.Count);
            Assert.AreEqual(expectedChildCount, htmlNode.Children.Count);
            Assert.AreEqual(expectedIsSelfClosing, htmlNode.IsSelfClosing);
            return htmlNode;
        }

        private void VerifyTextContent(INode node, string expectedText)
        {
            Assert.IsInstanceOfType(node, typeof(TextContent));
            var text = (TextContent)node;
            Assert.IsNotNull(text);
            Assert.AreEqual(expectedText, text.Text);
        }
    }
}
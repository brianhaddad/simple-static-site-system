using Microsoft.VisualStudio.TestTools.UnitTesting;
using SSClasses;
using SSHPW.Exceptions;
using SSHPW.Tools;
using System.Linq;

namespace SSHPW.Test.Tools
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
                "        <p class=\"test\">This is some <em>weird</em> text.</p>",
                "        <hr />",
                "        <div></div>",
                "        <script>",
                "            var test = \"<testing>\";",
                "            alert(test);",
                "        </script>",
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
            _parser = new HtmlParser(new StateMachinePreParser(), new HtmlNodeTreeBuilder());
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
            Assert.IsInstanceOfType(result, typeof(HtmlDocument));
            Assert.IsTrue(result.ContainsDocTypeDeclaration);
            Assert.AreEqual(1, result.DocTypeValues.Count);
            Assert.AreEqual("html", result.DocTypeValues.First());

            var html = VerifyHtmlNode(result.RootNode, "html", 0, 2, false);
            var head = VerifyHtmlNode(html.Children.First(), "head", 0, 1, false);
            var title = VerifyHtmlNode(head.Children.First(), "title", 0, 1, false);
            VerifyTextContent(title.Children.First(), "My Test Page");

            var body = VerifyHtmlNode(html.Children.Last(), "body", 0, 4, false);

            Assert.IsInstanceOfType(body.Children.First(), typeof(HtmlNode));
            var p = VerifyHtmlNode(body.Children.First(), "p", 1, 3, false);
            Assert.IsNotNull(p.Attributes);
            Assert.IsTrue(p.Attributes.Count == 1);
            Assert.AreEqual("class", p.Attributes.First().Name);
            Assert.AreEqual("test", p.Attributes.First().Value);
            VerifyTextContent(p.Children.First(), "This is some ");
            var em = VerifyHtmlNode(p.Children.ElementAt(1), "em", 0, 1, false);
            VerifyTextContent(em.Children.First(), "weird");
            VerifyTextContent(p.Children.Last(), " text.");

            VerifyHtmlNode(body.Children.ElementAt(1), "hr", 0, 0, true);
            VerifyHtmlNode(body.Children.ElementAt(2), "div", 0, 0, false);
            VerifyHtmlNode(body.Children.Last(), "script", 0, 1, false);
        }

        private HtmlNode VerifyHtmlNode(HtmlNode node, string expectedTagName, int expectedAttributeCount, int expectedChildCount, bool expectedIsSelfClosing)
        {
            Assert.IsNotNull(node);
            Assert.IsFalse(node.IsTextOnlyNode);
            Assert.AreEqual(expectedTagName, node.TagName);
            Assert.AreEqual(expectedAttributeCount, node.Attributes?.Count ?? 0);
            Assert.AreEqual(expectedChildCount, node.Children?.Count ?? 0);
            Assert.AreEqual(expectedIsSelfClosing, node.IsSelfClosing);
            if (!expectedIsSelfClosing && expectedChildCount == 0)
            {
                Assert.IsTrue(node.ForceSeparateCloseTagForEmptyNode);
            }
            return node;
        }

        private void VerifyTextContent(HtmlNode node, string expectedText)
        {
            Assert.IsNotNull(node);
            Assert.IsTrue(node.IsTextOnlyNode);
            Assert.AreEqual(expectedText, node.Text);
        }
    }
}
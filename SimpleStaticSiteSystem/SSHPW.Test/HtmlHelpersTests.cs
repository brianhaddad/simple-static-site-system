using Microsoft.VisualStudio.TestTools.UnitTesting;
using SSHPW.Classes;
using System;
using System.Collections.Generic;

namespace SSHPW.Test
{
    [TestClass]
    public class HtmlHelpersTests
    {
        private string[] SuperSimpleTestResult => new string[]
        {
            "<!DOCTYPE html>",
            "<html>",
            "    <head>",
            "        <title>Test Page</title>",
            "    </head>",
            "    <body>",
            "        <p>Hello <em>world<em>!</p>",
            "        <hr />",
            "    </body>",
            "</html>",
        };
        private ParsedHtmlNodeTree GetSuperSimpleTestData()
        {
            var result = new ParsedHtmlNodeTree
            {
                ContainsDocTypeDeclaration = true,
                DocTypeValues = new List<string> { "html" },
                RootNode = new HtmlNode
                {
                    TagName = "html",
                    Children = new List<INode>
                    {
                        new HtmlNode
                        {
                            TagName = "head",
                            Children = new List<INode>
                            {
                                new HtmlNode
                                {
                                    TagName = "title",
                                    Children = new List<INode>
                                    {
                                        new TextContent { Text = "Test Page" },
                                    },
                                },
                            },
                        },
                        new HtmlNode
                        {
                            TagName = "body",
                            Children = new List<INode>
                            {
                                new HtmlNode
                                {
                                    TagName = "p",
                                    Children = new List<INode>
                                    {
                                        new TextContent { Text = "Hello " },
                                        new HtmlNode
                                        {
                                            TagName = "em",
                                            Children = new List<INode>
                                            {
                                                new TextContent { Text = "world" },
                                            },
                                        },
                                        new TextContent { Text = "!" },
                                    },
                                },
                                new HtmlNode
                                {
                                    TagName = "hr",
                                    IsSelfClosing = true,
                                },
                            },
                        }
                    }
                }
            };

            return result;
        }

        [TestMethod]
        public void ParsedHtmlNodeTreeCanStringify()
        {
            // Arrange
            var data = GetSuperSimpleTestData();
            var options = new HtmlStringificationOptions
            {
                IndentString = "    ",
            };

            // Act
            var result = data.Stringify(options);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(string));
            Assert.AreEqual(SuperSimpleTestResult.Join(Environment.NewLine), result);
        }
    }
}

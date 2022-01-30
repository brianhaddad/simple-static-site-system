using Microsoft.VisualStudio.TestTools.UnitTesting;
using SSClasses;
using SSHPW.Classes;
using SSHPW.Classes.Enums;
using SSHPW.Extensions;
using SSHPW.Tools;
using System;
using System.Collections.Generic;

namespace SSHPW.Test.Tools
{
    [TestClass]
    public class HtmlNodeStringifierTests
    {
        private string[] SuperSimpleTestExpectedResult => new string[]
        {
            "<!DOCTYPE html>",
            "<HTML>",
            "    <HEAD>",
            "        <TITLE>Test Page</TITLE>",
            "    </HEAD>",
            "    <BODY>",
            "        <P class=\"paragraph\">Hello <EM>world</EM>!</P>",
            "        <HR />",
            "        <P>Hello<BR />world!</P>",
            "        <DIV width=32></DIV>",
            "        <SCRIPT>",
            "            var test = \"<testing>\";",
            "            alert(test);",
            "        </SCRIPT>",
            "        <!-- comment -->",
            "    </BODY>",
            "</HTML>",
        };
        private HtmlDocument GetSuperSimpleTestData()
        {
            var result = new HtmlDocument
            {
                ContainsDocTypeDeclaration = true,
                DocTypeValues = new List<string> { "html" },
                RootNode = new HtmlNode
                {
                    TagName = "html",
                    Children = new List<HtmlNode>
                    {
                        new HtmlNode
                        {
                            TagName = "head",
                            Children = new List<HtmlNode>
                            {
                                new HtmlNode
                                {
                                    TagName = "title",
                                    Children = new List<HtmlNode>
                                    {
                                        new HtmlNode("Test Page"),
                                    },
                                },
                            },
                        },
                        new HtmlNode
                        {
                            TagName = "body",
                            Children = new List<HtmlNode>
                            {
                                new HtmlNode
                                {
                                    TagName = "p",
                                    Attributes = new List<HtmlNodeAttribute>
                                    {
                                        new HtmlNodeAttribute
                                        {
                                            Name = "class",
                                            Value = "paragraph",
                                            IsImplicitTrue = false,
                                            QuotesAroundValue = true,
                                        },
                                    },
                                    Children = new List<HtmlNode>
                                    {
                                        new HtmlNode("Hello "),
                                        new HtmlNode
                                        {
                                            TagName = "em",
                                            Children = new List<HtmlNode>
                                            {
                                                new HtmlNode("world"),
                                            },
                                        },
                                        new HtmlNode("!"),
                                    },
                                },
                                new HtmlNode
                                {
                                    TagName = "hr",
                                },
                                new HtmlNode
                                {
                                    TagName = "p",
                                    Children = new List<HtmlNode>
                                    {
                                        new HtmlNode("Hello"),
                                        new HtmlNode
                                        {
                                            TagName = "br",
                                        },
                                        new HtmlNode("world!"),
                                    },
                                },
                                new HtmlNode
                                {
                                    TagName = "div",
                                    Attributes = new List<HtmlNodeAttribute>
                                    {
                                        new HtmlNodeAttribute
                                        {
                                            Name = "width",
                                            Value = "32",
                                            IsImplicitTrue = false,
                                            QuotesAroundValue = false,
                                        },
                                    },
                                    ForceSeparateCloseTagForEmptyNode = true,
                                },
                                new HtmlNode
                                {
                                    TagName = "script",
                                    Children = new List<HtmlNode>
                                    {
                                        new HtmlNode("var test = \"<testing>\";\nalert(test);"),
                                    },
                                },
                                new HtmlNode(" comment ", true),
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
                TagCaseBehavior = TagCaseOptions.UpperCase,
            };
            var stringifier = new HtmlNodeStringifier(options);

            // Act
            var result = stringifier.Stringify(data);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Length == SuperSimpleTestExpectedResult.Length);
            Assert.AreEqual(SuperSimpleTestExpectedResult.Join(Environment.NewLine), result.Join(Environment.NewLine));
        }
    }
}

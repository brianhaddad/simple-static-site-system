﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using SSHPW.Tools;

namespace SSHPW.Test
{
    [TestClass]
    public class HtmlPreParserTests
    {
        [TestMethod]
        public void GetParsingData_green_path()
        {
            // Arrange
            var preParser = new HtmlPreParser();
            var testHtml = "<!doctype HTML>\r\n<html>\r\n <head>\r\n <title>My Test \r\n Page</title>\r\n </head>\r\n <body>\r\n <p class=\"test\">This is some <em>weird</em> text.</p>\r\n <hr class=\"test\" />\r\n </body> \r\n</html>";

            // Act
            var result = preParser.GetParsingData(testHtml);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ContainsDocTypeDeclaration);
            Assert.AreEqual(1, result.DocTypeValues.Count);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(18, result.Data.Count); //This might need adjusting as I learn how to handle newlines
        }

        [TestMethod]
        public void GetParsingData_handles_script_tag_with_html_data_inside()
        {
            // Arrange
            var preParser = new HtmlPreParser();
            var testHtml = "<!doctype HTML>\r\n<html>\r\n <head>\r\n <title>My Test Page</title>\r\n </head>\r\n <body>\r\n <div id=\"test\"></div>\r\n <script type=\"text/javascript\">var html = \"<tag>Hi there!</tag>\";\r\nalert(html);</script>\r\n </body> \r\n</html>";

            // Act
            var result = preParser.GetParsingData(testHtml);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ContainsDocTypeDeclaration);
            Assert.AreEqual(1, result.DocTypeValues.Count);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(15, result.Data.Count); //This might need adjusting as I learn how to handle newlines and stuff
        }

        [TestMethod]
        public void GetParsingData_multiline_handles_script_tag_with_html_data_inside()
        {
            // Arrange
            var preParser = new HtmlPreParser();
            var htmlLines = new[]
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
                    "    </BODY>",
                    "</HTML>",
                };
            var sanitizer = new HtmlStringSanitizer();
            var testHtml = sanitizer.Sanitize(htmlLines);

            // Act
            var result = preParser.GetParsingData(testHtml);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ContainsDocTypeDeclaration);
            Assert.AreEqual(1, result.DocTypeValues.Count);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(27, result.Data.Count); //This might need adjusting as I learn how to handle newlines and stuff
        }
    }
}

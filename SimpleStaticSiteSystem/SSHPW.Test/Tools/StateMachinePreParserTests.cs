using Microsoft.VisualStudio.TestTools.UnitTesting;
using SSHPW.Classes.Enums;
using SSHPW.Tools;
using System.Linq;

namespace SSHPW.Test.Tools
{
    [TestClass]
    public class StateMachinePreParserTests
    {
        [TestMethod]
        public void GetParsingData_green_path()
        {
            // Arrange
            var preParser = new StateMachinePreParser();
            var testHtml = "<!doctype HTML>\r\n<html>\r\n <head>\r\n <title>My Test \r\n Page</title>\r\n </head>\r\n <body>\r\n <p class=\"test\">This is some <em>weird</em> text.</p>\r\n <hr class=\"test\" />\r\n </body> \r\n</html>";

            // Act
            var result = preParser.GetParsedSymbols(testHtml);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.FirstOrDefault()?.TagName == "!doctype");
            Assert.AreEqual(1, result.FirstOrDefault().Attributes.Count());
            Assert.AreEqual(18, result.Count);
            Assert.AreEqual("\"test\"", result.FirstOrDefault(x => x.TagName == "p").Attributes.FirstOrDefault()[1]);
        }

        [TestMethod]
        public void GetParsingData_handles_script_tag_with_html_data_inside()
        {
            // Arrange
            var preParser = new StateMachinePreParser();
            var testHtml = "<!doctype HTML>\r\n<html>\r\n <head>\r\n <title>My Test Page</title>\r\n </head>\r\n <body>\r\n <div id=\"test\"></div>\r\n <script type=\"text/javascript\">var html = \"<tag>Hi there!</tag>\";\r\nalert(html);</script>\r\n </body> \r\n</html>";

            // Act
            var result = preParser.GetParsedSymbols(testHtml);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(15, result.Count); //This might need adjusting as I learn how to handle newlines and stuff
        }

        [TestMethod]
        public void GetParsingData_multiline_handles_script_tag_with_html_data_inside()
        {
            // Arrange
            var preParser = new StateMachinePreParser();
            var htmlLines = new[]
                {
                    "<!DOCTYPE html>",
                    "<HTML>",
                    "    <HEAD>",
                    "        <TITLE>Test Page</TITLE>",
                    "    </HEAD>",
                    "    <BODY>",
                    "        <SCRIPT>",
                    "            var test = \"</SCRIPT>\";",
                    "            var anotherTest = '</SCRIPT>';",
                    "            var aFinalTest = `</SCRIPT>`;",
                    "            //var test = \"</SCRIPT>\";",
                    "            //var anotherTest = '</SCRIPT>';",
                    "            //var aFinalTest = `</SCRIPT>`;",
                    "            //Ignore me: </SCRIPT>",
                    "            /*",
                    "            Ignoring lots of things.",
                    "            </script>",
                    "            '</script>'",
                    "            //</script>",
                    "            var test = \"</SCRIPT>\";",
                    "            var anotherTest = '</SCRIPT>';",
                    "            var aFinalTest = `</SCRIPT>`;",
                    "            */",
                    "            alert(test);",
                    "        </SCRIPT>",
                    "    </BODY>",
                    "</HTML>",
                };

            // Act
            var result = preParser.GetParsedSymbols(htmlLines);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(13, result.Count); //This might need adjusting as I learn how to handle newlines and stuff
        }

        [TestMethod]
        public void GetParsingData_multiline_handles_attributes_with_html_symbols_inside()
        {
            // Arrange
            var preParser = new StateMachinePreParser();
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
                    "        <INPUT type=\"button\" value=\"<\" />",
                    "        <INPUT type=\"button\" value=\">\" />",
                    "    </BODY>",
                    "</HTML>",
                };

            // Act
            var result = preParser.GetParsedSymbols(htmlLines);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(27, result.Count); //This might need adjusting as I learn how to handle newlines and stuff
        }

        [TestMethod]
        public void GetParsingData_multiline_handles_html_comments()
        {
            // Arrange
            var preParser = new StateMachinePreParser();
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
                    "        <!-- this is a comment -->",
                    "    </BODY>",
                    "</HTML>",
                };

            // Act
            var result = preParser.GetParsedSymbols(htmlLines);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count(x => x.ParsedDataType == ParsingDataType.Comment));
            Assert.AreEqual(26, result.Count); //This might need adjusting as I learn how to handle newlines and stuff
        }
    }
}

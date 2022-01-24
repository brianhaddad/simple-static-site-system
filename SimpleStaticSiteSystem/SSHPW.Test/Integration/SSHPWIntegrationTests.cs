using Microsoft.VisualStudio.TestTools.UnitTesting;
using SSHPW.Classes;
using SSHPW.Classes.Enums;
using System.Collections.Generic;

namespace SSHPW.Test.Integration
{
    [TestClass]
    public class SSHPWIntegrationTests
    {
        private SuperSimpleHtmlParserWriter _sshpw;

        [TestInitialize]
        public void Initialize()
        {
            var options = new HtmlStringificationOptions
            {
                IndentString = "    ",
                TagCaseBehavior = TagCaseOptions.UpperCase,
            };
            _sshpw = new SuperSimpleHtmlParserWriter(options);
        }

        [TestMethod]
        public void Parse_stringify_party_can_survive_multiple_executions()
        {
            // Arrange
            var numIterations = 100;
            var startingDocument = new string[]
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
            var latestDoc = CopyStringArray(startingDocument);

            // Act
            for (var i=0; i<numIterations; i++)
            {
                var doc = _sshpw.Parse(latestDoc);
                latestDoc = _sshpw.Stringify(doc);
            }

            // Assert
            CompareStringArrays(startingDocument, latestDoc);
        }

        private string[] CopyStringArray(string[] original)
        {
            var result = new List<string>();
            foreach (var item in original)
            {
                result.Add(item);
            }
            return result.ToArray();
        }

        private void CompareStringArrays(string[] expected, string[] actual)
        {
            Assert.AreEqual(expected.Length, actual.Length);
            for (var i=0; i<expected.Length; i++)
            {
                Assert.AreEqual(expected[i], actual[i]);
            }
        }
    }
}

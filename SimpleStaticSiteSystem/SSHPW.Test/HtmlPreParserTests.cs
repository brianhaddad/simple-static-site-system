using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            var testHtml = "<!doctype HTML>\r\n<html>\r\n <head>\r\n <title>My Test \r\n Page</title>\r\n </head>\r\n <body>\r\n <p class=\"test\">This is some <em>weird</em> text.</p>\r\n <hr />\r\n </body> \r\n</html>";

            // Act
            var result = preParser.GetParsingData(testHtml);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ContainsDocTypeDeclaration);
            Assert.AreEqual(1, result.DocTypeValues.Count);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(18, result.Data.Count); //This might need adjusting as I learn how to handle newlines
        }
    }
}

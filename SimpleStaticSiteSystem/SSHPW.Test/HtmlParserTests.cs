using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SSHPW.Test
{
    [TestClass]
    public class HtmlParserTests
    {
        private HtmlParser _parser;

        [TestInitialize]
        public void Setup()
        {
            _parser = new HtmlParser(new HtmlStringSanitizer());
        }

        [TestMethod]
        public void TestMethod1()
        {
            // Arrange

            // Act

            // Assert

        }
    }
}
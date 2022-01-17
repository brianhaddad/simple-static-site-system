using Microsoft.VisualStudio.TestTools.UnitTesting;
using SSHPW.Classes;
using System;

namespace SSHPW.Test
{
    [TestClass]
    public class HtmlHelpersTests
    {
        private ParsedHtmlNodeTree GetTestData()
        {
            var result = new ParsedHtmlNodeTree();

            return result;
        }

        [TestMethod]
        public void ParsedHtmlNodeTreeCanStringify()
        {
            // Arrange
            var data = GetTestData();

            // Act
            var result = data.Stringify();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(string));
        }
    }
}

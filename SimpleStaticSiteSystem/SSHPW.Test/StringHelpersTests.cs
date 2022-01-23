using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace SSHPW.Test
{
    [TestClass]
    public class StringHelpersTests
    {
        [TestMethod]
        public void ReplaceAll_replaces_all_instances_of_double_string()
        {
            // Arrange
            var testText = "          ";

            // Act
            var result = testText.ReplaceAll("  ", " ");

            // Assert
            Assert.AreEqual(" ", result);
        }

        [TestMethod]
        public void RegexReplace_works_on_all_new_lines()
        {
            // Arrange
            var regex = @"(\r\n|\n|\r)";
            var testText = "Test text \r testing text \r\ntest text" + Environment.NewLine + " testing the text \nwith texty text.";
            var expectedText = "Test text [nl] testing text [nl]test text[nl] testing the text [nl]with texty text.";

            // Act
            var result = testText.RegexReplace(regex, "[nl]");

            // Assert
            Assert.AreEqual(expectedText, result);
        }

        [TestMethod]
        public void Join_does_join_string_array_properly()
        {
            // Arrange
            var testTextArray = new[]
            {
                "one",
                "two",
                "three",
            };
            var expectedResult = "one-two-three";

            // Act
            var result = testTextArray.Join("-");

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void Join_does_join_list_properly()
        {
            // Arrange
            var testTextArray = new List<string>
            {
                "one",
                "two",
                "three",
            };
            var expectedResult = "one-two-three";

            // Act
            var result = testTextArray.Join("-");

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void BeginsWith_ignores_case()
        {
            // Arrange
            var testText = "<!doctype HTML><html></html>";
            var findText = "<!DOCTYPE html>";

            // Act
            var result = testText.BeginsWith(findText);

            // Assert
            Assert.IsTrue(result);
        }
    }
}

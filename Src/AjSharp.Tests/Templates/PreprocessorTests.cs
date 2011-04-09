namespace AjSharp.Tests.Templates
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using AjSharp.Templates;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class PreprocessorTests
    {
        [TestMethod]
        public void ProcessOneLine()
        {
            string input = "Hello, Template\r\n";
            Assert.AreEqual("PrintLine(\"Hello, Template\");\r\n", Process(input));
        }

        [TestMethod]
        public void ProcessTwoLines()
        {
            string input = "Hello\r\nTemplate\r\n";
            Assert.AreEqual("PrintLine(\"Hello\");\r\nPrintLine(\"Template\");\r\n", Process(input));
        }

        [TestMethod]
        public void ProcessCodeLine()
        {
            string input = "@int k;\r\n";
            Assert.AreEqual("int k;\r\n", Process(input));
        }

        [TestMethod]
        public void ProcessTwoCodeLines()
        {
            string input = "@int k;\r\n@int j;\r\n";
            Assert.AreEqual("int k;\r\nint j;\r\n", Process(input));
        }

        [TestMethod]
        public void ProcessSimpleFor()
        {
            string input = "@for (int k=1; k<=10; k++) { \r\nHello\r\n@}\r\n";
            Assert.AreEqual("for (int k=1; k<=10; k++) { \r\nPrintLine(\"Hello\");\r\n}\r\n", Process(input));
        }

        [TestMethod]
        public void ProcessMultiLine()
        {
            string input = "@{\r\nint k;\r\nint j;\r\n@}\r\n";
            Assert.AreEqual("int k;\r\nint j;\r\n", Process(input));
        }

        private static string Process(string input)
        {
            StringReader reader = new StringReader(input);
            StringWriter writer = new StringWriter();
            Preprocessor pproc = new Preprocessor(reader, writer);
            pproc.Process();
            reader.Close();
            writer.Close();
            return writer.ToString();
        }
    }
}

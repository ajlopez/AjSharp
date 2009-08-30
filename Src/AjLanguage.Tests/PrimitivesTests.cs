namespace AjLanguage.Tests
{
    using System;
    using System.IO;
    using System.Text;
    using System.Collections.Generic;
    using System.Linq;

    using AjLanguage;
    using AjLanguage.Primitives;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class PrimitivesTests
    {
        [TestMethod]
        public void WriteString()
        {
            PrintSubroutine print = new PrintSubroutine();
            Machine machine = new Machine();
            TextWriter sw = new StringWriter();
            machine.Out = sw;

            print.Invoke(machine.Environment, new object[] { "foo", "bar" });

            Assert.AreEqual("foobar", sw.ToString());
        }

        [TestMethod]
        public void WriteLineString()
        {
            PrintLineSubroutine print = new PrintLineSubroutine();
            Machine machine = new Machine();
            TextWriter sw = new StringWriter();
            machine.Out = sw;

            print.Invoke(machine.Environment, new object[] { "foo", "bar" });

            Assert.AreEqual("foobar\r\n", sw.ToString());
        }
    }
}

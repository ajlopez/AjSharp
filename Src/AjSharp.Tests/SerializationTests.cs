using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using AjLanguage.Primitives;
using AjSharp.Compiler;
using AjLanguage.Commands;
using AjLanguage.Expressions;
using AjLanguage.Language;

namespace AjSharp.Tests
{
    [TestClass]
    public class SerializationTests
    {
        private BinaryFormatter fmt;

        [TestInitialize]
        public void Setup()
        {
            this.fmt = new BinaryFormatter();
        }

        [TestMethod]
        public void SerializeDeserializeSimpleAddExpression()
        {
            IExpression expression = this.ProcessExpression("1+2");
            Assert.IsNotNull(expression);
            Assert.AreEqual(3, expression.Evaluate(null));
        }

        [TestMethod]
        public void SerializeDeserializeNewDynamicObjectExpression()
        {
            IExpression expression = this.ProcessExpression("new DynamicObject()");
            Assert.IsNotNull(expression);
        }

        [TestMethod]
        public void SerializeDeserializeInvokeExpression()
        {
            IExpression expression = this.ProcessExpression("foo.Bar(1,2)");
            Assert.IsNotNull(expression);
        }

        [TestMethod]
        public void SerializeDeserializePrintLineCommand()
        {
            ICommand command = this.ProcessCommand("PrintLine(\"Hello, world\");");
            Assert.IsNotNull(command);
        }

        [TestMethod]
        public void SerializeDeserializeSimpleAssignmentCommand()
        {
            ICommand command = this.ProcessCommand("a = \"foo\";");
            Assert.IsNotNull(command);
        }

        [TestMethod]
        public void SerializeDeserializeForCommand()
        {
            ICommand command = this.ProcessCommand("for (k=1; k<=5; k++) result = result+1;");
            Assert.IsNotNull(command);
            Assert.IsInstanceOfType(command, typeof(ForCommand));
        }

        [TestMethod]
        public void SerializeDeserializeFunction()
        {
            IExpression expression = this.GetExpression("function(x) { return x+1; }");
            Function result = (Function) this.SerializeDeserialize(expression.Evaluate(null));
            Assert.IsNotNull(result);
        }

        private byte[] Serialize(object obj)
        {
            MemoryStream stream = new MemoryStream();

            fmt.Serialize(stream, obj);
            stream.Close();

            return stream.ToArray();
        }

        private object Deserialize(byte[] data)
        {
            MemoryStream stream = new MemoryStream(data);

            return fmt.Deserialize(stream);
        }

        private object SerializeDeserialize(object obj)
        {
            return this.Deserialize(this.Serialize(obj));
        }

        private ICommand ProcessCommand(string commandtext)
        {
            Parser parser = new Parser(commandtext);
            ICommand command = parser.ParseCommand();
            return (ICommand)this.SerializeDeserialize(command);
        }

        private IExpression ProcessExpression(string expressiontext)
        {
            Parser parser = new Parser(expressiontext);
            IExpression expression = parser.ParseExpression();
            return (IExpression)this.SerializeDeserialize(expression);
        }

        private IExpression GetExpression(string expressiontext)
        {
            Parser parser = new Parser(expressiontext);
            return parser.ParseExpression();
        }
    }
}

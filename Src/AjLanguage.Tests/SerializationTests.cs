using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

using System.Runtime.Serialization.Formatters.Binary;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using AjLanguage.Expressions;
using AjLanguage.Language;

namespace AjLanguage.Tests
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
        public void SerializeDeserializeConstantExpression()
        {
            ConstantExpression expression = new ConstantExpression("foo");
            byte[] data = this.Serialize(expression);
            object result = this.Deserialize(data);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ConstantExpression));

            ConstantExpression expression2 = (ConstantExpression)result;
            Assert.AreEqual("foo", expression2.Evaluate(null));
        }

        [TestMethod]
        public void SerializeDeserializeVariableExpressions()
        {
            BindingEnvironment environment = new BindingEnvironment();

            environment.SetValue("foo", "bar");
            environment.SetValue("one", 1);

            VariableExpression varFoo = (VariableExpression) this.SerializeDeserialize(new VariableExpression("foo"));
            VariableExpression varOne = (VariableExpression) this.SerializeDeserialize(new VariableExpression("one"));

            Assert.AreEqual("bar", varFoo.Evaluate(environment));
            Assert.AreEqual(1, varOne.Evaluate(environment));
        }

        [TestMethod]
        public void CreateBinaryExpression()
        {
            IExpression leftExpression = new ConstantExpression(1);
            IExpression rightExpression = new ConstantExpression(2);
            BinaryExpression expression = (BinaryExpression) this.SerializeDeserialize(new ArithmeticBinaryExpression(ArithmeticOperator.Add, leftExpression, rightExpression));

            Assert.AreEqual(1, expression.LeftExpression.Evaluate(null));
            Assert.AreEqual(2, expression.RightExpression.Evaluate(null));
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
    }
}

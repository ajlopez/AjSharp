namespace AjSharp.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using AjLanguage.Commands;
    using AjLanguage.Expressions;
    using AjLanguage.Hosting;
    using AjLanguage.Language;
    using AjSharp.Compiler;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class HostEvaluationTest
    {
        private Host host;

        [TestInitialize]
        public void Setup()
        {
            this.host = new Host(new AjSharpMachine(false));
        }

        [TestMethod]
        public void EvaluateSimpleConstants()
        {
            object result = this.EvaluateExpression("\"foo\"");
            Assert.AreEqual("foo", result);
            result = this.EvaluateExpression("123");
            Assert.AreEqual(123, result);
        }

        [Ignore]
        [TestMethod]
        public void EvaluateNewDynamicObjectAsProxy()
        {
            object result = this.EvaluateExpression("new DynamicObject()");

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ObjectProxy));

            ObjectProxy proxy = (ObjectProxy)result;

            Assert.IsInstanceOfType(proxy.Object, typeof(DynamicObject));
        }

        [Ignore]
        [TestMethod]
        public void CreateAndSetDynamicObject()
        {
            this.ExecuteCommand("adam = new DynamicObject();");
            this.ExecuteCommand("adam.Name = \"Adam\";");
            this.ExecuteCommand("adam.Age = 800;");

            object result = this.EvaluateExpression("adam");

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ObjectProxy));

            ObjectProxy proxy = (ObjectProxy)result;

            Assert.IsInstanceOfType(proxy.Object, typeof(DynamicObject));

            DynamicObject dynobj = (DynamicObject)proxy.Object;

            Assert.AreEqual("Adam", dynobj.GetValue("Name"));
            Assert.AreEqual(800, dynobj.GetValue("Age"));
        }

        private object EvaluateExpression(string text)
        {
            Parser parser = new Parser(text);

            IExpression expression = parser.ParseExpression();

            return this.host.Evaluate(expression);
        }

        private void ExecuteCommand(string text)
        {
            Parser parser = new Parser(text);

            ICommand command = parser.ParseCommand();

            this.host.Execute(command);
        }
    }
}

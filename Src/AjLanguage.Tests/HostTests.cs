namespace AjLanguage.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using AjLanguage.Commands;
    using AjLanguage.Expressions;
    using AjLanguage.Hosting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class HostTests
    {
        private Host host;

        [TestInitialize]
        public void Setup()
        {
            this.host = new Host();
        }

        [TestMethod]
        public void EvaluateConstantExpression()
        {
            ConstantExpression expression = new ConstantExpression("foo");
            object result = this.host.Evaluate(expression);

            Assert.IsNotNull(result);
            Assert.AreEqual("foo", result);
        }

        [TestMethod]
        public void ExecuteSetCommand()
        {
            SetVariableCommand command = new SetVariableCommand("foo", new ConstantExpression("bar"));
            this.host.Execute(command);

            Assert.AreEqual("bar", this.host.Machine.Environment.GetValue("foo"));
        }
    }
}

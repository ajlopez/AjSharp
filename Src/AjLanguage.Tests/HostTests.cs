using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AjLanguage.Hosting;
using AjLanguage.Expressions;
using AjLanguage.Commands;

namespace AjLanguage.Tests
{
    [TestClass]
    public class HostTests
    {
        private Host host;

        [TestInitialize]
        public void Setup()
        {
            host = new Host();
        }

        [TestMethod]
        public void EvaluateConstantExpression()
        {
            ConstantExpression expression = new ConstantExpression("foo");
            object result = host.Evaluate(expression);

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

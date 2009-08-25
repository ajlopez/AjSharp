namespace AjLanguage.Tests
{
    using System;
    using System.Text;
    using System.Collections.Generic;
    using System.Linq;

    using AjLanguage.Commands;
    using AjLanguage.Expressions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class FunctionTests
    {
        [TestMethod]
        public void InvokeSimpleFunction()
        {
            ICommand body = new ReturnCommand(new VariableExpression("x"));
            Function function = new Function(new string[] { "x" }, body);

            BindingEnvironment environment = new BindingEnvironment();
            environment.SetValue("x", 2);

            object result = function.Invoke(environment, new object[] { 1 });

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result);
        }
    }
}

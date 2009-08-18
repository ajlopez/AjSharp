namespace AjLanguage.Tests
{
    using System;
    using System.Text;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using AjLanguage;
    using AjLanguage.Commands;
    using AjLanguage.Expressions;

    [TestClass]
    public class ExpressionsTests
    {
        [TestMethod]
        public void EvaluateSimpleConstantExpressions()
        {
            Assert.AreEqual("foo", (new ConstantExpression("foo")).Evaluate(null));
            Assert.AreEqual(1, (new ConstantExpression(1)).Evaluate(null));
            Assert.IsNull((new ConstantExpression(null)).Evaluate(null));
        }

        [TestMethod]
        public void EvaluateVariableExpressions()
        {
            BindingEnvironment environment = new BindingEnvironment();

            environment.SetValue("foo", "bar");
            environment.SetValue("one", 1);

            VariableExpression varFoo = new VariableExpression("foo");
            VariableExpression varOne = new VariableExpression("one");

            Assert.AreEqual("bar", varFoo.Evaluate(environment));
            Assert.AreEqual(1, varOne.Evaluate(environment));
        }

        [TestMethod]
        public void UndefinedVariableIsNull()
        {
            BindingEnvironment environment = new BindingEnvironment();
            VariableExpression var = new VariableExpression("foo");

            Assert.IsNull(var.Evaluate(environment));
        }
    }
}

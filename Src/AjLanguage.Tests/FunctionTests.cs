namespace AjLanguage.Tests
{
    using System;
    using System.Text;
    using System.Collections.Generic;
    using System.Linq;

    using AjLanguage.Commands;
    using AjLanguage.Expressions;
    using AjLanguage.Language;

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

        [TestMethod]
        public void EvaluateFactorialFunction()
        {
            BindingEnvironment environment = new BindingEnvironment();
            ICallable factorial = BuildFactorialFunction();
            environment.SetValue("Factorial", factorial);

            object result;

            result = factorial.Invoke(environment, new object[] { 3 });

            Assert.IsNotNull(result);
            Assert.AreEqual(6, result);

            result = factorial.Invoke(environment, new object[] { 4 });

            Assert.IsNotNull(result);
            Assert.AreEqual(24, result);

            result = factorial.Invoke(environment, new object[] { 5 });

            Assert.IsNotNull(result);
            Assert.AreEqual(120, result);
        }

        private static ICallable BuildFactorialFunction()
        {
            IExpression condition = new CompareExpression(ComparisonOperator.LessEqual, new VariableExpression("n"), new ConstantExpression(1));

            ICommand return1 = new ReturnCommand(new ConstantExpression(1));
            ICommand return2 = new ReturnCommand(new ArithmeticBinaryExpression(ArithmeticOperator.Multiply,
                new VariableExpression("n"),
                new InvokeExpression("Factorial", new IExpression[] { new ArithmeticBinaryExpression(ArithmeticOperator.Subtract, new VariableExpression("n"), new ConstantExpression(1)) })));

            ICommand ifcmd = new IfCommand(condition, return1, return2);
            ICallable factorial = new Function(new string[] { "n" }, ifcmd);

            return factorial;
        }
    }
}

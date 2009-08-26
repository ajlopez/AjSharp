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
    using AjLanguage.Language;

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

        [TestMethod]
        public void CreateBinaryExpression()
        {
            IExpression leftExpression = new ConstantExpression(1);
            IExpression rightExpression = new ConstantExpression(2);
            BinaryExpression expression = new ArithmeticBinaryExpression(ArithmeticOperator.Add, leftExpression, rightExpression);

            Assert.IsTrue(expression.LeftExpression == leftExpression);
            Assert.IsTrue(expression.RightExpression == rightExpression);
        }

        [TestMethod]
        public void CreateUnaryExpression()
        {
            IExpression valueExpression = new ConstantExpression(1);
            UnaryExpression expression = new ArithmeticUnaryExpression(ArithmeticOperator.Minus, valueExpression);

            Assert.IsTrue(expression.Expression == valueExpression);
        }

        [TestMethod]
        public void EvaluateAddOperation()
        {
            Assert.AreEqual(2, EvaluateArithmeticBinaryOperator(ArithmeticOperator.Add, 1, 1));
            Assert.AreEqual(2.4, EvaluateArithmeticBinaryOperator(ArithmeticOperator.Add, 1.2, 1.2));
        }

        [TestMethod]
        public void EvaluateSubtractOperation()
        {
            Assert.AreEqual(1, EvaluateArithmeticBinaryOperator(ArithmeticOperator.Subtract, 2, 1));
            Assert.AreEqual(2.2, EvaluateArithmeticBinaryOperator(ArithmeticOperator.Subtract, 3.4, 1.2));
        }

        [TestMethod]
        public void EvaluateMultiplyOperation()
        {
            Assert.AreEqual(6, EvaluateArithmeticBinaryOperator(ArithmeticOperator.Multiply, 2, 3));
            Assert.AreEqual(6.8, EvaluateArithmeticBinaryOperator(ArithmeticOperator.Multiply, 3.4, 2));
        }

        [TestMethod]
        public void EvaluateDivideOperation()
        {
            Assert.AreEqual(1.5, EvaluateArithmeticBinaryOperator(ArithmeticOperator.Divide, 3, 2));
            Assert.AreEqual(1.7, EvaluateArithmeticBinaryOperator(ArithmeticOperator.Divide, 3.4, 2));
        }

        [TestMethod]
        public void EvaluateMinusOperation()
        {
            Assert.AreEqual(-1, EvaluateArithmeticUnaryOperator(ArithmeticOperator.Minus, 1));
            Assert.AreEqual(-1.7, EvaluateArithmeticUnaryOperator(ArithmeticOperator.Minus, 1.7));
        }

        [TestMethod]
        public void EvaluatePlusOperation()
        {
            Assert.AreEqual(1, EvaluateArithmeticUnaryOperator(ArithmeticOperator.Plus, 1));
            Assert.AreEqual(-1.7, EvaluateArithmeticUnaryOperator(ArithmeticOperator.Plus, -1.7));
        }

        [TestMethod]
        public void EvaluateNotExpression()
        {
            Assert.IsTrue(EvaluateNotOperator(null));
            Assert.IsTrue(EvaluateNotOperator(false));
            Assert.IsTrue(EvaluateNotOperator(0));
            Assert.IsTrue(EvaluateNotOperator(string.Empty));

            Assert.IsFalse(EvaluateNotOperator(true));
            Assert.IsFalse(EvaluateNotOperator(-1));
            Assert.IsFalse(EvaluateNotOperator("foo"));
            Assert.IsFalse(EvaluateNotOperator(new BindingEnvironment()));
        }

        [TestMethod]
        public void EvaluateAndExpression()
        {
            Assert.IsTrue(EvaluateAndOperator(true, true));
            Assert.IsTrue(EvaluateAndOperator("foo", "bar"));
            Assert.IsTrue(EvaluateAndOperator(1, 2));
            Assert.IsTrue(EvaluateAndOperator(new BindingEnvironment(), true));

            Assert.IsFalse(EvaluateAndOperator(false, true));
            Assert.IsFalse(EvaluateAndOperator(null, true));
            Assert.IsFalse(EvaluateAndOperator(0, 1));
            Assert.IsFalse(EvaluateAndOperator(1, string.Empty));
        }

        [TestMethod]
        public void EvaluateOrExpression()
        {
            Assert.IsTrue(EvaluateOrOperator(true, true));
            Assert.IsTrue(EvaluateOrOperator(true, false));
            Assert.IsTrue(EvaluateOrOperator(false, true));
            Assert.IsTrue(EvaluateOrOperator(1, 0));
            Assert.IsTrue(EvaluateOrOperator(0, 1));

            Assert.IsFalse(EvaluateOrOperator(false, false));
            Assert.IsFalse(EvaluateOrOperator(null, string.Empty));
            Assert.IsFalse(EvaluateOrOperator(0, false));
        }

        [TestMethod]
        public void EvaluateEqualOperator()
        {
            Assert.IsTrue(EvaluateComparisonOperator(ComparisonOperator.Equal, null, null));
            Assert.IsTrue(EvaluateComparisonOperator(ComparisonOperator.Equal, true, true));
            Assert.IsTrue(EvaluateComparisonOperator(ComparisonOperator.Equal, false, false));
            Assert.IsTrue(EvaluateComparisonOperator(ComparisonOperator.Equal, 1, 1));
            Assert.IsTrue(EvaluateComparisonOperator(ComparisonOperator.Equal, 1.2, 1.0 + 0.2));
            Assert.IsTrue(EvaluateComparisonOperator(ComparisonOperator.Equal, "foo", "foo"));
            Assert.IsTrue(EvaluateComparisonOperator(ComparisonOperator.Equal, 2, "2"));
            Assert.IsTrue(EvaluateComparisonOperator(ComparisonOperator.Equal, "foo", "sfoo".Substring(1)));

            Assert.IsFalse(EvaluateComparisonOperator(ComparisonOperator.Equal, true, false));
            Assert.IsFalse(EvaluateComparisonOperator(ComparisonOperator.Equal, 2, 1));
            Assert.IsFalse(EvaluateComparisonOperator(ComparisonOperator.Equal, 3.14, 2.12));
            Assert.IsFalse(EvaluateComparisonOperator(ComparisonOperator.Equal, "foo", "bar"));
            Assert.IsFalse(EvaluateComparisonOperator(ComparisonOperator.Equal, "foo", "Foo"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void RaiseIfNotComparableObject()
        {
            Assert.IsFalse(EvaluateComparisonOperator(ComparisonOperator.Equal, new BindingEnvironment(), new BindingEnvironment()));
        }

        [TestMethod]
        public void EvaluateNotEqualOperator()
        {
            Assert.IsFalse(EvaluateComparisonOperator(ComparisonOperator.NotEqual, null, null));
            Assert.IsFalse(EvaluateComparisonOperator(ComparisonOperator.NotEqual, true, true));
            Assert.IsFalse(EvaluateComparisonOperator(ComparisonOperator.NotEqual, false, false));
            Assert.IsFalse(EvaluateComparisonOperator(ComparisonOperator.NotEqual, 1, 1));
            Assert.IsFalse(EvaluateComparisonOperator(ComparisonOperator.NotEqual, 1.2, 1.0 + 0.2));
            Assert.IsFalse(EvaluateComparisonOperator(ComparisonOperator.NotEqual, "foo", "foo"));
            Assert.IsFalse(EvaluateComparisonOperator(ComparisonOperator.NotEqual, 2, "2"));
            Assert.IsFalse(EvaluateComparisonOperator(ComparisonOperator.NotEqual, "foo", "sfoo".Substring(1)));

            Assert.IsTrue(EvaluateComparisonOperator(ComparisonOperator.NotEqual, true, false));
            Assert.IsTrue(EvaluateComparisonOperator(ComparisonOperator.NotEqual, 2, 1));
            Assert.IsTrue(EvaluateComparisonOperator(ComparisonOperator.NotEqual, 3.14, 2.12));
            Assert.IsTrue(EvaluateComparisonOperator(ComparisonOperator.NotEqual, "foo", "bar"));
            Assert.IsTrue(EvaluateComparisonOperator(ComparisonOperator.NotEqual, "foo", "Foo"));
        }

        [TestMethod]
        public void EvaluateLessOperator()
        {
            Assert.IsTrue(EvaluateComparisonOperator(ComparisonOperator.Less, 1, 2));
            Assert.IsTrue(EvaluateComparisonOperator(ComparisonOperator.Less, "bar", "foo"));
            Assert.IsTrue(EvaluateComparisonOperator(ComparisonOperator.Less, 1.2, 3.4));

            Assert.IsFalse(EvaluateComparisonOperator(ComparisonOperator.Less, 1, 1));
            Assert.IsFalse(EvaluateComparisonOperator(ComparisonOperator.Less, 2, 1));
            Assert.IsFalse(EvaluateComparisonOperator(ComparisonOperator.Less, null, null));
            Assert.IsFalse(EvaluateComparisonOperator(ComparisonOperator.Less, "foo", "foo"));
            Assert.IsFalse(EvaluateComparisonOperator(ComparisonOperator.Less, "foo", "bar"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void RaiseIfNotComparableObjectInLess()
        {
            Assert.IsFalse(EvaluateComparisonOperator(ComparisonOperator.Less, new BindingEnvironment(), new BindingEnvironment()));
        }

        [TestMethod]
        public void EvaluateLessEqualOperator()
        {
            Assert.IsTrue(EvaluateComparisonOperator(ComparisonOperator.LessEqual, 1, 2));
            Assert.IsTrue(EvaluateComparisonOperator(ComparisonOperator.LessEqual, "bar", "foo"));
            Assert.IsTrue(EvaluateComparisonOperator(ComparisonOperator.LessEqual, 1.2, 3.4));
            Assert.IsTrue(EvaluateComparisonOperator(ComparisonOperator.LessEqual, 1, 1));
            Assert.IsTrue(EvaluateComparisonOperator(ComparisonOperator.LessEqual, null, null));
            Assert.IsTrue(EvaluateComparisonOperator(ComparisonOperator.LessEqual, "foo", "foo"));

            Assert.IsFalse(EvaluateComparisonOperator(ComparisonOperator.LessEqual, 2, 1));
            Assert.IsFalse(EvaluateComparisonOperator(ComparisonOperator.LessEqual, "foo", "bar"));
        }

        [TestMethod]
        public void EvaluateGreaterOperator()
        {
            Assert.IsTrue(EvaluateComparisonOperator(ComparisonOperator.Greater, 2, 1));
            Assert.IsTrue(EvaluateComparisonOperator(ComparisonOperator.Greater, "foo", "bar"));
            Assert.IsTrue(EvaluateComparisonOperator(ComparisonOperator.Greater, 3.14, 2.12));
            Assert.IsTrue(EvaluateComparisonOperator(ComparisonOperator.Greater, "3", 2));

            Assert.IsFalse(EvaluateComparisonOperator(ComparisonOperator.Greater, 2, 2));
            Assert.IsFalse(EvaluateComparisonOperator(ComparisonOperator.Greater, 1, 2));
            Assert.IsFalse(EvaluateComparisonOperator(ComparisonOperator.Greater, "foo", "foo"));
            Assert.IsFalse(EvaluateComparisonOperator(ComparisonOperator.Greater, "bar", "foo"));
            Assert.IsFalse(EvaluateComparisonOperator(ComparisonOperator.Greater, 2, "3"));
        }

        [TestMethod]
        public void EvaluateGreaterEqualOperator()
        {
            Assert.IsTrue(EvaluateComparisonOperator(ComparisonOperator.GreaterEqual, 2, 1));
            Assert.IsTrue(EvaluateComparisonOperator(ComparisonOperator.GreaterEqual, "foo", "bar"));
            Assert.IsTrue(EvaluateComparisonOperator(ComparisonOperator.GreaterEqual, 3.14, 2.12));
            Assert.IsTrue(EvaluateComparisonOperator(ComparisonOperator.GreaterEqual, "3", 2));
            Assert.IsTrue(EvaluateComparisonOperator(ComparisonOperator.GreaterEqual, 2, 2));
            Assert.IsTrue(EvaluateComparisonOperator(ComparisonOperator.GreaterEqual, "foo", "foo"));

            Assert.IsFalse(EvaluateComparisonOperator(ComparisonOperator.GreaterEqual, 1, 2));
            Assert.IsFalse(EvaluateComparisonOperator(ComparisonOperator.GreaterEqual, "bar", "foo"));
            Assert.IsFalse(EvaluateComparisonOperator(ComparisonOperator.GreaterEqual, 2, "3"));
        }
        
        [TestMethod]
        public void EvaluateInvokeExpression()
        {
            ICommand body = new ReturnCommand(new VariableExpression("x"));
            Function function = new Function(new string[] { "x" }, body);

            BindingEnvironment environment = new BindingEnvironment();
            environment.SetValue("foo", function);

            IExpression expression = new InvokeExpression("foo", new IExpression[] { new ConstantExpression(1) });

            object result = expression.Evaluate(environment);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result);
        }

        private static object EvaluateArithmeticBinaryOperator(ArithmeticOperator operation, object left, object right)
        {
            IExpression expression = new ArithmeticBinaryExpression(operation, new ConstantExpression(left), new ConstantExpression(right));

            return expression.Evaluate(null);
        }

        private static object EvaluateArithmeticUnaryOperator(ArithmeticOperator operation, object value)
        {
            IExpression expression = new ArithmeticUnaryExpression(operation, new ConstantExpression(value));

            return expression.Evaluate(null);
        }

        private static bool EvaluateNotOperator(object value)
        {
            IExpression expression = new NotExpression(new ConstantExpression(value));

            return (bool) expression.Evaluate(null);
        }

        private static bool EvaluateAndOperator(object left, object right)
        {
            IExpression expression = new AndExpression(new ConstantExpression(left), new ConstantExpression(right));

            return (bool)expression.Evaluate(null);
        }

        private static bool EvaluateOrOperator(object left, object right)
        {
            IExpression expression = new OrExpression(new ConstantExpression(left), new ConstantExpression(right));

            return (bool)expression.Evaluate(null);
        }

        private static bool EvaluateComparisonOperator(ComparisonOperator operation, object left, object right)
        {
            IExpression expression = new CompareExpression(operation, new ConstantExpression(left), new ConstantExpression(right));

            return (bool) expression.Evaluate(null);
        }

        private static ICallable BuildFactorialFunction()
        {
            IExpression condition = new CompareExpression(ComparisonOperator.LessEqual, new VariableExpression("n"), new ConstantExpression(1));

            ICommand return1 = new ReturnCommand(new ConstantExpression(1));
            ICommand return2 = new ReturnCommand(new ArithmeticBinaryExpression(ArithmeticOperator.Multiply, 
                new VariableExpression("n"),
                new InvokeExpression("factorial", new IExpression[] { new ArithmeticBinaryExpression(ArithmeticOperator.Subtract, new VariableExpression("n"), new ConstantExpression(1)) })));

            ICommand ifcmd = new IfCommand(condition, return1, return2);
            ICallable factorial = new Function(new string[] { "n" }, ifcmd);

            return factorial;
        }
    }
}

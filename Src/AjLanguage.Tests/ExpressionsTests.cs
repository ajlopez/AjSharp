namespace AjLanguage.Tests
{
    using System;
    using System.IO;
    using System.Text;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using AjLanguage;
    using AjLanguage.Commands;
    using AjLanguage.Expressions;
    using AjLanguage.Language;
    using AjLanguage.Hosting;

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
        public void EvaluateModOperation()
        {
            Assert.AreEqual(1, EvaluateArithmeticBinaryOperator(ArithmeticOperator.Modulo, 3, 2));
            Assert.AreEqual(0, EvaluateArithmeticBinaryOperator(ArithmeticOperator.Modulo, 6, 3));
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

        [TestMethod]
        public void EvaluateDotExpressionOnInteger()
        {
            IExpression expression = new DotExpression(new ConstantExpression(1), "ToString", new List<IExpression>());

            Assert.AreEqual("1", expression.Evaluate(null));
        }

        [TestMethod]
        public void EvaluateDotExpressionOnString()
        {
            IExpression expression = new DotExpression(new ConstantExpression("foo"), "Length");

            Assert.AreEqual(3, expression.Evaluate(null));
        }

        [TestMethod]
        public void EvaluateDotExpressionAsTypeInvocation()
        {
            DotExpression dot = new DotExpression(new DotExpression(new DotExpression(new VariableExpression("System"), "IO"), "File"), "Exists", new IExpression[] { new ConstantExpression("unknown.txt") });

            Assert.IsFalse((bool) dot.Evaluate(new BindingEnvironment()));
        }

        [TestMethod]
        public void EvaluateSimpleNewExpression()
        {
            IExpression expression = new NewExpression("System.Data.DataSet", null);

            object result = expression.Evaluate(new BindingEnvironment());

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(System.Data.DataSet));
        }

        [TestMethod]
        public void EvaluateNewExpressionWithArguments()
        {
            IExpression expression = new NewExpression("System.IO.DirectoryInfo", new IExpression[] { new ConstantExpression(".") });

            object result = expression.Evaluate(new BindingEnvironment());

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(System.IO.DirectoryInfo));

            DirectoryInfo di = (DirectoryInfo)result;

            DirectoryInfo current = new DirectoryInfo(".");

            Assert.AreEqual(current.FullName, di.FullName);
        }

        [TestMethod]
        public void EvaluateNewExpressionWithAliasedType()
        {
            IExpression expression = new NewExpression("Channel", null);
            BindingEnvironment environment = new BindingEnvironment();
            environment.SetValue("Channel", typeof(Channel));

            object result = expression.Evaluate(environment);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(Channel));
        }

        [TestMethod]
        public void EvaluateFunctionExpression()
        {
            IExpression expression = new FunctionExpression(new string[] { "x" }, new ReturnCommand(new VariableExpression("x")));

            object result = expression.Evaluate(new BindingEnvironment());

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(Function));

            Function function = (Function)result;

            Assert.AreEqual(1, function.ParameterNames.Length);
            Assert.AreEqual("x", function.ParameterNames[0]);
            Assert.IsNotNull(function.Body);
            Assert.IsInstanceOfType(function.Body, typeof(ReturnCommand));
            Assert.IsFalse(function.IsDefault);
        }

        [TestMethod]
        public void EvaluateMultipleSetExpressionUsingDynamicObject()
        {
            DynamicObject dynobj = new DynamicObject();

            MultipleSetExpression expression = new MultipleSetExpression(new ConstantExpression(dynobj), new string[] { "FirstName", "LastName" }, new IExpression[] { new ConstantExpression("John"), new ConstantExpression("Doe") });

            object result = expression.Evaluate(new BindingEnvironment());

            Assert.IsNotNull(result);
            Assert.IsTrue(result == dynobj);

            Assert.AreEqual("John", dynobj.GetValue("FirstName"));
            Assert.AreEqual("Doe", dynobj.GetValue("LastName"));
        }

        [TestMethod]
        public void EvaluateSimpleNewArrayExpression()
        {
            IExpression expression = new NewArrayExpression("System.Int32", new IExpression[] { new ConstantExpression(10) });

            object result = expression.Evaluate(new BindingEnvironment());

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(int[]));

            int[] array = (int[])result;

            Assert.AreEqual(10, array.Length);
        }

        [TestMethod]
        public void EvaluateSimpleInitializeArrayExpression()
        {
            IExpression expression = new InitializeArrayExpression("System.Int32", new IExpression[] { new ConstantExpression(1), new ConstantExpression(2) });

            object result = expression.Evaluate(new BindingEnvironment());

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(int[]));

            int[] array = (int[])result;

            Assert.AreEqual(2, array.Length);
            Assert.AreEqual(1, array[0]);
            Assert.AreEqual(2, array[1]);
        }

        [TestMethod]
        public void EvaluateSimpleNewIClassicObjectArrayExpression()
        {
            BindingEnvironment environment = new BindingEnvironment();

            environment.SetValue("ADynamicClass", new DynamicClass("ADynamicClass"));

            IExpression expression = new NewArrayExpression("ADynamicClass", new IExpression[] { new ConstantExpression(10) });

            object result = expression.Evaluate(environment);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(IClassicObject[]));

            IClassicObject[] array = (IClassicObject[])result;

            Assert.AreEqual(10, array.Length);
        }

        [TestMethod]
        public void EvaluateArrayVariableExpression()
        {
            BindingEnvironment environment = new BindingEnvironment();

            environment.SetValue("array", new string[] { "one" , "two", "three" });

            IExpression expression = new ArrayExpression(new VariableExpression("array"), new IExpression[] { new ConstantExpression(1) });

            object result = expression.Evaluate(environment);

            Assert.IsNotNull(result);
            Assert.AreEqual("two", result);
        }

        [TestMethod]
        public void EvaluateArrayDotExpression()
        {
            BindingEnvironment environment = new BindingEnvironment();

            DynamicObject data = new DynamicObject();
            data.SetValue("Numbers", new string[] { "one", "two", "three" });

            environment.SetValue("data", data);

            IExpression expression = new ArrayExpression(new DotExpression(new VariableExpression("data"), "Numbers"), new IExpression[] { new ConstantExpression(1) });

            object result = expression.Evaluate(environment);

            Assert.IsNotNull(result);
            Assert.AreEqual("two", result);
        }

        [TestMethod]
        public void EvaluateConcatenateExpressionUsingNulls()
        {
            IExpression expression = new ConcatenateExpression(new ConstantExpression(null), new ConstantExpression(null));

            Assert.AreEqual(string.Empty, expression.Evaluate(null));

            expression = new ConcatenateExpression(new ConstantExpression("foo"), new ConstantExpression(null));

            Assert.AreEqual("foo", expression.Evaluate(null));

            expression = new ConcatenateExpression(new ConstantExpression(null), new ConstantExpression("bar"));

            Assert.AreEqual("bar", expression.Evaluate(null));
        }

        [TestMethod]
        public void EvaluateConcatenateExpressionUsingStrings()
        {
            IExpression expression = new ConcatenateExpression(new ConstantExpression("foo"), new ConstantExpression("bar"));

            Assert.AreEqual("foobar", expression.Evaluate(null));
        }

        [TestMethod]
        public void EvaluateConcatenateExpressionUsingIntegers()
        {
            IExpression expression = new ConcatenateExpression(new ConstantExpression(12), new ConstantExpression(34));

            Assert.AreEqual("1234", expression.Evaluate(null));
        }

        [TestMethod]
        public void EvaluateHostedExpression()
        {
            Host host = new Host(new Machine());
            Host host2 = new Host(new Machine(false));
            host.Machine.Environment.SetValue("host", host2);
            host2.Machine.Environment.SetValue("foo", "bar");
            HostedExpression expression = new HostedExpression(new VariableExpression("foo"),new VariableExpression("host"));

            object result = expression.Evaluate(host.Machine.Environment);

            Assert.AreSame(Machine.Current, host.Machine);
            Assert.AreEqual("bar", result);
            Assert.IsNull(host.Machine.Environment.GetValue("foo"));
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

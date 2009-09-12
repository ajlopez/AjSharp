namespace AjSharp.Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using AjLanguage;
    using AjLanguage.Commands;
    using AjLanguage.Expressions;
    using AjLanguage.Language;

    using AjSharp;
    using AjSharp.Compiler;
    using AjSharp.Primitives;
    using AjSharp.Tests.Classes;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class EvaluationTests
    {
        private Machine machine;

        [TestInitialize]
        public void SetupMachine()
        {
            machine = new Machine();
        }

        [TestMethod]
        public void EvaluateConstants()
        {
            Assert.AreEqual(1, this.EvaluateExpression("1"));
            Assert.AreEqual(3.14, this.EvaluateExpression("3.14"));
            Assert.AreEqual("foo", this.EvaluateExpression("\"foo\""));
        }

        [TestMethod]
        public void EvaluateSimpleArithmeticExpressions()
        {
            Assert.AreEqual(3, this.EvaluateExpression("1+2"));
            Assert.AreEqual(6, this.EvaluateExpression("2*3"));
            Assert.AreEqual(4, this.EvaluateExpression("5-1"));
            Assert.AreEqual(6.0 / 3.0, this.EvaluateExpression("6/3"));
            Assert.AreEqual(6.0 / 4.0, this.EvaluateExpression("6/4"));
        }

        [TestMethod]
        public void EvaluateSimpleComparisonExpressions()
        {
            Assert.IsTrue((bool)this.EvaluateExpression("1<2"));
            Assert.IsTrue((bool)this.EvaluateExpression("2<=2"));
            Assert.IsTrue((bool)this.EvaluateExpression("2==2"));
            Assert.IsTrue((bool)this.EvaluateExpression("2!=3"));
            Assert.IsTrue((bool)this.EvaluateExpression("2>1"));
            Assert.IsTrue((bool)this.EvaluateExpression("2>=1"));
        }

        [TestMethod]
        public void ExecuteSimpleAssign()
        {
            this.ExecuteCommand("x = 1;");

            Assert.AreEqual(1, this.machine.Environment.GetValue("x"));
        }

        [TestMethod]
        public void ExecuteAssignWithPropertyExpression()
        {
            this.ExecuteCommand("x = \"foo\".Length;");

            Assert.AreEqual(3, this.machine.Environment.GetValue("x"));
        }

        [TestMethod]
        public void ExecuteAssignWithCallExpression()
        {
            this.ExecuteCommand("x = \"foo\".Substring(1);");

            Assert.AreEqual("oo", this.machine.Environment.GetValue("x"));
        }

        [TestMethod]
        public void ExecuteSimpleIf()
        {
            this.ExecuteCommand("if (x) x=1; else x=2;");

            Assert.AreEqual(2, this.machine.Environment.GetValue("x"));
        }

        [TestMethod]
        public void ExecuteSimpleWhile()
        {
            this.ExecuteCommand("x = 0;");
            this.ExecuteCommand("while (x<6) x=x+1;");

            Assert.AreEqual(6, this.machine.Environment.GetValue("x"));
        }

        [TestMethod]
        public void EvaluateSimpleForEach()
        {
            this.machine.Environment.SetValue("numbers", new int[] { 1, 2, 3 });
            this.ExecuteCommand("sum = 0;");

            this.ExecuteCommand("foreach (number in numbers) sum = sum + number;");

            Assert.AreEqual(6, this.machine.Environment.GetValue("sum"));
        }

        [TestMethod]
        public void EvaluateAddNumbersFunction()
        {
            this.machine.Environment.SetValue("numbers", new int[] { 1, 2, 3 });
            this.ExecuteCommand("function AddNumbers(values) { sum = 0; foreach (value in values) sum = sum + value; return sum;}");

            Assert.AreEqual(6, this.EvaluateExpression("AddNumbers(numbers)"));
        }

        [TestMethod]
        public void EvaluateApplyFunction()
        {
            this.machine.Environment.SetValue("numbers", new int[] { 1, 2, 3 });
            this.ExecuteCommand("function Apply(func, values) { sum = 0; foreach (value in values) sum = sum + func(value); return sum;}");

            Assert.AreEqual(9, this.EvaluateExpression("Apply(function (n) { return n+1; }, numbers)"));
        }

        [TestMethod]
        public void EvaluateApplyFunctionWithAdd()
        {
            this.machine.Environment.SetValue("numbers", new int[] { 1, 2, 3 });
            this.ExecuteCommand("function Add(x,y) return x+y;");
            this.ExecuteCommand("function Apply(func, values) { sum = 0; foreach (value in values) sum = Add(sum, value+1); return sum;}");

            Assert.AreEqual(9, this.EvaluateExpression("Apply(function (n) { return n+1; }, numbers)"));
        }

        [TestMethod]
        public void EvaluateFactorial()
        {
            this.ExecuteCommand("function Factorial(n)\r\n{\r\n if (n<=1)\r\n return 1;\r\n else\r\n return n * Factorial(n-1);\r\n}\r\n");

            Assert.AreEqual(1, this.EvaluateExpression("Factorial(0)"));
            Assert.AreEqual(1, this.EvaluateExpression("Factorial(1)"));
            Assert.AreEqual(2, this.EvaluateExpression("Factorial(2)"));
            Assert.AreEqual(6, this.EvaluateExpression("Factorial(3)"));
            Assert.AreEqual(24, this.EvaluateExpression("Factorial(4)"));
            Assert.AreEqual(120, this.EvaluateExpression("Factorial(5)"));
        }

        [TestMethod]
        [DeploymentItem("Examples\\Factorial.ajs")]
        public void EvaluateFactorialUsingInclude()
        {
            IncludeFile("Factorial.ajs");

            Assert.AreEqual(1, this.EvaluateExpression("Factorial(0)"));
            Assert.AreEqual(1, this.EvaluateExpression("Factorial(1)"));
            Assert.AreEqual(2, this.EvaluateExpression("Factorial(2)"));
            Assert.AreEqual(6, this.EvaluateExpression("Factorial(3)"));
            Assert.AreEqual(24, this.EvaluateExpression("Factorial(4)"));
            Assert.AreEqual(120, this.EvaluateExpression("Factorial(5)"));
        }

        [TestMethod]
        [DeploymentItem("Examples\\SetNumbers.ajs")]
        public void SetNumbersUsingInclude()
        {
            IncludeFile("SetNumbers.ajs");

            Assert.AreEqual(1, this.machine.Environment.GetValue("one"));
            Assert.AreEqual(2, this.machine.Environment.GetValue("two"));
            Assert.AreEqual(3, this.machine.Environment.GetValue("three"));
            Assert.AreEqual(4, this.machine.Environment.GetValue("four"));
        }

        [TestMethod]
        [DeploymentItem("Examples\\Apply.ajs")]
        public void ApplySquareToNumbersUsingInclude()
        {
            IncludeFile("Apply.ajs");

            object result = this.machine.Environment.GetValue("squared");

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ArrayList));

            ArrayList squared = (ArrayList)result;

            Assert.AreEqual(3, squared.Count);
            Assert.AreEqual(1, squared[0]);
            Assert.AreEqual(4, squared[1]);
            Assert.AreEqual(9, squared[2]);

            result = this.machine.Environment.GetValue("squared2");

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ArrayList));

            squared = (ArrayList)result;

            Assert.AreEqual(3, squared.Count);
            Assert.AreEqual(1, squared[0]);
            Assert.AreEqual(4, squared[1]);
            Assert.AreEqual(9, squared[2]);
        }

        [TestMethod]
        public void EvaluateFunctionExpression()
        {
            object result = this.EvaluateExpression("function (n) { return n*n; }");

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(Function));

            Function function = (Function)result;

            Assert.AreEqual(4, function.Invoke(new BindingEnvironment(), new object[] { 2 }));
        }

        [TestMethod]
        public void InvokeFunctionExpression()
        {
            this.ExecuteCommand("func = function (n) { return n*n; };");
            Assert.AreEqual(4, this.EvaluateExpression("func(2)"));
        }

        [TestMethod]
        public void EvaluateNewExpression()
        {
            object result = this.EvaluateExpression("new AjSharp.Tests.Classes.Person()");

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(Person));

            Person person = (Person)result;

            Assert.IsNull(person.Name);
            Assert.AreEqual(0, person.Age);
        }

        [TestMethod]
        public void EvaluateNewExpressionWithArguments()
        {
            object result = this.EvaluateExpression("new AjSharp.Tests.Classes.Person(\"Adam\", 800)");

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(Person));

            Person person = (Person)result;

            Assert.AreEqual("Adam", person.Name);
            Assert.AreEqual(800, person.Age);
        }

        [TestMethod]
        public void EvaluateNewExpressionWithPropertySetting()
        {
            object result = this.EvaluateExpression("new AjSharp.Tests.Classes.Person() { Name = \"Adam\", Age = 800 }");

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(Person));

            Person person = (Person)result;

            Assert.AreEqual("Adam", person.Name);
            Assert.AreEqual(800, person.Age);
        }

        [TestMethod]
        public void EvaluateNewExpressionWithDynamicObject()
        {
            object result = this.EvaluateExpression("new { Name = \"Adam\", Age = 800 }");

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(DynamicObject));

            DynamicObject dynobj = (DynamicObject)result;

            Assert.AreEqual("Adam", dynobj.GetValue("Name"));
            Assert.AreEqual(800, dynobj.GetValue("Age"));
        }

        [TestMethod]
        public void EvaluateConcatExpressionWithTwoStrings()
        {
            object result = this.EvaluateExpression("\"foo\" + \"bar\"");

            Assert.AreEqual("foobar", result);
        }

        [TestMethod]
        public void EvaluateConcatExpressionWithStringAndInteger()
        {
            object result = this.EvaluateExpression("\"foo\" + 4");

            Assert.AreEqual("foo4", result);
        }

        private object EvaluateExpression(string text)
        {
            Parser parser = new Parser(text);

            IExpression expression = parser.ParseExpression();

            return expression.Evaluate(this.machine.Environment);
        }

        private void ExecuteCommand(string text)
        {
            Parser parser = new Parser(text);

            ICommand command = parser.ParseCommand();

            command.Execute(this.machine.Environment);
        }

        private void IncludeFile(string filename)
        {
            this.machine.Environment.SetValue("Include", new IncludeSubroutine());

            this.ExecuteCommand(string.Format("Include(\"{0}\");", filename));
        }
    }
}

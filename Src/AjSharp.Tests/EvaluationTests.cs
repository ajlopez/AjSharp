﻿namespace AjSharp.Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using AjLanguage;
    using AjLanguage.Commands;
    using AjLanguage.Expressions;
    using AjLanguage.Hosting;
    using AjLanguage.Language;
    using AjSharp;
    using AjSharp.Compiler;
    using AjSharp.Hosting;
    using AjSharp.Primitives;
    using AjSharp.Tests.Classes;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class EvaluationTests
    {
        private AjSharpMachine machine;

        [TestInitialize]
        public void SetupMachine()
        {
            this.machine = new AjSharpMachine();
            new AjSharp.Hosting.Host(this.machine);
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
        public void EvaluateSimpleArithmeticExpressionWithUndefinedVariables()
        {
            Assert.AreEqual(1, this.EvaluateExpression("1+foo"));
            Assert.AreEqual(0, this.EvaluateExpression("foo*3"));
            Assert.AreEqual(5, this.EvaluateExpression("5-foo"));
            Assert.AreEqual(0.0, this.EvaluateExpression("foo/3"));
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
            this.ExecuteCommand("function Apply(func, values) { sum = 0; foreach (value in values) sum = func(sum, value+1); return sum;}");

            Assert.AreEqual(9, this.EvaluateExpression("Apply(Add, numbers)"));
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
        public void EvaluateAnonymousFunction()
        {
            object result = this.EvaluateExpression("function(x) { return x+1; }(1)");

            Assert.AreEqual(2, result);
        }

        [TestMethod]
        public void ExecuteAnonymousSubroutine()
        {
            this.EvaluateExpression("sub(x) { global GlobalValue; GlobalValue = x+1; } (1)");
            Assert.AreEqual(2, this.machine.Environment.GetValue("GlobalValue"));
        }

        [TestMethod]
        public void EvaluateEnumValue()
        {
            Assert.AreEqual(System.UriKind.Relative, this.EvaluateExpression("System.UriKind.Relative"));
        }

        [TestMethod]
        public void EvaluateVariableArgumentsFunction()
        {
            this.ExecuteCommand("function CountParameters(pars...) { return pars.Length; }");

            object obj = this.machine.Environment.GetValue("CountParameters");

            Assert.IsNotNull(obj);
            Assert.IsInstanceOfType(obj, typeof(Function));

            Function func = (Function)obj;

            Assert.AreEqual(1, func.Arity);
            Assert.IsTrue(func.HasVariableParameters);

            Assert.AreEqual(0, this.EvaluateExpression("CountParameters()"));
            Assert.AreEqual(1, this.EvaluateExpression("CountParameters(10)"));
            Assert.AreEqual(2, this.EvaluateExpression("CountParameters(10,20)"));
        }

        [TestMethod]
        public void EvaluatePassingVariableArguments()
        {
            this.ExecuteCommand("function AddParameters(pars...) { var result = 0; foreach (par in pars) result = result + par; return result; }");

            object obj = this.machine.Environment.GetValue("AddParameters");

            Assert.IsNotNull(obj);
            Assert.IsInstanceOfType(obj, typeof(Function));

            Function func = (Function)obj;

            Assert.AreEqual(1, func.Arity);
            Assert.IsTrue(func.HasVariableParameters);

            Assert.AreEqual(0, this.EvaluateExpression("AddParameters()"));
            Assert.AreEqual(10, this.EvaluateExpression("AddParameters(10)"));
            Assert.AreEqual(30, this.EvaluateExpression("AddParameters(10,20)"));
        }

        [TestMethod]
        public void EvaluatePassingVariableArgumentsUsingSplats()
        {
            this.ExecuteCommand("function AddParameters(pars...) { var result = 0; foreach (par in pars) result = result + par; return result; }");
            this.ExecuteCommand("pars = new int[] { 1, 2, 3 };");

            Assert.AreEqual(6, this.EvaluateExpression("AddParameters(pars...)"));
            Assert.AreEqual(16, this.EvaluateExpression("AddParameters(10, pars...)"));
            Assert.AreEqual(26, this.EvaluateExpression("AddParameters(pars...,20)"));
            Assert.AreEqual(3, this.EvaluateExpression("AddParameters(1,foo...,2)"));
        }

        [TestMethod]
        public void DefineObjectAndGetFunctionValue()
        {
            this.ExecuteCommand("object Foo { function Increment(n) { return n+1; } }");

            object f = this.EvaluateExpression("Foo.Increment");
            Assert.IsNotNull(f);
            Assert.IsInstanceOfType(f, typeof(Function));
        }

        [TestMethod]
        public void DefineObjectAndGetValuesUsingArrayNotation()
        {
            this.ExecuteCommand("object Foo { var Age = 800; function Increment(n) { return n+1; } }");

            object f = this.EvaluateExpression("Foo[\"Increment\"]");
            Assert.IsNotNull(f);
            Assert.IsInstanceOfType(f, typeof(Function));

            Assert.AreEqual(800, this.EvaluateExpression("Foo[\"Age\"]"));
        }

        [TestMethod]
        public void DefineObjectAndInvokeSubUsingArrayNotation()
        {
            this.ExecuteCommand("object Foo { var Age = 700; sub AddYears(n) { this.Age = this.Age + n; } }");

            Assert.AreEqual(700, this.EvaluateExpression("Foo[\"Age\"]"));
            this.EvaluateExpression("Foo[\"AddYears\"](100)");
            Assert.AreEqual(800, this.EvaluateExpression("Foo[\"Age\"]"));
        }

        [TestMethod]
        [DeploymentItem("Examples\\Factorial.ajs")]
        public void EvaluateFactorialUsingInclude()
        {
            this.IncludeFile("Factorial.ajs");

            Assert.AreEqual(1, this.EvaluateExpression("Factorial(0)"));
            Assert.AreEqual(1, this.EvaluateExpression("Factorial(1)"));
            Assert.AreEqual(2, this.EvaluateExpression("Factorial(2)"));
            Assert.AreEqual(6, this.EvaluateExpression("Factorial(3)"));
            Assert.AreEqual(24, this.EvaluateExpression("Factorial(4)"));
            Assert.AreEqual(120, this.EvaluateExpression("Factorial(5)"));
        }

        [TestMethod]
        [DeploymentItem("Examples\\TwoFactorials.ajs")]
        public void EvaluateFactorialUsingTwoFunctions()
        {
            this.IncludeFile("TwoFactorials.ajs");

            Assert.AreEqual(1, this.EvaluateExpression("FactorialOne(0)"));
            Assert.AreEqual(1, this.EvaluateExpression("FactorialTwo(1)"));
            Assert.AreEqual(2, this.EvaluateExpression("FactorialOne(2)"));
            Assert.AreEqual(6, this.EvaluateExpression("FactorialTwo(3)"));
            Assert.AreEqual(24, this.EvaluateExpression("FactorialOne(4)"));
            Assert.AreEqual(120, this.EvaluateExpression("FactorialTwo(5)"));
        }

        [TestMethod]
        [DeploymentItem("Examples\\SetNumbers.ajs")]
        public void SetNumbersUsingInclude()
        {
            this.IncludeFile("SetNumbers.ajs");

            Assert.AreEqual(1, this.machine.Environment.GetValue("one"));
            Assert.AreEqual(2, this.machine.Environment.GetValue("two"));
            Assert.AreEqual(3, this.machine.Environment.GetValue("three"));
            Assert.AreEqual(4, this.machine.Environment.GetValue("four"));
        }

        [TestMethod]
        [DeploymentItem("Examples\\SumArray.ajs")]
        public void SumArrayUsingInclude()
        {
            this.IncludeFile("SumArray.ajs");

            Assert.AreEqual(6, this.EvaluateExpression("sum"));
            Assert.AreEqual(1, this.EvaluateExpression("numbers[0]"));
            Assert.AreEqual(2, this.EvaluateExpression("numbers[1]"));
            Assert.AreEqual(3, this.EvaluateExpression("numbers[2]"));
        }

        [TestMethod]
        [DeploymentItem("Examples\\Apply.ajs")]
        public void ApplySquareToNumbersUsingInclude()
        {
            this.IncludeFile("Apply.ajs");

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
        [DeploymentItem("Examples\\Class.ajs")]
        public void DefineClass()
        {
            this.IncludeFile("Class.ajs");

            object result = this.machine.Environment.GetValue("Person");

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(DynamicClass));

            DynamicClass dynclass = (DynamicClass)result;

            Assert.IsTrue(dynclass.GetMemberNames().Contains("Age"));
            Assert.IsTrue(dynclass.GetMemberNames().Contains("Name"));
        }

        [TestMethod]
        [DeploymentItem("Examples\\Object.ajs")]
        public void DefineObject()
        {
            this.IncludeFile("Object.ajs");

            object result = this.machine.Environment.GetValue("Adam");

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(DynamicObject));

            DynamicObject dynobj = (DynamicObject)result;

            Assert.AreEqual("Adam", dynobj.GetValue("Name"));
            Assert.AreEqual(800, dynobj.GetValue("Age"));
        }

        [TestMethod]
        [DeploymentItem("Examples\\ObjectArrayAccess.ajs")]
        public void DefineObjectAnsUseArrayAccess()
        {
            this.IncludeFile("ObjectArrayAccess.ajs");

            Assert.AreEqual(700, this.machine.Environment.GetValue("result1"));
            Assert.AreEqual(800, this.machine.Environment.GetValue("result2"));
        }

        [TestMethod]
        [DeploymentItem("Examples\\ClassObject.ajs")]
        public void DefineClassCreateObjectCallMethod()
        {
            this.IncludeFile("ClassObject.ajs");

            object result = this.machine.Environment.GetValue("adam");

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(DynamicClassicObject));

            DynamicClassicObject dynobj = (DynamicClassicObject)result;

            Assert.AreEqual(800, dynobj.GetValue("Age"));
            Assert.AreEqual("Adam", dynobj.GetValue("Name"));
        }

        [TestMethod]
        [DeploymentItem("Examples\\ClassObjectNoThis.ajs")]
        public void DefineClassCreateObjectCallMethodWithoutUsingThis()
        {
            this.IncludeFile("ClassObjectNoThis.ajs");

            object result = this.machine.Environment.GetValue("adam");

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(DynamicClassicObject));

            DynamicClassicObject dynobj = (DynamicClassicObject)result;

            Assert.AreEqual(800, dynobj.GetValue("Age"));
            Assert.AreEqual("Adam", dynobj.GetValue("Name"));
            Assert.IsNull(dynobj.GetValue("Age2"));
            Assert.AreEqual(100, dynobj.GetValue("Something"));
        }

        [TestMethod]
        [DeploymentItem("Examples\\Constructor.ajs")]
        public void DefineClassCreateObjectUsingConstructor()
        {
            this.IncludeFile("Constructor.ajs");

            object result = this.machine.Environment.GetValue("adam");

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(DynamicClassicObject));

            DynamicClassicObject dynobj = (DynamicClassicObject)result;

            Assert.AreEqual(800, dynobj.GetValue("Age"));
            Assert.AreEqual("Adam", dynobj.GetValue("Name"));
        }

        [TestMethod]
        [DeploymentItem("Examples\\For.ajs")]
        public void ExecuteForCommand()
        {
            this.IncludeFile("For.ajs");

            Assert.AreEqual(15, this.EvaluateExpression("result"));
        }

        [TestMethod]
        [DeploymentItem("Examples\\Go.ajs")]
        public void ExecuteGoCommand()
        {
            this.IncludeFile("Go.ajs");

            Assert.AreEqual(1, this.EvaluateExpression("result"));
        }

        [TestMethod]
        [DeploymentItem("Examples\\Channel.ajs")]
        public void UseChannelWithGoCommand()
        {
            this.IncludeFile("Channel.ajs");

            Assert.AreEqual(10, this.EvaluateExpression("result"));
        }

        [TestMethod]
        [DeploymentItem("Examples\\ChannelOperator.ajs")]
        public void UseChannelWithGoCommandAndOperator()
        {
            this.IncludeFile("ChannelOperator.ajs");

            Assert.AreEqual(10, this.EvaluateExpression("result"));
        }

        [TestMethod]
        [DeploymentItem("Examples\\ChannelManyTimes.ajs")]
        public void UseChannelManyTimesWithGoCommand()
        {
            this.IncludeFile("ChannelManyTimes.ajs");

            Assert.AreEqual(15, this.EvaluateExpression("result"));
        }

        [TestMethod]
        [DeploymentItem("Examples\\ChannelManyTimesOperator.ajs")]
        public void UseChannelManyTimesWithGoCommandAndOperator()
        {
            this.IncludeFile("ChannelManyTimesOperator.ajs");

            Assert.AreEqual(15, this.EvaluateExpression("result"));
        }

        [TestMethod]
        [DeploymentItem("Examples\\Generate.ajs")]
        public void UseChannelToGenerateNumbers()
        {
            this.IncludeFile("Generate.ajs");

            Assert.AreEqual(15, this.EvaluateExpression("result"));
        }

        [TestMethod]
        [DeploymentItem("Examples\\Filter.ajs")]
        public void UseChannelToSumOddNumbers()
        {
            this.IncludeFile("Filter.ajs");

            Assert.AreEqual(16, this.EvaluateExpression("result"));
        }

        [TestMethod]
        [DeploymentItem("Examples\\Global.ajs")]
        public void GlobalVariableInFunction()
        {
            this.IncludeFile("Global.ajs");

            Assert.AreEqual(12, this.EvaluateExpression("result"));
        }

        [TestMethod]
        [DeploymentItem("Examples\\ScopeWhile.ajs")]
        public void ScopeInSimpleWhile()
        {
            this.IncludeFile("ScopeWhile.ajs");

            Assert.AreEqual(15, this.EvaluateExpression("result"));
            Assert.AreEqual(6, this.EvaluateExpression("k"));
        }

        [TestMethod]
        [DeploymentItem("Examples\\ScopeWhileComposite.ajs")]
        public void ScopeInCompositeWhile()
        {
            this.IncludeFile("ScopeWhileComposite.ajs");

            Assert.AreEqual(15, this.EvaluateExpression("result"));
            Assert.AreEqual(6, this.EvaluateExpression("k"));
        }

        [TestMethod]
        [DeploymentItem("Examples\\ScopeFunction.ajs")]
        public void ScopeInSimpleFunction()
        {
            this.IncludeFile("ScopeFunction.ajs");

            Assert.AreEqual(10, this.EvaluateExpression("value"));
            Assert.AreEqual(5, this.EvaluateExpression("k"));
            Assert.AreEqual(17, this.EvaluateExpression("result"));
        }

        [TestMethod]
        [DeploymentItem("Examples\\ScopeVar.ajs")]
        public void ScopeVar()
        {
            this.IncludeFile("ScopeVar.ajs");

            Assert.AreEqual(10, this.EvaluateExpression("result"));
        }

        [TestMethod]
        [DeploymentItem("Examples\\ScopeVarFor.ajs")]
        public void ScopeVarInForCommand()
        {
            this.IncludeFile("ScopeVarFor.ajs");

            Assert.AreEqual(10, this.EvaluateExpression("k"));
            Assert.AreEqual(15, this.EvaluateExpression("result"));
        }

        [TestMethod]
        [DeploymentItem("Examples\\ScopeVarForEach.ajs")]
        public void ScopeVarInForEachCommand()
        {
            this.IncludeFile("ScopeVarForEach.ajs");

            Assert.AreEqual(10, this.EvaluateExpression("number"));
            Assert.AreEqual(3, this.EvaluateExpression("result"));
        }

        [TestMethod]
        [DeploymentItem("Examples\\ScopeFunctionReturn.ajs")]
        public void ScopeInFunctionWithReturn()
        {
            this.IncludeFile("ScopeFunctionReturn.ajs");

            Assert.AreEqual(10, this.EvaluateExpression("value"));
            Assert.IsNull(this.EvaluateExpression("result"));
        }

        [TestMethod]
        [DeploymentItem("Examples\\ScopeEvaluate.ajs")]
        public void ScopeInEvaluateAtTop()
        {
            this.IncludeFile("ScopeEvaluate.ajs");

            Assert.AreEqual(22, this.EvaluateExpression("result"));
        }

        [TestMethod]
        [DeploymentItem("Examples\\ScopeEvaluateInFunction.ajs")]
        public void ScopeInEvaluateInFunction()
        {
            this.IncludeFile("ScopeEvaluateInFunction.ajs");

            Assert.AreEqual(3, this.EvaluateExpression("result"));
        }

        [TestMethod]
        [DeploymentItem("Examples\\Function.ajs")]
        public void EvaluateIncrementFunction()
        {
            this.IncludeFile("Function.ajs");

            Assert.AreEqual(2, this.EvaluateExpression("result"));
        }

        [TestMethod]
        [DeploymentItem("Examples\\FunctionIncrement.ajs")]
        public void CreateAndCallIncrementFunction()
        {
            this.IncludeFile("FunctionIncrement.ajs");

            Assert.AreEqual(3, this.EvaluateExpression("result"));
        }

        [TestMethod]
        [DeploymentItem("Examples\\FunctionMakeIncrement.ajs")]
        public void MakeAndCallIncrementFunction()
        {
            this.IncludeFile("FunctionMakeIncrement.ajs");

            Assert.AreEqual(4, this.EvaluateExpression("result"));
            Assert.AreEqual(5, this.EvaluateExpression("result2"));
            Assert.AreEqual(7, this.EvaluateExpression("result3"));
            Assert.AreEqual(9, this.EvaluateExpression("result4"));
        }

        [TestMethod]
        [DeploymentItem("Examples\\FunctionClass.ajs")]
        public void CallFunctionAssignedToClass()
        {
            this.IncludeFile("FunctionClass.ajs");

            Assert.AreEqual(800, this.EvaluateExpression("result"));
            Assert.AreEqual(100, this.EvaluateExpression("result2"));
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
        public void EvaluateNewExpressionWithDynamicObjectMemberNotation()
        {
            object result = this.EvaluateExpression("new { var Name = \"Adam\"; var Age = 800; function GetName() { return Name; } }");

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(DynamicObject));

            DynamicObject dynobj = (DynamicObject)result;

            Assert.AreEqual("Adam", dynobj.GetValue("Name"));
            Assert.AreEqual(800, dynobj.GetValue("Age"));
            Assert.AreEqual("Adam", dynobj.Invoke("GetName", new object[] { }));
            Assert.AreEqual("Adam", dynobj.Invoke("GetName", null));
        }

        [TestMethod]
        public void EvaluateConcatExpressionWithTwoStrings()
        {
            object result = this.EvaluateExpression("\"foo\" + \"bar\"");

            Assert.AreEqual("foobar", result);
        }

        [TestMethod]
        public void EvaluateConcatExpressionWithUndefinedVariable()
        {
            object result = this.EvaluateExpression("foo + \"bar\"");

            Assert.AreEqual("bar", result);
        }

        [TestMethod]
        public void EvaluateConcatExpressionWithUndefinedVariables()
        {
            object result = this.EvaluateExpression("foo + bar");

            Assert.AreEqual(string.Empty, result);
        }

        [TestMethod]
        public void EvaluateConcatExpressionWithStringAndInteger()
        {
            object result = this.EvaluateExpression("\"foo\" + 4");

            Assert.AreEqual("foo4", result);
        }

        [TestMethod]
        public void EvaluateEmptyClassCommand()
        {
            this.ExecuteCommand("class Foo { }");

            object result = this.machine.Environment.GetValue("Foo");

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(IClass));
            Assert.AreEqual(0, ((IClass)result).GetMemberNames().Count);
        }

        [TestMethod]
        public void EvaluateNewEmptyClassInstance()
        {
            this.ExecuteCommand("class Foo { }");
            this.ExecuteCommand("foo = new Foo();");

            object result = this.machine.Environment.GetValue("Foo");

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(IClass));
            Assert.AreEqual(0, ((IClass)result).GetMemberNames().Count);

            object instance = this.machine.Environment.GetValue("foo");

            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, typeof(IClassicObject));
        }

        [TestMethod]
        public void EvaluateNewClassInstance()
        {
            this.ExecuteCommand("class Person { var Name = \"Adam\"; var Age = 800; function GetName() { return Name; } }");
            this.ExecuteCommand("adam = new Person();");

            object result = this.machine.Environment.GetValue("Person");

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(IClass));
            Assert.AreEqual(3, ((IClass)result).GetMemberNames().Count);

            object instance = this.machine.Environment.GetValue("adam");

            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, typeof(IClassicObject));

            IClassicObject obj = (IClassicObject)instance;

            Assert.AreEqual("Adam", obj.GetValue("Name"));
            Assert.AreEqual(800, obj.GetValue("Age"));
            Assert.AreEqual("Adam", obj.Invoke("GetName", new object[] { }));
            Assert.AreEqual("Adam", obj.Invoke("GetName", null));
        }

        [TestMethod]
        public void EvaluateNewClassInstanceWithUnitializedMembers()
        {
            this.ExecuteCommand("class Person { var Name; var Age; function GetName() { return Name; } }");
            this.ExecuteCommand("adam = new Person();");

            object result = this.machine.Environment.GetValue("Person");

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(IClass));
            Assert.AreEqual(3, ((IClass)result).GetMemberNames().Count);

            object instance = this.machine.Environment.GetValue("adam");

            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, typeof(IClassicObject));

            IClassicObject obj = (IClassicObject)instance;

            Assert.IsNull(obj.GetValue("Name"));
            Assert.IsNull(obj.GetValue("Age"));
            Assert.IsNull(obj.Invoke("GetName", new object[] { }));
            Assert.IsNull(obj.Invoke("GetName", null));
        }

        [TestMethod]
        public void EvaluateIncrementOperatorsOnVariable()
        {
            this.machine.Environment.SetValue("foo", 1);

            Assert.AreEqual(2, this.EvaluateExpression("++foo"));
            Assert.AreEqual(2, this.machine.Environment.GetValue("foo"));

            Assert.AreEqual(2, this.EvaluateExpression("foo++"));
            Assert.AreEqual(3, this.machine.Environment.GetValue("foo"));

            Assert.AreEqual(2, this.EvaluateExpression("--foo"));
            Assert.AreEqual(2, this.machine.Environment.GetValue("foo"));

            Assert.AreEqual(2, this.EvaluateExpression("foo--"));
            Assert.AreEqual(1, this.machine.Environment.GetValue("foo"));
        }

        [TestMethod]
        public void EvaluateIncrementOperatorsOnObjectProperty()
        {
            this.machine.Environment.SetValue("foo", new Person() { Age = 1 });

            Assert.AreEqual(2, this.EvaluateExpression("++foo.Age"));
            Assert.AreEqual(2, this.EvaluateExpression("foo.Age"));

            Assert.AreEqual(2, this.EvaluateExpression("foo.Age++"));
            Assert.AreEqual(3, this.EvaluateExpression("foo.Age"));

            Assert.AreEqual(2, this.EvaluateExpression("--foo.Age"));
            Assert.AreEqual(2, this.EvaluateExpression("foo.Age"));

            Assert.AreEqual(2, this.EvaluateExpression("foo.Age--"));
            Assert.AreEqual(1, this.EvaluateExpression("foo.Age"));
        }

        [TestMethod]
        public void EvaluateIncrementOperatorsOnDynamicObjectProperty()
        {
            this.ExecuteCommand("foo = new { Age = 1 };");

            Assert.AreEqual(2, this.EvaluateExpression("++foo.Age"));
            Assert.AreEqual(2, this.EvaluateExpression("foo.Age"));

            Assert.AreEqual(2, this.EvaluateExpression("foo.Age++"));
            Assert.AreEqual(3, this.EvaluateExpression("foo.Age"));

            Assert.AreEqual(2, this.EvaluateExpression("--foo.Age"));
            Assert.AreEqual(2, this.EvaluateExpression("foo.Age"));

            Assert.AreEqual(2, this.EvaluateExpression("foo.Age--"));
            Assert.AreEqual(1, this.EvaluateExpression("foo.Age"));
        }

        [TestMethod]
        public void EvaluateNewDynamicObject()
        {
            object obj = this.EvaluateExpression("new DynamicObject()");

            Assert.IsNotNull(obj);
            Assert.IsInstanceOfType(obj, typeof(DynamicObject));
        }

        [TestMethod]
        public void EvaluateNewList()
        {
            object obj = this.EvaluateExpression("new List()");

            Assert.IsNotNull(obj);
            Assert.IsInstanceOfType(obj, typeof(ArrayList));
        }

        [TestMethod]
        public void EvaluateNewDynamicClass()
        {
            object obj = this.EvaluateExpression("new DynamicClass(\"MyClass\")");

            Assert.IsNotNull(obj);
            Assert.IsInstanceOfType(obj, typeof(DynamicClass));
        }

        [TestMethod]
        public void CreateDynamicObjectSettingProperties()
        {
            this.ExecuteCommand("Project.Database.Server = \"(local)\";");

            object project = this.EvaluateExpression("Project");

            Assert.IsNotNull(project);
            Assert.IsInstanceOfType(project, typeof(DynamicObject));

            object database = this.EvaluateExpression("Project.Database");

            Assert.IsNotNull(database);
            Assert.IsInstanceOfType(database, typeof(DynamicObject));

            object server = this.EvaluateExpression("Project.Database.Server");

            Assert.IsNotNull(server);
            Assert.IsInstanceOfType(server, typeof(string));
            Assert.AreEqual("(local)", server);
        }

        [TestMethod]
        public void CreateDynamicListAddingObject()
        {
            this.ExecuteCommand("Project.Entities.Add(new { Name = \"Person\" });");

            object project = this.EvaluateExpression("Project");

            Assert.IsNotNull(project);
            Assert.IsInstanceOfType(project, typeof(DynamicObject));

            object entities = this.EvaluateExpression("Project.Entities");

            Assert.IsNotNull(entities);
            Assert.IsInstanceOfType(entities, typeof(IList));
        }

        [TestMethod]
        public void CreateArrayOfDoubles()
        {
            object result = this.EvaluateExpression("new double[10]");

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(double[]));

            double[] array = (double[])result;

            Assert.AreEqual(10, array.Length);
        }

        [TestMethod]
        public void CreateArrayOfIntegersWithValues()
        {
            object result = this.EvaluateExpression("new int[] { 1, 2, 3 }");

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(int[]));

            int[] array = (int[])result;

            Assert.AreEqual(3, array.Length);

            Assert.AreEqual(1, array[0]);
            Assert.AreEqual(2, array[1]);
            Assert.AreEqual(3, array[2]);
        }

        [TestMethod]
        public void EvaluateArrayElements()
        {
            this.ExecuteCommand("numbers = new int[] { 1, 2, 3 };");

            Assert.AreEqual(1, this.EvaluateExpression("numbers[0]"));
            Assert.AreEqual(2, this.EvaluateExpression("numbers[1]"));
            Assert.AreEqual(3, this.EvaluateExpression("numbers[2]"));
            Assert.AreEqual(3, this.EvaluateExpression("numbers.Length"));
        }

        [TestMethod]
        public void EvaluatePredefinedConstants()
        {
            Assert.AreEqual(true, this.EvaluateExpression("true"));
            Assert.AreEqual(false, this.EvaluateExpression("false"));
            Assert.AreEqual(null, this.EvaluateExpression("null"));
        }

        [TestMethod]
        public void EvaluateUndefinedComplexExpressionToNull()
        {
            Assert.IsNull(this.EvaluateExpression("Project.Entities.Count"));
            Assert.IsNull(this.EvaluateExpression("Project.Entities"));
            Assert.IsNull(this.EvaluateExpression("Project"));
        }

        [TestMethod]
        public void CreateAndSetArrayValues()
        {
            this.ExecuteCommand("numbers = new int[3];");

            this.ExecuteCommand("numbers[0] = 1;");
            this.ExecuteCommand("numbers[1] = 2;");
            this.ExecuteCommand("numbers[2] = 3;");

            Assert.AreEqual(1, this.EvaluateExpression("numbers[0]"));
            Assert.AreEqual(2, this.EvaluateExpression("numbers[1]"));
            Assert.AreEqual(3, this.EvaluateExpression("numbers[2]"));
        }

        [TestMethod]
        public void CreateDinamicallyAndSetArrayValues()
        {
            this.ExecuteCommand("numbers[0] = 1;");
            this.ExecuteCommand("numbers[1] = 2;");
            this.ExecuteCommand("numbers[2] = 3;");

            Assert.AreEqual(1, this.EvaluateExpression("numbers[0]"));
            Assert.AreEqual(2, this.EvaluateExpression("numbers[1]"));
            Assert.AreEqual(3, this.EvaluateExpression("numbers[2]"));
        }

        [TestMethod]
        public void CreateAndSetDictionaryValues()
        {
            this.ExecuteCommand("numbers = new Dictionary();");

            this.ExecuteCommand("numbers[\"one\"] = 1;");
            this.ExecuteCommand("numbers[\"two\"] = 2;");
            this.ExecuteCommand("numbers[\"three\"] = 3;");

            Assert.AreEqual(1, this.EvaluateExpression("numbers[\"one\"]"));
            Assert.AreEqual(2, this.EvaluateExpression("numbers[\"two\"]"));
            Assert.AreEqual(3, this.EvaluateExpression("numbers[\"three\"]"));
        }

        [TestMethod]
        public void CreateDinamicallyAndSetDictionaryValues()
        {
            this.ExecuteCommand("numbers[\"one\"] = 1;");
            this.ExecuteCommand("numbers[\"two\"] = 2;");
            this.ExecuteCommand("numbers[\"three\"] = 3;");

            Assert.AreEqual(1, this.EvaluateExpression("numbers[\"one\"]"));
            Assert.AreEqual(2, this.EvaluateExpression("numbers[\"two\"]"));
            Assert.AreEqual(3, this.EvaluateExpression("numbers[\"three\"]"));
        }

        [TestMethod]
        public void EvaluateStringExpansions()
        {
            this.ExecuteCommand("foo = \"bar\";");
            this.ExecuteCommand("add1 = function(n) { return n+1; };");

            Assert.AreEqual("foobar", this.EvaluateExpression("\"foo${foo}\""));
            Assert.AreEqual("barfoo", this.EvaluateExpression("\"${foo}foo\""));
            Assert.AreEqual("foo3", this.EvaluateExpression("\"foo${1+2}\""));
            Assert.AreEqual("foo", this.EvaluateExpression("\"foo${bar}\""));
            Assert.AreEqual("1234", this.EvaluateExpression("\"1${1+1}3${1+1+1+1}\""));
            Assert.AreEqual("01234", this.EvaluateExpression("\"${0}1${1+1}3${1+1+1+1}\""));
            Assert.AreEqual("01234", this.EvaluateExpression("\"${0}1${add1(1)}3${add1(3)}\""));
        }

        [TestMethod]
        public void EvaluateEvaluateFunction()
        {
            this.ExecuteCommand("one = 1;");
            this.ExecuteCommand("two = 2;");
            Assert.AreEqual(0, this.EvaluateExpression("Evaluate(\"0\")"));
            Assert.AreEqual(3, this.EvaluateExpression("Evaluate(\"1+2\")"));
            Assert.AreEqual(3, this.EvaluateExpression("Evaluate(\"one+two\")"));
        }

        [TestMethod]
        public void EvaluateExecuteSubroutineAndEvaluateFunction()
        {
            this.EvaluateExpression("Execute(\"one = 1; two=2;\")");
            Assert.AreEqual(0, this.EvaluateExpression("Evaluate(\"0\")"));
            Assert.AreEqual(3, this.EvaluateExpression("Evaluate(\"1+2\")"));
            Assert.AreEqual(3, this.EvaluateExpression("Evaluate(\"one+two\")"));
        }

        [TestMethod]
        public void CreateChannel()
        {
            object result = this.EvaluateExpression("new Channel()");

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(Channel));
        }

        [TestMethod]
        public void CreateFuture()
        {
            object result = this.EvaluateExpression("new Future()");

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(Future));
        }

        [TestMethod]
        [DeploymentItem("Examples\\Future.ajs")]
        public void UseFutureWithGoCommand()
        {
            this.IncludeFile("Future.ajs");

            Assert.AreEqual(1, this.EvaluateExpression("result"));
            Assert.AreEqual(1, this.EvaluateExpression("result2"));
        }

        [TestMethod]
        [DeploymentItem("Examples\\FunctionalRunningSum.ajs")]
        public void EvaluateFunctionalRunningSum()
        {
            this.IncludeFile("FunctionalRunningSum.ajs");

            Assert.AreEqual(5, this.EvaluateExpression("result"));
            Assert.AreEqual(15, this.EvaluateExpression("result2"));
        }

        [TestMethod]
        [DeploymentItem("Examples\\FunctionalRunningSumParam.ajs")]
        public void EvaluateFunctionalRunningSumParam()
        {
            this.IncludeFile("FunctionalRunningSumParam.ajs");

            Assert.AreEqual(5, this.EvaluateExpression("result"));
            Assert.AreEqual(15, this.EvaluateExpression("result2"));
        }

        [TestMethod]
        [DeploymentItem("Examples\\FunctionalCall.ajs")]
        public void EvaluateFunctionalCall()
        {
            this.IncludeFile("FunctionalCall.ajs");

            Assert.AreEqual(810, this.EvaluateExpression("result"));
        }

        [TestMethod]
        [DeploymentItem("Examples\\AgentCall.ajs")]
        public void EvaluateAgentCall()
        {
            this.IncludeFile("AgentCall.ajs");

            Assert.AreEqual(2, this.EvaluateExpression("result"));
        }

        [TestMethod]
        [DeploymentItem("Examples\\AgentChain.ajs")]
        public void EvaluateAgentChain()
        {
            this.IncludeFile("AgentChain.ajs");

            Assert.AreEqual(6, this.EvaluateExpression("result"));
        }

        [TestMethod]
        [DeploymentItem("Examples\\AgentCollatz.ajs")]
        public void EvaluateAgentCollatz()
        {
            this.IncludeFile("AgentCollatz.ajs");

            Assert.IsNotNull(this.EvaluateExpression("result"));
            Assert.AreEqual(16, this.EvaluateExpression("result.Count"));
        }

        [TestMethod]
        [DeploymentItem("Examples\\DefaultMethodProxy.ajs")]
        public void EvaluateDefaultMethodProxy()
        {
            this.IncludeFile("DefaultMethodProxy.ajs");

            Assert.AreEqual(4, this.EvaluateExpression("result"));
        }

        [TestMethod]
        [DeploymentItem("Examples\\DefaultMethodLoadBalancer.ajs")]
        public void EvaluateDefaultMethodLoadBalancer()
        {
            this.IncludeFile("DefaultMethodLoadBalancer.ajs");

            Assert.AreEqual(10, this.EvaluateExpression("result"));
        }

        [TestMethod]
        [DeploymentItem("Examples\\FunctionVarArgs.ajs")]
        public void EvaluateFunctionVarArgs()
        {
            this.IncludeFile("FunctionVarArgs.ajs");

            Assert.AreEqual(1, this.EvaluateExpression("result"));
            Assert.AreEqual(2, this.EvaluateExpression("result2"));
            Assert.IsNull(this.EvaluateExpression("result3"));
        }

        [TestMethod]
        [DeploymentItem("Examples\\FunctionVarArgsSplat.ajs")]
        public void EvaluateFunctionVarArgsSplat()
        {
            this.IncludeFile("FunctionVarArgsSplat.ajs");

            Assert.AreEqual(1, this.EvaluateExpression("result"));
            Assert.AreEqual(2, this.EvaluateExpression("result2"));
            Assert.IsNull(this.EvaluateExpression("result3"));
        }

        [TestMethod]
        [DeploymentItem("Examples\\RemotingHost.ajs")]
        public void EvaluateRemotingHost()
        {
            Assert.AreEqual(1, this.machine.GetLocalHosts().Count);
            Assert.AreEqual(0, this.machine.GetRemoteHosts().Count);

            this.IncludeFile("RemotingHost.ajs");

            Assert.AreEqual("Adam", this.EvaluateExpression("result"));
            Assert.AreEqual("Adam", this.EvaluateExpression("result2"));
            Assert.AreEqual("New Adam", this.EvaluateExpression("result3"));
            Assert.AreEqual("New Adam", this.EvaluateExpression("result4"));
            Assert.AreEqual("Adam", this.EvaluateExpression("result5"));

            Assert.AreEqual(2, this.machine.GetLocalHosts().Count);
            Assert.AreEqual(1, this.machine.GetRemoteHosts().Count);

            IHost server = (IHost)this.machine.Environment.GetValue("server");
            Assert.IsNotNull(server);
            Assert.IsInstanceOfType(server, typeof(RemotingHostServer));
            Assert.AreEqual("tcp://localhost:10000/RemoteHost", server.Address);
        }

        [TestMethod]
        [DeploymentItem("Examples\\RemotingHostExecuteCommand.ajs")]
        public void EvaluateRemotingHostExecuteCommand()
        {
            Assert.AreEqual(1, this.machine.GetLocalHosts().Count);
            Assert.AreEqual(0, this.machine.GetRemoteHosts().Count);

            this.IncludeFile("RemotingHostExecuteCommand.ajs");

            Assert.AreEqual("Adam", this.EvaluateExpression("result"));
            Assert.AreEqual("Adam", this.EvaluateExpression("result2"));

            Assert.AreEqual(2, this.machine.GetLocalHosts().Count);
            Assert.AreEqual(1, this.machine.GetRemoteHosts().Count);
        }

        [TestMethod]
        [DeploymentItem("Examples\\RemotingHostInclude.ajs")]
        [DeploymentItem("Examples\\AgentCall.ajs")]
        public void EvaluateRemotingHostInclude()
        {
            Assert.AreEqual(1, this.machine.GetLocalHosts().Count);
            Assert.AreEqual(0, this.machine.GetRemoteHosts().Count);

            this.IncludeFile("RemotingHostInclude.ajs");

            Assert.AreEqual(2, this.EvaluateExpression("result"));

            Assert.AreEqual(2, this.machine.GetLocalHosts().Count);
            Assert.AreEqual(1, this.machine.GetRemoteHosts().Count);
        }

        [TestMethod]
        [DeploymentItem("Examples\\RegisterRemotingHost.ajs")]
        public void EvaluateRegisterRemotingHost()
        {
            Assert.AreEqual(1, this.machine.GetLocalHosts().Count);
            Assert.AreEqual(0, this.machine.GetRemoteHosts().Count);

            this.IncludeFile("RegisterRemotingHost.ajs");

            Assert.AreEqual(3, this.machine.GetLocalHosts().Count);
            Assert.AreEqual(2, this.machine.GetRemoteHosts().Count);

            IHost server = (IHost)this.machine.Environment.GetValue("server");
            Assert.IsNotNull(server);
            Assert.IsInstanceOfType(server, typeof(RemotingHostServer));
            Assert.AreEqual("tcp://localhost:12000/RemoteHost0", server.Address);

            IHost server2 = (IHost)this.machine.Environment.GetValue("server2");
            Assert.IsNotNull(server2);
            Assert.IsInstanceOfType(server2, typeof(RemotingHostServer));
            Assert.AreEqual("tcp://localhost:30000/RemoteHost2", server2.Address);

            Assert.AreEqual("tcp://localhost:30000/RemoteHost2", this.machine.Environment.GetValue("result"));
        }

        [TestMethod]
        [DeploymentItem("Examples\\WcfHost.ajs")]
        public void EvaluateWcfHost()
        {
            Assert.AreEqual(1, this.machine.GetLocalHosts().Count);
            Assert.AreEqual(0, this.machine.GetRemoteHosts().Count);

            this.IncludeFile("WcfHost.ajs");

            Assert.AreEqual("Adam", this.EvaluateExpression("result"));
            Assert.AreEqual("Adam", this.EvaluateExpression("result2"));

            Assert.AreEqual(2, this.machine.GetLocalHosts().Count);
            Assert.AreEqual(1, this.machine.GetRemoteHosts().Count);

            IHost server = (IHost)this.machine.Environment.GetValue("server");
            Assert.IsNotNull(server);
            Assert.IsInstanceOfType(server, typeof(WcfHostServer));
            Assert.AreEqual("http://localhost:20000/RemoteHost", server.Address);
        }

        [TestMethod]
        public void EvaluateHostedInvocation()
        {
            Assert.AreEqual(3, this.EvaluateExpression("at Machine.Current.Host function(x,y) { return x+y; } with (1,2)"));
        }

        [TestMethod]
        [DeploymentItem("Examples\\ReferenceSetAndGet.ajs")]
        public void EvaluateReferenceSetAndGet()
        {
            this.IncludeFile("ReferenceSetAndGet.ajs");
            Assert.AreEqual(1, this.machine.Environment.GetValue("result"));
        }

        [TestMethod]
        public void ExecuteSimpleTransaction()
        {
            this.ExecuteCommand("transaction { result = 1; }");
            Assert.AreEqual(1, this.machine.Environment.GetValue("result"));
        }

        [TestMethod]
        [DeploymentItem("Examples\\TransactionsTwoReferences.ajs")]
        public void EvaluateTransactionsTwoReferences()
        {
            this.IncludeFile("TransactionsTwoReferences.ajs");
            Assert.AreEqual(2, this.machine.Environment.GetValue("result1"));
            Assert.AreEqual(3, this.machine.Environment.GetValue("result2"));
            Assert.AreEqual(1, this.machine.Environment.GetValue("result1original"));
            Assert.AreEqual(2, this.machine.Environment.GetValue("result2original"));
        }

        [TestMethod]
        public void EvaluateMachineCurrent()
        {
            Assert.AreEqual(this.machine, this.EvaluateExpression("Machine.Current"));
        }

        [TestMethod]
        public void EvaluateStaticMethod()
        {
            Assert.IsFalse((bool)this.EvaluateExpression("System.IO.File.Exists(\"foo.txt\")"));
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

namespace AjSharp.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    using AjLanguage.Commands;
    using AjLanguage.Expressions;
    using AjLanguage.Language;

    using AjSharp;
    using AjSharp.Compiler;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ParserTests
    {
        [TestMethod]
        public void ParseConstantExpressions()
        {
            IExpression expression;

            expression = ParseExpression("1");
            Assert.IsNotNull(expression);
            Assert.IsInstanceOfType(expression, typeof(ConstantExpression));
            Assert.AreEqual(1, expression.Evaluate(null));

            expression = ParseExpression("1.2");
            Assert.IsNotNull(expression);
            Assert.IsInstanceOfType(expression, typeof(ConstantExpression));
            Assert.AreEqual(1.2, expression.Evaluate(null));

            expression = ParseExpression("false");
            Assert.IsNotNull(expression);
            Assert.IsInstanceOfType(expression, typeof(ConstantExpression));
            Assert.IsFalse((bool)expression.Evaluate(null));

            expression = ParseExpression("\"foo\"");
            Assert.IsNotNull(expression);
            Assert.IsInstanceOfType(expression, typeof(ConstantExpression));
            Assert.AreEqual("foo", expression.Evaluate(null));

            Assert.IsNull(ParseExpression(""));
        }

        [TestMethod]
        public void ParseSimpleUnaryExpression()
        {
            IExpression expression = ParseExpression("-2");

            Assert.IsNotNull(expression);
            Assert.IsInstanceOfType(expression, typeof(ArithmeticUnaryExpression));

            ArithmeticUnaryExpression operation = (ArithmeticUnaryExpression)expression;

            Assert.AreEqual(ArithmeticOperator.Minus, operation.Operation);
            Assert.IsNotNull(operation.Expression);
            Assert.IsInstanceOfType(operation.Expression, typeof(ConstantExpression));
        }

        [TestMethod]
        public void ParseSimpleBinaryExpression()
        {
            IExpression expression = ParseExpression("a + 2");

            Assert.IsNotNull(expression);
            Assert.IsInstanceOfType(expression, typeof(ArithmeticBinaryExpression));

            ArithmeticBinaryExpression operation = (ArithmeticBinaryExpression)expression;

            Assert.AreEqual(ArithmeticOperator.Add, operation.Operation);
            Assert.IsNotNull(operation.LeftExpression);
            Assert.IsInstanceOfType(operation.LeftExpression, typeof(VariableExpression));
            Assert.IsNotNull(operation.RightExpression);
            Assert.IsInstanceOfType(operation.RightExpression, typeof(ConstantExpression));
        }

        [TestMethod]
        public void ParseModExpression()
        {
            IExpression expression = ParseExpression("a % 2");

            Assert.IsNotNull(expression);
            Assert.IsInstanceOfType(expression, typeof(ArithmeticBinaryExpression));

            ArithmeticBinaryExpression operation = (ArithmeticBinaryExpression)expression;

            Assert.AreEqual(ArithmeticOperator.Modulo, operation.Operation);
            Assert.IsNotNull(operation.LeftExpression);
            Assert.IsInstanceOfType(operation.LeftExpression, typeof(VariableExpression));
            Assert.IsNotNull(operation.RightExpression);
            Assert.IsInstanceOfType(operation.RightExpression, typeof(ConstantExpression));
        }

        [TestMethod]
        public void ParseSimpleCompareExpression()
        {
            IExpression expression = ParseExpression("x <= 1");

            Assert.IsNotNull(expression);
            Assert.IsInstanceOfType(expression, typeof(CompareExpression));

            CompareExpression operation = (CompareExpression)expression;

            Assert.AreEqual(ComparisonOperator.LessEqual, operation.Operation);
            Assert.IsNotNull(operation.LeftExpression);
            Assert.IsInstanceOfType(operation.LeftExpression, typeof(VariableExpression));
            Assert.IsNotNull(operation.RightExpression);
            Assert.IsInstanceOfType(operation.RightExpression, typeof(ConstantExpression));
        }

        [TestMethod]
        public void ParseSimpleBinaryExpressionWithParenthesis()
        {
            IExpression expression = ParseExpression("((a) + (2))");

            Assert.IsNotNull(expression);
            Assert.IsInstanceOfType(expression, typeof(ArithmeticBinaryExpression));

            ArithmeticBinaryExpression operation = (ArithmeticBinaryExpression)expression;

            Assert.AreEqual(ArithmeticOperator.Add, operation.Operation);
            Assert.IsNotNull(operation.LeftExpression);
            Assert.IsInstanceOfType(operation.LeftExpression, typeof(VariableExpression));
            VariableExpression varexpr = (VariableExpression)operation.LeftExpression;
            Assert.AreEqual("a", varexpr.VariableName);
            Assert.IsNotNull(operation.RightExpression);
            Assert.IsInstanceOfType(operation.RightExpression, typeof(ConstantExpression));
        }

        [TestMethod]
        public void ParseTwoBinaryExpression()
        {
            IExpression expression = ParseExpression("a + 2 - 3");

            Assert.IsNotNull(expression);
            Assert.IsInstanceOfType(expression, typeof(ArithmeticBinaryExpression));

            ArithmeticBinaryExpression operation = (ArithmeticBinaryExpression)expression;

            Assert.AreEqual(ArithmeticOperator.Subtract, operation.Operation);
            Assert.IsNotNull(operation.LeftExpression);
            Assert.IsInstanceOfType(operation.LeftExpression, typeof(ArithmeticBinaryExpression));
            Assert.IsNotNull(operation.RightExpression);
            Assert.IsInstanceOfType(operation.RightExpression, typeof(ConstantExpression));
        }

        [TestMethod]
        public void ParseTwoBinaryExpressionDifferentLevels()
        {
            IExpression expression = ParseExpression("a + 2 * 3");

            Assert.IsNotNull(expression);
            Assert.IsInstanceOfType(expression, typeof(ArithmeticBinaryExpression));

            ArithmeticBinaryExpression arithmeticExpression = (ArithmeticBinaryExpression)expression;

            Assert.AreEqual(ArithmeticOperator.Add, arithmeticExpression.Operation);
            Assert.IsNotNull(arithmeticExpression.LeftExpression);
            Assert.IsInstanceOfType(arithmeticExpression.LeftExpression, typeof(VariableExpression));
            Assert.IsNotNull(arithmeticExpression.RightExpression);
            Assert.IsInstanceOfType(arithmeticExpression.RightExpression, typeof(ArithmeticBinaryExpression));

            ArithmeticBinaryExpression rigthExpression = (ArithmeticBinaryExpression) arithmeticExpression.RightExpression;

            Assert.AreEqual(ArithmeticOperator.Multiply, rigthExpression.Operation);
            Assert.IsInstanceOfType(rigthExpression.LeftExpression, typeof(ConstantExpression));
            Assert.IsInstanceOfType(rigthExpression.RightExpression, typeof(ConstantExpression));
        }

        [TestMethod]
        public void ParseSetVariableCommand()
        {
            ICommand command = ParseCommand("a = 1;");

            Assert.IsNotNull(command);
            Assert.IsInstanceOfType(command, typeof(SetCommand));

            SetCommand setcmd = (SetCommand)command;

            Assert.IsInstanceOfType(setcmd.LeftValue, typeof(VariableExpression));
            Assert.AreEqual("a", ((VariableExpression)setcmd.LeftValue).VariableName);
            Assert.IsNotNull(setcmd.Expression);
            Assert.IsInstanceOfType(setcmd.Expression, typeof(ConstantExpression));
            Assert.AreEqual(1, setcmd.Expression.Evaluate(null));
        }

        [TestMethod]
        public void ParseReturnCommand()
        {
            ICommand command = ParseCommand("return;");

            Assert.IsNotNull(command);
            Assert.IsInstanceOfType(command, typeof(ReturnCommand));

            ReturnCommand retcmd = (ReturnCommand)command;

            Assert.IsNull(retcmd.Expression);
        }

        [TestMethod]
        public void ParseReturnCommandWithExpression()
        {
            ICommand command = ParseCommand("return 1;");

            Assert.IsNotNull(command);
            Assert.IsInstanceOfType(command, typeof(ReturnCommand));

            ReturnCommand retcmd = (ReturnCommand)command;

            Assert.IsNotNull(retcmd.Expression);
            Assert.IsInstanceOfType(retcmd.Expression, typeof(ConstantExpression));
        }

        [TestMethod]
        public void ParseIfCommand()
        {
            ICommand command = ParseCommand("if (x<=1) return 1;");

            Assert.IsNotNull(command);
            Assert.IsInstanceOfType(command, typeof(IfCommand));

            IfCommand ifcmd = (IfCommand)command;

            Assert.IsNotNull(ifcmd.Condition);
            Assert.IsNotNull(ifcmd.ThenCommand);
            Assert.IsNull(ifcmd.ElseCommand);
        }

        [TestMethod]
        public void ParseIfCommandWithElse()
        {
            ICommand command = ParseCommand("if (x<=1) return 1; else return x * (x-1);");

            Assert.IsNotNull(command);
            Assert.IsInstanceOfType(command, typeof(IfCommand));

            IfCommand ifcmd = (IfCommand)command;

            Assert.IsNotNull(ifcmd.Condition);
            Assert.IsNotNull(ifcmd.ThenCommand);
            Assert.IsNotNull(ifcmd.ElseCommand);
        }

        [TestMethod]
        public void ParseSimpleWhile()
        {
            ICommand command = ParseCommand("while (x<10) x=x+1;");

            Assert.IsNotNull(command);
            Assert.IsInstanceOfType(command, typeof(WhileCommand));

            WhileCommand whilecmd = (WhileCommand)command;

            Assert.IsNotNull(whilecmd.Condition);
            Assert.IsNotNull(whilecmd.Command);
            Assert.IsInstanceOfType(whilecmd.Command, typeof(SetCommand));
        }

        [TestMethod]
        public void ParseSimpleForEach()
        {
            ICommand command = ParseCommand("foreach (x in xs) y=y+x;");

            Assert.IsNotNull(command);
            Assert.IsInstanceOfType(command, typeof(ForEachCommand));

            ForEachCommand foreachcmd = (ForEachCommand) command;

            Assert.AreEqual("x", foreachcmd.Name);
            Assert.IsNotNull(foreachcmd.Expression);
            Assert.IsInstanceOfType(foreachcmd.Expression, typeof(VariableExpression));
            Assert.IsNotNull(foreachcmd.Command);
            Assert.IsInstanceOfType(foreachcmd.Command, typeof(SetCommand));
            Assert.IsFalse(foreachcmd.LocalVariable);
        }

        [TestMethod]
        public void ParseSimpleForEachWithLocalVar()
        {
            ICommand command = ParseCommand("foreach (var x in xs) y=y+x;");

            Assert.IsNotNull(command);
            Assert.IsInstanceOfType(command, typeof(ForEachCommand));

            ForEachCommand foreachcmd = (ForEachCommand)command;

            Assert.AreEqual("x", foreachcmd.Name);
            Assert.IsNotNull(foreachcmd.Expression);
            Assert.IsInstanceOfType(foreachcmd.Expression, typeof(VariableExpression));
            Assert.IsNotNull(foreachcmd.Command);
            Assert.IsInstanceOfType(foreachcmd.Command, typeof(SetCommand));
            Assert.IsTrue(foreachcmd.LocalVariable);
        }

        [TestMethod]
        public void ParseSimpleIncrement()
        {
            ICommand command = ParseCommand("k++;");

            Assert.IsNotNull(command);
            Assert.IsInstanceOfType(command, typeof(ExpressionCommand));

            ExpressionCommand expcmd = (ExpressionCommand)command;

            Assert.IsNotNull(expcmd.Expression);
            Assert.IsInstanceOfType(expcmd.Expression, typeof(IncrementExpression));
        }

        [TestMethod]
        public void ParseSimpleFor()
        {
            ICommand command = ParseCommand("for (k=1; k<=5; k++) result=result+k;");

            Assert.IsNotNull(command);
            Assert.IsInstanceOfType(command, typeof(ForCommand));

            ForCommand forcommand = (ForCommand)command;

            Assert.IsNotNull(forcommand.InitialCommand);
            Assert.IsNotNull(forcommand.Condition);
            Assert.IsNotNull(forcommand.EndCommand);
            Assert.IsNotNull(forcommand.Body);
        }

        [TestMethod]
        public void ParseCompositeCommand()
        {
            ICommand command = ParseCommand("{ x=1; y=2; }");

            Assert.IsNotNull(command);
            Assert.IsInstanceOfType(command, typeof(CompositeCommand));

            CompositeCommand compcmd = (CompositeCommand)command;

            Assert.AreEqual(2, compcmd.CommandCount);
            Assert.IsNotNull(compcmd.Commands);
            Assert.AreEqual(2, compcmd.Commands.Count);

            foreach (ICommand cmd in compcmd.Commands)
            {
                Assert.IsNotNull(cmd);
                Assert.IsInstanceOfType(cmd, typeof(SetCommand));
            }
        }

        [TestMethod]
        public void ParseFunctionDefinition()
        {
            ICommand command = ParseCommand("function Abs(x) { if (x<0) return -x; else return x * factorial(x-1); }");

            Assert.IsNotNull(command);
            Assert.IsInstanceOfType(command, typeof(DefineFunctionCommand));

            DefineFunctionCommand defcmd = (DefineFunctionCommand) command;

            Assert.AreEqual("Abs", defcmd.FunctionName);
            Assert.AreEqual(1, defcmd.ParameterNames.Length);
            Assert.IsFalse(defcmd.HasVariableParameters);
            Assert.AreEqual("x", defcmd.ParameterNames[0]);
            Assert.IsNotNull(defcmd.Body);
            Assert.IsInstanceOfType(defcmd.Body, typeof(CompositeCommand));
            Assert.IsFalse(defcmd.IsDefault);
        }

        [TestMethod]
        public void ParseFunctionDefinitionWithVariableArguments()
        {
            ICommand command = ParseCommand("function Foo(par1,pars...) { return par1 + pars.Count; }");

            Assert.IsNotNull(command);
            Assert.IsInstanceOfType(command, typeof(DefineFunctionCommand));

            DefineFunctionCommand defcmd = (DefineFunctionCommand)command;

            Assert.AreEqual("Foo", defcmd.FunctionName);
            Assert.AreEqual(2, defcmd.ParameterNames.Length);
            Assert.IsTrue(defcmd.HasVariableParameters);
            Assert.AreEqual("par1", defcmd.ParameterNames[0]);
            Assert.AreEqual("pars", defcmd.ParameterNames[1]);
            Assert.IsNotNull(defcmd.Body);
            Assert.IsInstanceOfType(defcmd.Body, typeof(CompositeCommand));
            Assert.IsFalse(defcmd.IsDefault);
        }

        [TestMethod]
        public void ParseInvokeExpression()
        {
            IExpression expression = ParseExpression("Factorial(3)");

            Assert.IsNotNull(expression);
            Assert.IsInstanceOfType(expression, typeof(InvokeExpression));

            InvokeExpression invexp = (InvokeExpression)expression;

            Assert.AreEqual("Factorial", invexp.Name);
            Assert.AreEqual(1, invexp.Arguments.Count);
            Assert.IsInstanceOfType(invexp.Arguments.First(), typeof(ConstantExpression));
        }

        [TestMethod]
        public void ParseInvokeExpressionWithSplats()
        {
            IExpression expression = ParseExpression("DoSomething(3, pars...)");

            Assert.IsNotNull(expression);
            Assert.IsInstanceOfType(expression, typeof(InvokeExpression));

            InvokeExpression invexp = (InvokeExpression)expression;

            Assert.AreEqual("DoSomething", invexp.Name);
            Assert.AreEqual(2, invexp.Arguments.Count);
            Assert.IsInstanceOfType(invexp.Arguments.First(), typeof(ConstantExpression));
        }

        [TestMethod]
        public void ParseInvokeExpressionExpression()
        {
            IExpression expression = ParseExpression("MyFunc(2)(3)");

            Assert.IsNotNull(expression);
            Assert.IsInstanceOfType(expression, typeof(InvokeExpressionExpression));

            InvokeExpressionExpression invexp = (InvokeExpressionExpression)expression;

            Assert.IsInstanceOfType(invexp.Expression, typeof(InvokeExpression));
            Assert.AreEqual(1, invexp.Arguments.Count);
            Assert.IsInstanceOfType(invexp.Arguments.First(), typeof(ConstantExpression));
        }

        [TestMethod]
        public void ParsePrintCommand()
        {
            ICommand command = ParseCommand("Print(1);");

            Assert.IsNotNull(command);
            Assert.IsInstanceOfType(command, typeof(ExpressionCommand));
        }

        [TestMethod]
        public void ParseSimpleDotExpression()
        {
            IExpression expression = ParseExpression("a.Length");

            Assert.IsNotNull(expression);
            Assert.IsInstanceOfType(expression, typeof(DotExpression));            
        }

        [TestMethod]
        public void ParseSimpleDotExpressionWithArguments()
        {
            IExpression expression = ParseExpression("foo.Bar(1,2)");

            Assert.IsNotNull(expression);
            Assert.IsInstanceOfType(expression, typeof(DotExpression));
        }

        [TestMethod]
        public void ParseSimpleDotExpressionWithSplats()
        {
            IExpression expression = ParseExpression("foo.Bar(pars...)");

            Assert.IsNotNull(expression);
            Assert.IsInstanceOfType(expression, typeof(DotExpression));
        }

        [TestMethod]
        [ExpectedException(typeof(UnexpectedTokenException))]
        public void RaiseIfUnexpectedTokenDot()
        {
            ParseExpression(".");
        }

        [TestMethod]
        public void ParseNewExpressionWithSimpleName()
        {
            IExpression expression = ParseExpression("new DynamicObject()");

            Assert.IsNotNull(expression);
            Assert.IsInstanceOfType(expression, typeof(NewExpression));

            NewExpression newexp = (NewExpression)expression;

            Assert.AreEqual("DynamicObject", newexp.TypeName);
        }

        [TestMethod]
        public void ParseNewExpressionWithQualifiedName()
        {
            IExpression expression = ParseExpression("new System.IO.FileInfo(\".\")");

            Assert.IsNotNull(expression);
            Assert.IsInstanceOfType(expression, typeof(NewExpression));

            NewExpression newexp = (NewExpression)expression;

            Assert.AreEqual("System.IO.FileInfo", newexp.TypeName);
            Assert.AreEqual(1, newexp.Arguments.Count);
            Assert.IsInstanceOfType(newexp.Arguments.First(), typeof(ConstantExpression));
        }

        [TestMethod]
        public void ParseNewExpressionWithPropertyInitialization()
        {
            IExpression expression = ParseExpression("new System.Data.SqlClient.SqlConnection() { ConnectionString = \"foo\", CommandText = \"bar\"}");

            Assert.IsNotNull(expression);
            Assert.IsInstanceOfType(expression, typeof(MultipleSetExpression));

            MultipleSetExpression msetexp = (MultipleSetExpression)expression;

            Assert.IsNotNull(msetexp.LeftObject);
            Assert.IsInstanceOfType(msetexp.LeftObject, typeof(NewExpression));
            Assert.AreEqual(2, msetexp.PropertyNames.Length);
            Assert.AreEqual("ConnectionString", msetexp.PropertyNames[0]);
            Assert.AreEqual("CommandText", msetexp.PropertyNames[1]);
        }

        [TestMethod]
        public void ParseNewExpressionWithDynamicObject()
        {
            IExpression expression = ParseExpression("new { Name = \"Adam\", Age = 800}");

            Assert.IsNotNull(expression);
            Assert.IsInstanceOfType(expression, typeof(MultipleSetExpression));

            MultipleSetExpression msetexp = (MultipleSetExpression)expression;

            Assert.IsNotNull(msetexp.LeftObject);
            Assert.IsInstanceOfType(msetexp.LeftObject, typeof(NewExpression));
            Assert.AreEqual(2, msetexp.PropertyNames.Length);
            Assert.AreEqual("Name", msetexp.PropertyNames[0]);
            Assert.AreEqual("Age", msetexp.PropertyNames[1]);
        }

        [TestMethod]
        public void ParseNewExpressionWithDynamicObjectMemberNotation()
        {
            IExpression expression = ParseExpression("new { var Name = \"Adam\"; var Age = 800;}");

            Assert.IsNotNull(expression);
            Assert.IsInstanceOfType(expression, typeof(MultipleSetExpression));

            MultipleSetExpression msetexp = (MultipleSetExpression)expression;

            Assert.IsNotNull(msetexp.LeftObject);
            Assert.IsInstanceOfType(msetexp.LeftObject, typeof(NewExpression));
            Assert.AreEqual(2, msetexp.PropertyNames.Length);
            Assert.AreEqual("Name", msetexp.PropertyNames[0]);
            Assert.AreEqual("Age", msetexp.PropertyNames[1]);
        }

        [TestMethod]
        public void ParseEmptyClassDefinition()
        {
            ICommand command = ParseCommand("class Foo { }");

            Assert.IsNotNull(command);
            Assert.IsInstanceOfType(command, typeof(DefineClassCommand));
        }

        [TestMethod]
        public void ParseClassDefinitionWithMemberVariable()
        {
            ICommand command = ParseCommand("class Foo { var Bar; }");

            Assert.IsNotNull(command);
            Assert.IsInstanceOfType(command, typeof(DefineClassCommand));
        }

        [TestMethod]
        public void ParseClassDefinitionWithInitializedMemberVariables()
        {
            ICommand command = ParseCommand("class Person { var Name = \"Adam\"; var Age = 800; }");

            Assert.IsNotNull(command);
            Assert.IsInstanceOfType(command, typeof(DefineClassCommand));
        }

        [TestMethod]
        public void ParseClassDefinitionWithMemberVariableAndMethod()
        {
            ICommand command = ParseCommand("class Foo { var Bar; function GetBar() { return Bar; }}");

            Assert.IsNotNull(command);
            Assert.IsInstanceOfType(command, typeof(DefineClassCommand));
        }

        [TestMethod]
        public void ParseClassDefinitionWithMemberVariableAndDefaultMethod()
        {
            ICommand command = ParseCommand("class Foo { var Bar; default function GetBar() { return Bar; }}");

            Assert.IsNotNull(command);
            Assert.IsInstanceOfType(command, typeof(DefineClassCommand));
        }

        [TestMethod]
        public void ParseEmptyObjectDefinition()
        {
            ICommand command = ParseCommand("object Foo { }");

            Assert.IsNotNull(command);
            Assert.IsInstanceOfType(command, typeof(DefineObjectCommand));
        }

        [TestMethod]
        public void ParseObjectDefinitionWithMemberVariable()
        {
            ICommand command = ParseCommand("object Foo { var Bar; }");

            Assert.IsNotNull(command);
            Assert.IsInstanceOfType(command, typeof(DefineObjectCommand));
        }

        [TestMethod]
        public void ParseObjectDefinitionWithInitializedMemberVariables()
        {
            ICommand command = ParseCommand("object Person { var Name = \"Adam\"; var Age = 800; }");

            Assert.IsNotNull(command);
            Assert.IsInstanceOfType(command, typeof(DefineObjectCommand));
        }

        [TestMethod]
        public void ParseObjectDefinitionWithMemberVariableAndMethod()
        {
            ICommand command = ParseCommand("object Foo { var Bar; function GetBar() { return Bar; }}");

            Assert.IsNotNull(command);
            Assert.IsInstanceOfType(command, typeof(DefineObjectCommand));
        }

        [TestMethod]
        public void ParseSetPropertyCommand()
        {
            ICommand command = ParseCommand("x.FirstName = \"Adam\";");

            Assert.IsNotNull(command);            
        }

        [TestMethod]
        public void ParseFunctionExpression()
        {
            IExpression expression = ParseExpression("function (x) return x;");

            Assert.IsNotNull(expression);
            Assert.IsInstanceOfType(expression, typeof(FunctionExpression));

            FunctionExpression funcexp = (FunctionExpression)expression;

            Assert.AreEqual(1, funcexp.ParameterNames.Length);
            Assert.AreEqual("x", funcexp.ParameterNames[0]);
            Assert.IsNotNull(funcexp.Body);
            Assert.IsInstanceOfType(funcexp.Body, typeof(ReturnCommand));
        }

        [TestMethod]
        public void ParsePreIncrementExpressionWithVariable()
        {
            IExpression expression = ParseExpression("++foo");

            Assert.IsNotNull(expression);
            Assert.IsInstanceOfType(expression, typeof(IncrementExpression));

            IncrementExpression incexpr = (IncrementExpression)expression;

            Assert.AreEqual(IncrementOperator.PreIncrement, incexpr.Operator);
            Assert.IsNotNull(incexpr.Expression);
            Assert.IsInstanceOfType(incexpr.Expression, typeof(VariableExpression));
        }

        [TestMethod]
        public void ParsePreDecrementExpressionWithDotName()
        {
            IExpression expression = ParseExpression("--adam.Age");

            Assert.IsNotNull(expression);
            Assert.IsInstanceOfType(expression, typeof(IncrementExpression));

            IncrementExpression incexpr = (IncrementExpression)expression;

            Assert.AreEqual(IncrementOperator.PreDecrement, incexpr.Operator);
            Assert.IsNotNull(incexpr.Expression);
            Assert.IsInstanceOfType(incexpr.Expression, typeof(DotExpression));
        }

        [TestMethod]
        public void ParsePostIncrementExpressionWithVariable()
        {
            IExpression expression = ParseExpression("foo++");

            Assert.IsNotNull(expression);
            Assert.IsInstanceOfType(expression, typeof(IncrementExpression));

            IncrementExpression incexpr = (IncrementExpression)expression;

            Assert.AreEqual(IncrementOperator.PostIncrement, incexpr.Operator);
            Assert.IsNotNull(incexpr.Expression);
            Assert.IsInstanceOfType(incexpr.Expression, typeof(VariableExpression));
        }

        [TestMethod]
        public void ParsePostDecrementExpressionWithDotName()
        {
            IExpression expression = ParseExpression("adam.Age--");

            Assert.IsNotNull(expression);
            Assert.IsInstanceOfType(expression, typeof(IncrementExpression));

            IncrementExpression incexpr = (IncrementExpression)expression;

            Assert.AreEqual(IncrementOperator.PostDecrement, incexpr.Operator);
            Assert.IsNotNull(incexpr.Expression);
            Assert.IsInstanceOfType(incexpr.Expression, typeof(DotExpression));
        }

        [TestMethod]
        public void ParseNewArray()
        {
            IExpression expression = ParseExpression("new System.Int32[10]");

            Assert.IsNotNull(expression);
            Assert.IsInstanceOfType(expression, typeof(NewArrayExpression));

            NewArrayExpression newarrexpr = (NewArrayExpression)expression;

            Assert.AreEqual("System.Int32", newarrexpr.TypeName);
            Assert.IsNotNull(newarrexpr.Arguments);
            Assert.AreEqual(1, newarrexpr.Arguments.Count);
        }

        [TestMethod]
        public void ParseNewArrayWithValues()
        {
            IExpression expression = ParseExpression("new int[] { 1, 2, 3 }");

            Assert.IsNotNull(expression);
            Assert.IsInstanceOfType(expression, typeof(InitializeArrayExpression));

            InitializeArrayExpression newarrexpr = (InitializeArrayExpression)expression;

            Assert.AreEqual("int", newarrexpr.TypeName);
            Assert.IsNotNull(newarrexpr.Values);
            Assert.AreEqual(3, newarrexpr.Values.Count);
        }

        [TestMethod]
        public void ParseArrayExpression()
        {
            IExpression expression = ParseExpression("numbers[1]");

            Assert.IsNotNull(expression);
            Assert.IsInstanceOfType(expression, typeof(ArrayExpression));

            ArrayExpression arrexpr = (ArrayExpression) expression;

            Assert.IsInstanceOfType(arrexpr.Expression, typeof(VariableExpression));

            VariableExpression varexpr = (VariableExpression) arrexpr.Expression;

            Assert.AreEqual("numbers", varexpr.VariableName);

            Assert.AreEqual(1, arrexpr.Arguments.Count);
            Assert.IsInstanceOfType(arrexpr.Arguments.First(), typeof(ConstantExpression));
        }

        [TestMethod]
        public void ParseSetArrayCommand()
        {
            ICommand command = ParseCommand("numbers[0] = 1;");

            Assert.IsNotNull(command);
            Assert.IsInstanceOfType(command, typeof(SetArrayCommand));

            SetArrayCommand setcmd = (SetArrayCommand) command;

            Assert.IsInstanceOfType(setcmd.LeftValue, typeof(VariableExpression));
            Assert.AreEqual(1, setcmd.Arguments.Count);
            Assert.IsInstanceOfType(setcmd.Expression, typeof(ConstantExpression));
        }

        [TestMethod]
        public void ParseSetArrayCommandWithDotExpression()
        {
            ICommand command = ParseCommand("foo.Values[0] = 1;");

            Assert.IsNotNull(command);
            Assert.IsInstanceOfType(command, typeof(SetArrayCommand));

            SetArrayCommand setcmd = (SetArrayCommand)command;

            Assert.IsInstanceOfType(setcmd.LeftValue, typeof(DotExpression));
            Assert.AreEqual(1, setcmd.Arguments.Count);
            Assert.IsInstanceOfType(setcmd.Expression, typeof(ConstantExpression));
        }

        [TestMethod]
        public void ParseStringWithExpression()
        {
            IExpression expression = ParseExpression("\"foo${bar}\"");

            Assert.IsNotNull(expression);
            Assert.IsInstanceOfType(expression, typeof(ConcatenateExpression));

            ConcatenateExpression cexpr = (ConcatenateExpression)expression;

            Assert.IsInstanceOfType(cexpr.LeftExpression, typeof(ConstantExpression));
            Assert.IsInstanceOfType(cexpr.RightExpression, typeof(VariableExpression));
        }

        [TestMethod]
        public void ParseNotExpression()
        {
            IExpression expression = ParseExpression("!k");

            Assert.IsNotNull(expression);
            Assert.IsInstanceOfType(expression, typeof(NotExpression));

            NotExpression notexpr = (NotExpression)expression;

            Assert.IsInstanceOfType(notexpr.Expression, typeof(VariableExpression));
        }

        [TestMethod]
        public void ParseAndExpression()
        {
            IExpression expression = ParseExpression("k==1 && l==1");

            Assert.IsNotNull(expression);
            Assert.IsInstanceOfType(expression, typeof(AndExpression));

            AndExpression andexpr = (AndExpression)expression;

            Assert.IsInstanceOfType(andexpr.LeftExpression, typeof(CompareExpression));
            Assert.IsInstanceOfType(andexpr.RightExpression, typeof(CompareExpression));
        }

        [TestMethod]
        public void ParseOrExpression()
        {
            IExpression expression = ParseExpression("k==1 || l==1");

            Assert.IsNotNull(expression);
            Assert.IsInstanceOfType(expression, typeof(OrExpression));

            OrExpression orexpr = (OrExpression)expression;

            Assert.IsInstanceOfType(orexpr.LeftExpression, typeof(CompareExpression));
            Assert.IsInstanceOfType(orexpr.RightExpression, typeof(CompareExpression));
        }

        [TestMethod]
        public void ParseOrAndExpression()
        {
            IExpression expression = ParseExpression("k==1 || l==1 && j==1");

            Assert.IsNotNull(expression);
            Assert.IsInstanceOfType(expression, typeof(OrExpression));

            OrExpression orexpr = (OrExpression)expression;

            Assert.IsInstanceOfType(orexpr.LeftExpression, typeof(CompareExpression));
            Assert.IsInstanceOfType(orexpr.RightExpression, typeof(AndExpression));
        }

        [TestMethod]
        public void ParseAndOrExpression()
        {
            IExpression expression = ParseExpression("k==1 && l==1 || j==1");

            Assert.IsNotNull(expression);
            Assert.IsInstanceOfType(expression, typeof(OrExpression));

            OrExpression orexpr = (OrExpression)expression;

            Assert.IsInstanceOfType(orexpr.LeftExpression, typeof(AndExpression));
            Assert.IsInstanceOfType(orexpr.RightExpression, typeof(CompareExpression));
        }

        [TestMethod]
        public void ParseGoWithSimpleCommand()
        {
            ICommand command = ParseCommand("go a=1;");

            Assert.IsNotNull(command);
            Assert.IsInstanceOfType(command, typeof(GoCommand));
        }

        [TestMethod]
        public void ParseChannelReceiveUsingOperator()
        {
            IExpression expression = ParseExpression("<- a");

            Assert.IsNotNull(expression);
            Assert.IsInstanceOfType(expression, typeof(GetValueExpression));

            GetValueExpression getvalexp = (GetValueExpression)expression;

            Assert.IsNotNull(getvalexp.Expression);
            Assert.IsInstanceOfType(getvalexp.Expression, typeof(VariableExpression));
        }

        [TestMethod]
        public void ParseChannelSendUsingOperator()
        {
            ICommand command = ParseCommand("a <- 10;");

            Assert.IsNotNull(command);
            Assert.IsInstanceOfType(command, typeof(SetValueCommand));

            SetValueCommand setvalcmd = (SetValueCommand)command;

            Assert.IsInstanceOfType(setvalcmd.Expression, typeof(ConstantExpression));
            Assert.IsInstanceOfType(setvalcmd.LeftValue, typeof(VariableExpression));
        }

        [TestMethod]
        public void ParseVarCommand()
        {
            ICommand command = ParseCommand("var x;");

            Assert.IsNotNull(command);
            Assert.IsInstanceOfType(command, typeof(VarCommand));

            VarCommand varcmd = (VarCommand)command;

            Assert.AreEqual("x", varcmd.Name);
            Assert.IsNull(varcmd.Expression);
        }

        [TestMethod]
        public void ParseVarCommandWithInitialValue()
        {
            ICommand command = ParseCommand("var x = 10;");

            Assert.IsNotNull(command);
            Assert.IsInstanceOfType(command, typeof(VarCommand));

            VarCommand varcmd = (VarCommand)command;

            Assert.AreEqual("x", varcmd.Name);
            Assert.IsNotNull(varcmd.Expression);
            Assert.IsInstanceOfType(varcmd.Expression, typeof(ConstantExpression));
        }

        [TestMethod]
        public void ParseGlobalCommand()
        {
            ICommand command = ParseCommand("global x;");

            Assert.IsNotNull(command);
            Assert.IsInstanceOfType(command, typeof(GlobalCommand));

            GlobalCommand glbcmd = (GlobalCommand)command;

            Assert.AreEqual("x", glbcmd.Name);
        }

        [TestMethod]
        public void ParseEmptyAgentDefinition()
        {
            ICommand command = ParseCommand("agent Foo { }");

            Assert.IsNotNull(command);
            Assert.IsInstanceOfType(command, typeof(DefineAgentCommand));
        }

        [TestMethod]
        public void ParseAgentDefinitionWithMemberVariable()
        {
            ICommand command = ParseCommand("agent Foo { var Bar; }");

            Assert.IsNotNull(command);
            Assert.IsInstanceOfType(command, typeof(DefineAgentCommand));
        }

        [TestMethod]
        public void ParseAgentDefinitionWithInitializedMemberVariables()
        {
            ICommand command = ParseCommand("agent Person { var Name = \"Adam\"; var Age = 800; }");

            Assert.IsNotNull(command);
            Assert.IsInstanceOfType(command, typeof(DefineAgentCommand));
        }

        [TestMethod]
        public void ParseAgentDefinitionWithMemberVariableAndMethod()
        {
            ICommand command = ParseCommand("agent Foo { var Bar; function GetBar() { return Bar; }}");

            Assert.IsNotNull(command);
            Assert.IsInstanceOfType(command, typeof(DefineAgentCommand));
        }

        [TestMethod]
        public void ParseSplat()
        {
            IExpression expression = ParseExpression("pars...");
            Assert.IsNotNull(expression);
            Assert.IsInstanceOfType(expression, typeof(VariableVariableExpression));
            Assert.AreEqual("pars", ((VariableVariableExpression)expression).VariableName);
        }

        private static IExpression ParseExpression(string text)
        {
            Parser parser = new Parser(text);

            IExpression expression = parser.ParseExpression();

            Assert.IsNull(parser.ParseExpression());

            return expression;
        }

        private static ICommand ParseCommand(string text)
        {
            Parser parser = new Parser(text);

            ICommand command = parser.ParseCommand();

            Assert.IsNull(parser.ParseExpression());

            return command;
        }
    }
}

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
            Assert.IsInstanceOfType(command, typeof(SetVariableCommand));

            SetVariableCommand setcmd = (SetVariableCommand)command;

            Assert.AreEqual("a", setcmd.VariableName);
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
            Assert.IsInstanceOfType(whilecmd.Command, typeof(SetVariableCommand));
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
            Assert.IsInstanceOfType(foreachcmd.Command, typeof(SetVariableCommand));
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
                Assert.IsInstanceOfType(cmd, typeof(SetVariableCommand));
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
            Assert.AreEqual("x", defcmd.ParameterNames[0]);
            Assert.IsNotNull(defcmd.Body);
            Assert.IsInstanceOfType(defcmd.Body, typeof(CompositeCommand));
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
        public void ParsePrintCommand()
        {
            ICommand command = ParseCommand("Print(1);");

            Assert.IsNotNull(command);
            Assert.IsInstanceOfType(command, typeof(InvokeCommand));
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

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
    public class CommandsTests
    {
        [TestMethod]
        public void ExecuteSetCommand()
        {
            BindingEnvironment environment = new BindingEnvironment();
            SetVariableCommand command = new SetVariableCommand("foo", new ConstantExpression("bar"));

            command.Execute(environment);

            Assert.AreEqual("bar", environment.GetValue("foo"));
        }

        [TestMethod]
        public void ExecuteCompositeCommand()
        {
            BindingEnvironment environment = new BindingEnvironment();

            SetVariableCommand command1 = new SetVariableCommand("foo", new ConstantExpression("bar"));
            SetVariableCommand command2 = new SetVariableCommand("one", new ConstantExpression(1));
            SetVariableCommand command3 = new SetVariableCommand("bar", new VariableExpression("foo"));

            List<ICommand> commands = new List<ICommand>();
            commands.Add(command1);
            commands.Add(command2);
            commands.Add(command3);

            CompositeCommand command = new CompositeCommand(commands);

            command.Execute(environment);

            Assert.AreEqual("bar", environment.GetValue("foo"));
            Assert.AreEqual(1, environment.GetValue("one"));
            Assert.AreEqual("bar", environment.GetValue("bar"));
        }

        [TestMethod]
        public void ExecuteIfCommandWhenTrue()
        {
            IExpression condition = new ConstantExpression(true);
            ICommand setCommand = new SetVariableCommand("x", new ConstantExpression(1));
            IfCommand command = new IfCommand(condition, setCommand);

            BindingEnvironment environment = new BindingEnvironment();

            command.Execute(environment);

            Assert.AreEqual(1, environment.GetValue("x"));
        }

        [TestMethod]
        public void ExecuteIfCommandWhenFalse()
        {
            IExpression condition = new ConstantExpression(false);
            ICommand setCommand = new SetVariableCommand("x", new ConstantExpression(1));
            IfCommand command = new IfCommand(condition, setCommand);

            BindingEnvironment environment = new BindingEnvironment();

            command.Execute(environment);

            Assert.IsNull(environment.GetValue("x"));
        }

        [TestMethod]
        public void ExecuteIfCommandElseWhenFalse()
        {
            IExpression condition = new ConstantExpression(false);
            ICommand setXCommand = new SetVariableCommand("x", new ConstantExpression(1));
            ICommand setYCommand = new SetVariableCommand("y", new ConstantExpression(2));
            IfCommand command = new IfCommand(condition, setXCommand, setYCommand);

            BindingEnvironment environment = new BindingEnvironment();

            command.Execute(environment);

            Assert.IsNull(environment.GetValue("x"));
            Assert.AreEqual(2, environment.GetValue("y"));
        }

        [TestMethod]
        public void ExecuteWhileCommand()
        {
            IExpression incrementX = new ArithmeticBinaryExpression(ArithmeticOperator.Add, new ConstantExpression(1), new VariableExpression("x"));
            IExpression decrementY = new ArithmeticBinaryExpression(ArithmeticOperator.Subtract, new VariableExpression("y"), new ConstantExpression(1));
            ICommand setX = new SetVariableCommand("x", incrementX);
            ICommand setY = new SetVariableCommand("y", decrementY);
            List<ICommand> commands = new List<ICommand>();
            commands.Add(setX);
            commands.Add(setY);
            ICommand command = new CompositeCommand(commands);
            IExpression yexpr = new VariableExpression("y");

            WhileCommand whilecmd = new WhileCommand(yexpr, command);

            BindingEnvironment environment = new BindingEnvironment();

            environment.SetValue("x", 0);
            environment.SetValue("y", 5);

            whilecmd.Execute(environment);

            Assert.AreEqual(0, environment.GetValue("y"));
            Assert.AreEqual(5, environment.GetValue("x"));
        }

        [TestMethod]
        public void ExecuteForEachCommand()
        {
            IExpression addToX = new ArithmeticBinaryExpression(ArithmeticOperator.Add, new VariableExpression("y"), new VariableExpression("x"));
            ICommand setX = new SetVariableCommand("x", addToX);
            IExpression values = new ConstantExpression(new int [] { 1, 2, 3 } );

            ForEachCommand foreachcmd = new ForEachCommand("y", values, setX);

            BindingEnvironment environment = new BindingEnvironment();

            environment.SetValue("x", 0);

            foreachcmd.Execute(environment);

            Assert.AreEqual(6, environment.GetValue("x"));
        }
    }
}

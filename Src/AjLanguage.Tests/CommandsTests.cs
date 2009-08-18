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
    }
}

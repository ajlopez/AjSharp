namespace AjLanguage.Tests
{
    using System;
    using System.IO;
    using System.Text;
    using System.Collections;
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
        public void ExecuteSetVariableCommand()
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

            environment.SetValue("foo", null);
            environment.SetValue("one", null);

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

        [TestMethod]
        public void ExecuteForCommand()
        {
            ICommand setX = new SetVariableCommand("x", new ConstantExpression(0));
            ICommand setY = new SetVariableCommand("y", new ConstantExpression(0));
            List<ICommand> commands = new List<ICommand>();
            commands.Add(setX);
            commands.Add(setY);
            ICommand initialCommand = new CompositeCommand(commands);

            IExpression condition = new CompareExpression(ComparisonOperator.Less, new VariableExpression("x"), new ConstantExpression(6));

            IExpression addXtoY = new ArithmeticBinaryExpression(ArithmeticOperator.Add, new VariableExpression("y"), new VariableExpression("x"));
            ICommand addToY = new SetVariableCommand("y", addXtoY);

            ICommand endCommand = new SetVariableCommand("x", new ArithmeticBinaryExpression(ArithmeticOperator.Add, new VariableExpression("x"), new ConstantExpression(1)));

            ForCommand forcmd = new ForCommand(initialCommand, condition, endCommand, addToY);

            BindingEnvironment environment = new BindingEnvironment();

            environment.SetValue("y", null);

            forcmd.Execute(environment);

            Assert.AreEqual(15, environment.GetValue("y"));
        }

        [TestMethod]
        public void ExecuteSetCommandWithVariable()
        {
            BindingEnvironment environment = new BindingEnvironment();
            SetCommand command = new SetCommand(new VariableExpression("foo"), new ConstantExpression("bar"));

            command.Execute(environment);

            Assert.AreEqual("bar", environment.GetValue("foo"));
        }

        [TestMethod]
        public void ExecuteSetCommandWithDotExpression()
        {
            BindingEnvironment environment = new BindingEnvironment();
            DotExpression dotexpr = new DotExpression(new VariableExpression("foo"), "FirstName");
            SetCommand command = new SetCommand(dotexpr, new ConstantExpression("bar"));

            command.Execute(environment);

            object obj = environment.GetValue("foo");

            Assert.IsNotNull(obj);
            Assert.IsInstanceOfType(obj, typeof(DynamicObject));

            DynamicObject dynobj = (DynamicObject)obj;

            Assert.AreEqual("bar", dynobj.GetValue("FirstName"));
        }

        [TestMethod]
        public void ExecuteSetCommandWithComplexDotExpression()
        {
            BindingEnvironment environment = new BindingEnvironment();
            DotExpression dot = new DotExpression(new VariableExpression("foo"), "Address");
            DotExpression dotexpr = new DotExpression(dot, "Street");
            SetCommand command = new SetCommand(dotexpr, new ConstantExpression("bar"));

            command.Execute(environment);

            object obj = environment.GetValue("foo");

            Assert.IsNotNull(obj);
            Assert.IsInstanceOfType(obj, typeof(DynamicObject));

            DynamicObject dynobj = (DynamicObject)obj;

            object obj2 = dynobj.GetValue("Address");

            Assert.IsNotNull(obj2);
            Assert.IsInstanceOfType(obj2, typeof(DynamicObject));

            DynamicObject dynobj2 = (DynamicObject)obj2;

            Assert.AreEqual("bar", dynobj2.GetValue("Street"));
        }

        [TestMethod]
        public void ExecuteSetCommandWithObjectProperty()
        {
            Machine machine = new Machine();
            TextWriter outwriter = new StringWriter();
            machine.Environment.SetValue("machine", machine);
            machine.Environment.SetValue("out", outwriter);

            DotExpression dotexpr = new DotExpression(new VariableExpression("machine"), "Out");
            SetCommand command = new SetCommand(dotexpr, new VariableExpression("out"));

            command.Execute(machine.Environment);

            Assert.AreEqual(machine.Out, outwriter);
        }

        [TestMethod]
        public void ExecuteSetArrayCommandWithVariable()
        {
            BindingEnvironment environment = new BindingEnvironment();
            SetArrayCommand command = new SetArrayCommand(new VariableExpression("foo"), new IExpression[] { new ConstantExpression(0) }, new ConstantExpression("bar"));

            command.Execute(environment);

            object result = environment.GetValue("foo");

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(IList));
            Assert.AreEqual(1, ((IList)result).Count);
            Assert.AreEqual("bar", ((IList)result)[0]);
        }

        [TestMethod]
        public void ExecuteSetArrayCommandWithDotExpression()
        {
            BindingEnvironment environment = new BindingEnvironment();
            DotExpression dotexpr = new DotExpression(new VariableExpression("foo"), "Values");
            SetArrayCommand command = new SetArrayCommand(dotexpr, new IExpression[] { new ConstantExpression(0) }, new ConstantExpression("bar"));

            command.Execute(environment);

            object obj = environment.GetValue("foo");

            Assert.IsNotNull(obj);
            Assert.IsInstanceOfType(obj, typeof(DynamicObject));

            DynamicObject dynobj = (DynamicObject)obj;

            object obj2 = dynobj.GetValue("Values");

            Assert.IsNotNull(obj2);
            Assert.IsInstanceOfType(obj2, typeof(IList));

            IList list = (IList)obj2;

            Assert.AreEqual(1, list.Count);
            Assert.AreEqual("bar", list[0]);
        }

        [TestMethod]
        public void ExecuteDefineObjectCommand()
        {
            DefineObjectCommand command = new DefineObjectCommand("adam", new string[] { "Age", "Name" }, new IExpression[] { new ConstantExpression(800), new ConstantExpression("Adam") });
            BindingEnvironment environment = new BindingEnvironment();

            command.Execute(environment);

            object result = environment.GetValue("adam");

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(DynamicObject));

            DynamicObject dynobj = (DynamicObject)result;

            Assert.AreEqual(800, dynobj.GetValue("Age"));
            Assert.AreEqual("Adam", dynobj.GetValue("Name"));
        }

        [TestMethod]
        public void ExecuteVarCommand()
        {
            VarCommand command = new VarCommand("foo", new ConstantExpression(10));

            IBindingEnvironment environment = new BindingEnvironment();
            IBindingEnvironment local = new LocalBindingEnvironment(environment);
            IBindingEnvironment local2 = new LocalBindingEnvironment(local);

            command.Execute(local2);

            Assert.IsTrue(local2.ContainsName("foo"));
            Assert.IsFalse(local.ContainsName("foo"));
            Assert.IsFalse(environment.ContainsName("foo"));

            Assert.AreEqual(10, local2.GetValue("foo"));
        }

        [TestMethod]
        public void ExecuteVarCommandWithNullExpression()
        {
            VarCommand command = new VarCommand("foo", null);

            IBindingEnvironment environment = new BindingEnvironment();
            IBindingEnvironment local = new LocalBindingEnvironment(environment);
            IBindingEnvironment local2 = new LocalBindingEnvironment(local);

            command.Execute(local2);

            Assert.IsTrue(local2.ContainsName("foo"));
            Assert.IsFalse(local.ContainsName("foo"));
            Assert.IsFalse(environment.ContainsName("foo"));

            Assert.IsNull(local2.GetValue("foo"));
        }

        [TestMethod]
        public void ExecuteGlobalCommand()
        {
            GlobalCommand command = new GlobalCommand("global");
            Machine machine = new Machine();

            IBindingEnvironment environment = new BindingEnvironment();

            command.Execute(environment);

            environment.SetValue("global", 100);

            Assert.IsFalse(environment.ContainsName("global"));
            Assert.IsTrue(machine.Environment.ContainsName("global"));
            Assert.AreEqual(100, machine.Environment.GetValue("global"));
        }

        [TestMethod]
        [ExpectedException(typeof(ExitException))]
        public void ExecuteExitCommand()
        {
            ExitCommand cmd = new ExitCommand();
            cmd.Execute(null);
        }
    }
}

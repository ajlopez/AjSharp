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
    public class DynamicClassTests
    {
        private DynamicClass dynclass;

        [TestInitialize]
        public void SetUp()
        {
            this.dynclass = new DynamicClass("MyClass");
        }

        [TestMethod]
        public void GetName()
        {
            Assert.AreEqual("MyClass", this.dynclass.Name);
        }

        [TestMethod]
        public void DefineMember() 
        {
            this.dynclass.SetMember("Name", null);
            Assert.IsNull(this.dynclass.GetMember("Name"));
        }

        [TestMethod]
        public void DefineMemberWithDefaulValue()
        {
            this.dynclass.SetMember("Name", "John");
            Assert.AreEqual("John", this.dynclass.GetMember("Name"));
        }

        [TestMethod]
        public void CreateNewEmptyInstance()
        {
            object instance = this.dynclass.NewInstance(null);

            Assert.IsNotNull(instance);

            Assert.IsInstanceOfType(instance, typeof(IClassicObject));

            Assert.AreEqual(this.dynclass, ((IClassicObject)instance).GetClass());
        }

        [TestMethod]
        public void CreateNewInstance()
        {
            this.dynclass.SetMember("Name", "Adam");
            this.dynclass.SetMember("Age", 800);

            object instance = this.dynclass.NewInstance(null);

            Assert.IsNotNull(instance);

            Assert.IsInstanceOfType(instance, typeof(IObject));

            IObject obj = (IObject)instance;

            Assert.AreEqual("Adam", obj.GetValue("Name"));
            Assert.AreEqual(800, obj.GetValue("Age"));
        }

        [TestMethod]
        public void CreateNewInstanceWithConstructor()
        {
            ICommand body = new SetCommand(new DotExpression(new VariableExpression("this"), "Name"), new VariableExpression("n"));
            Function function = new Function(new string [] { "n" }, body);
            this.dynclass.SetMember(this.dynclass.Name, function);
            this.dynclass.SetMember("Name", null);
            this.dynclass.SetMember("Age", 800);

            object instance = this.dynclass.NewInstance(new object[] { "Adam" });

            Assert.IsNotNull(instance);

            Assert.IsInstanceOfType(instance, typeof(IObject));

            IObject obj = (IObject)instance;

            Assert.AreEqual("Adam", obj.GetValue("Name"));
            Assert.AreEqual(800, obj.GetValue("Age"));
        }

        [TestMethod]
        public void InvokeMethod()
        {
            ICommand body = new ReturnCommand(new VariableExpression("Name"));
            Function function = new Function(null, body);

            this.dynclass.SetMember("Name", "Adam");
            this.dynclass.SetMember("Age", 800);
            this.dynclass.SetMember("GetName", function);

            Assert.AreEqual(0, function.Arity);

            object instance = this.dynclass.NewInstance(null);

            Assert.IsNotNull(instance);

            Assert.IsInstanceOfType(instance, typeof(IObject));

            IObject dynobj = (IObject)instance;

            object result = dynobj.Invoke("GetName", new object[] { });

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(string));
            Assert.AreEqual("Adam", result);
        }

        [TestMethod]
        public void InvokeDefaultMethod()
        {
            ICommand body = new ReturnCommand(new VariableExpression("Name"));
            Function function = new Function(new string[] { "name", "parameters" }, body, null, true);

            this.dynclass.SetMember("Name", "Adam");
            this.dynclass.SetMember("Age", 800);
            this.dynclass.SetMember("DefaultMethod", function);

            Assert.AreEqual(2, function.Arity);
            Assert.IsTrue(function.IsDefault);

            Assert.IsNotNull(this.dynclass.DefaultMember);
            Assert.AreEqual(this.dynclass.DefaultMember, function);

            object instance = this.dynclass.NewInstance(null);

            Assert.IsNotNull(instance);

            Assert.IsInstanceOfType(instance, typeof(IObject));

            IObject dynobj = (IObject)instance;

            object result = dynobj.Invoke("AnyName", new object[] { });

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(string));
            Assert.AreEqual("Adam", result);
        }

        [TestMethod]
        public void InvokeMethodRedefinedInObject()
        {
            ICommand body = new ReturnCommand(new VariableExpression("Age"));
            Function function = new Function(null, body);

            ICommand body2 = new ReturnCommand(new ConstantExpression(0));
            Function function2 = new Function(null, body2);

            this.dynclass.SetMember("Name", "Adam");
            this.dynclass.SetMember("Age", 800);
            this.dynclass.SetMember("GetAge", function);

            Assert.AreEqual(0, function.Arity);

            object instance = this.dynclass.NewInstance(null);

            Assert.IsNotNull(instance);

            Assert.IsInstanceOfType(instance, typeof(IObject));

            IObject dynobj = (IObject)instance;

            dynobj.SetValue("GetAge", function2);

            object result = dynobj.Invoke("GetAge", new object[] { });

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(int));
            Assert.AreEqual(0, result);
        }
    }
}


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
            this.dynclass = new DynamicClass();
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
    }
}


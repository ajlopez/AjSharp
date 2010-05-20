namespace AjLanguage.Tests
{
    using System;
    using System.Text;
    using System.Collections.Generic;
    using System.Linq;

    using AjLanguage;
    using AjLanguage.Commands;
    using AjLanguage.Expressions;
    using AjLanguage.Language;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DynamicObjectTests
    {
        private DynamicObject dynobj;
        private Machine machine;

        [TestInitialize]
        public void SetupDynamicObject()
        {
            machine = new Machine();
            dynobj = new DynamicObject();
        }

        [TestMethod]
        public void GetNullForUndefinedValue()
        {
            Assert.IsNull(dynobj.GetValue("Foo"));
        }

        [TestMethod]
        public void SetAndGetValue()
        {
            dynobj.SetValue("Foo", "Bar");

            Assert.AreEqual("Bar", dynobj.GetValue("Foo"));
        }

        [TestMethod]
        public void GetNames()
        {
            dynobj.SetValue("FirstName", "Adam");
            dynobj.SetValue("LastName", "Genesis");

            ICollection<string> names = dynobj.GetNames();

            Assert.IsNotNull(names);
            Assert.AreEqual(2, names.Count);

            Assert.IsTrue(names.Contains("FirstName"));
            Assert.IsTrue(names.Contains("LastName"));
        }

        [TestMethod]
        public void DefineMethod()
        {
            ICommand body = new ReturnCommand(new VariableExpression("Name"));
            Function function = new Function(null, body);

            Assert.AreEqual(0, function.Arity);

            dynobj.SetValue("GetName", function);

            object result = dynobj.GetValue("GetName");

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ICallable));
            Assert.IsTrue(result == function);
        }

        [TestMethod]
        public void InvokeMethod()
        {
            ICommand body = new ReturnCommand(new VariableExpression("Name"));
            Function function = new Function(null, body);

            Assert.AreEqual(0, function.Arity);

            dynobj.SetValue("Name", "Adam");
            dynobj.SetValue("GetName", function);

            object result = dynobj.Invoke("GetName", new object[] { });

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(string));
            Assert.AreEqual("Adam", result);
        }

        [TestMethod]
        public void InvokeNativeMethod()
        {
            ICommand body = new ReturnCommand(new VariableExpression("Name"));
            Function function = new Function(null, body);

            Assert.AreEqual(0, function.Arity);

            dynobj.SetValue("Name", "Adam");

            object result = dynobj.Invoke("GetValue", new object[] { "Name" });

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(string));
            Assert.AreEqual("Adam", result);
        }

        [TestMethod]
        public void DetectNativeMethods()
        {
            Assert.IsTrue(this.dynobj.IsNativeMethod("ToString"));
            Assert.IsTrue(this.dynobj.IsNativeMethod("GetHashCode"));
            Assert.IsTrue(this.dynobj.IsNativeMethod("Equals"));

            Assert.IsTrue(this.dynobj.IsNativeMethod("GetValue"));
            Assert.IsTrue(this.dynobj.IsNativeMethod("SetValue"));
            Assert.IsTrue(this.dynobj.IsNativeMethod("GetNames"));
            Assert.IsTrue(this.dynobj.IsNativeMethod("Invoke"));
            Assert.IsTrue(this.dynobj.IsNativeMethod("Marshal"));

            Assert.IsFalse(this.dynobj.IsNativeMethod("Foo"));
        }

        [TestMethod]
        public void MarshalDynamicObject()
        {
            this.dynobj.SetValue("Name", "Adam");
            this.dynobj.SetValue("Age", 800);

            IObject proxy = this.dynobj.Marshal();

            Assert.IsInstanceOfType(proxy, typeof(MarshalByRefObject));

            Assert.AreEqual("Adam", proxy.GetValue("Name"));
            Assert.AreEqual(800, proxy.GetValue("Age"));
        }
    }
}

namespace AjSharp.Tests
{
    using System;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using AjLanguage.Language;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class AjSharpMachineTests
    {
        private AjSharpMachine machine;

        [TestInitialize]
        public void Setup()
        {
            this.machine = new AjSharpMachine();
        }

        [TestMethod]
        public void NativeTypesAreDefined()
        {
            this.IsType("int", typeof(System.Int32));
            this.IsType("short", typeof(System.Int16));
            this.IsType("long", typeof(System.Int64));
            this.IsType("byte", typeof(System.Byte));
            this.IsType("char", typeof(System.Char));
            this.IsType("float", typeof(System.Single));
            this.IsType("double", typeof(System.Double));
            this.IsType("decimal", typeof(System.Decimal));
            this.IsType("bool", typeof(System.Boolean));
            this.IsType("object", typeof(System.Object));
        }

        [TestMethod]
        public void TypeAliasAreDefined()
        {
            this.IsType("List", typeof(ArrayList));
            this.IsType("Dictionary", typeof(Hashtable));
            this.IsType("DynamicObject", typeof(DynamicObject));
            this.IsType("DynamicClass", typeof(DynamicClass));
        }

        [TestMethod]
        public void NewTypesAreDefined()
        {
            this.IsType("Channel", typeof(Channel));
            this.IsType("QueueChannel", typeof(QueueChannel));
            this.IsType("Future", typeof(Future));
        }

        [TestMethod]
        public void PrimitiveFunctionsAreDefined()
        {
            this.IsCallable("Print");
            this.IsCallable("PrintLine");
            this.IsCallable("Include");
            this.IsCallable("Evaluate");
            this.IsCallable("Execute");
        }

        private void IsCallable(string name)
        {
            object obj = this.machine.Environment.GetValue(name);

            Assert.IsNotNull(obj);
            Assert.IsInstanceOfType(obj, typeof(ICallable));
        }

        private void IsType(string typename, Type type)
        {
            object obj = this.machine.Environment.GetValue(typename);

            Assert.IsNotNull(obj);
            Assert.IsInstanceOfType(obj, typeof(Type));
            Assert.AreEqual(type, obj);
        }
    }
}

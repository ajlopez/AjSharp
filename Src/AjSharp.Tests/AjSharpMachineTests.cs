namespace AjSharp.Tests
{
    using System;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using AjLanguage.Language;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using AjLanguage;
    using System.Reflection;

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

        [TestMethod]
        public void MachineIsDefinedAsType()
        {
            this.IsType("Machine", typeof(AjSharp.AjSharpMachine));
        }

        [TestMethod]
        public void GetCurrentUsingTypeUtilities()
        {
            MethodInfo[] methods = typeof(AjSharpMachine).GetMethods(System.Reflection.BindingFlags.FlattenHierarchy | System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.GetField | System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.InvokeMethod | System.Reflection.BindingFlags.Static);

            foreach (MethodInfo method in methods)
                ;

            Assert.AreEqual(this.machine, TypeUtilities.InvokeTypeMember(typeof(AjSharp.AjSharpMachine), "Current", null));
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
            //Assert.IsInstanceOfType(obj, type);
            Assert.AreEqual(type, obj);
        }
    }
}

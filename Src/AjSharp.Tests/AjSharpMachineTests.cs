namespace AjSharp.Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using AjLanguage;
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
            this.IsType("int", typeof(int));
            this.IsType("short", typeof(short));
            this.IsType("long", typeof(long));
            this.IsType("byte", typeof(byte));
            this.IsType("char", typeof(char));
            this.IsType("float", typeof(float));
            this.IsType("double", typeof(double));
            this.IsType("decimal", typeof(decimal));
            this.IsType("bool", typeof(bool));
            this.IsType("object", typeof(object));
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
                Assert.IsNotNull(method);

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
            Assert.AreEqual(type, obj);
        }
    }
}

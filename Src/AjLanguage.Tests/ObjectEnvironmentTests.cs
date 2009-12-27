using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

using AjLanguage.Language;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AjLanguage.Tests
{
    [TestClass]
    public class ObjectEnvironmentTests
    {
        private DynamicObject dynobj;
        private ObjectEnvironment environment;

        [TestInitialize]
        public void SetUp()
        {
            this.dynobj = new DynamicObject();
            this.environment = new ObjectEnvironment(dynobj);
        }

        [TestMethod]
        public void SetAndRetrieveValue()
        {
            this.environment.SetValue("Name", "Adam");

            Assert.AreEqual("Adam", this.environment.GetValue("Name"));
            Assert.AreEqual("Adam", this.dynobj.GetValue("Name"));
        }

        [TestMethod]
        public void GetThisObject()
        {
            Assert.AreEqual(this.dynobj, this.environment.GetValue(ObjectEnvironment.ThisName));
        }

        [TestMethod]
        public void SetValueThruBindingEnvironmentIfDefinedInObject()
        {
            this.environment.SetValue("Age", null);

            BindingEnvironment binding = new BindingEnvironment(this.environment);

            binding.SetValue("Age", 800);
            binding.SetValue("Local", 100);

            Assert.AreEqual(800, binding.GetValue("Age"));
            Assert.AreEqual(100, binding.GetValue("Local"));
            Assert.AreEqual(800, this.environment.GetValue("Age"));
            Assert.IsNull(this.environment.GetValue("Local"));
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void RaiseOnSetLocalValue()
        {
            this.environment.SetLocalValue("local", 100);
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void RaiseOnDefineGlobal()
        {
            this.environment.DefineGlobal("global");
        }
    }
}

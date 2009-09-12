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
    }
}

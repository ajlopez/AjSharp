namespace AjLanguage.Tests
{
    using System;
    using System.Text;
    using System.Collections.Generic;
    using System.Linq;

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
    }
}


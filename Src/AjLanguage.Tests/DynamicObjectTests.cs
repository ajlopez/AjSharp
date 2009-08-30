namespace AjLanguage.Tests
{
    using System;
    using System.Text;
    using System.Collections.Generic;
    using System.Linq;

    using AjLanguage;
    using AjLanguage.Language;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DynamicObjectTests
    {
        private DynamicObject dynobj;

        [TestInitialize]
        public void SetupDynamicObject()
        {
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
    }
}

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
    public class ObjectUtilitiesTests
    {
        [TestMethod]
        public void GetPropertyFromString()
        {
            Assert.AreEqual(3, ObjectUtilities.GetValue("foo", "Length"));
        }

        [TestMethod]
        public void GetValueUsingCall()
        {
            Assert.AreEqual("oo", ObjectUtilities.GetValue("foo", "Substring", new object[] { 1 }));
        }

        [TestMethod]
        public void GetValueFromDynamicObject()
        {
            DynamicObject dynobj = new DynamicObject();
            dynobj.SetValue("FirstName", "Adam");

            Assert.AreEqual("Adam", ObjectUtilities.GetValue(dynobj, "FirstName"));
        }

        [TestMethod]
        public void IsNumber()
        {
            Assert.IsTrue(ObjectUtilities.IsNumber((byte) 1));
            Assert.IsTrue(ObjectUtilities.IsNumber((short) 2));
            Assert.IsTrue(ObjectUtilities.IsNumber((int) 3));
            Assert.IsTrue(ObjectUtilities.IsNumber((long) 4));
            Assert.IsTrue(ObjectUtilities.IsNumber((float) 1.2));
            Assert.IsTrue(ObjectUtilities.IsNumber((double) 2.3));

            Assert.IsFalse(ObjectUtilities.IsNumber(null));
            Assert.IsFalse(ObjectUtilities.IsNumber("foo"));
            Assert.IsFalse(ObjectUtilities.IsNumber('a'));
            Assert.IsFalse(ObjectUtilities.IsNumber(this));
        }
    }
}

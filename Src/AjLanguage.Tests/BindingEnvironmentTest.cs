namespace AjLanguage.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class BindingEnvironmentTest
    {
        [TestMethod]
        public void CanCreate()
        {
            BindingEnvironment environment = new BindingEnvironment();

            Assert.IsNotNull(environment);
        }

        [TestMethod]
        public void SetAndGetValue() 
        {
            BindingEnvironment environment = new BindingEnvironment();

            environment.SetValue("foo", "bar");

            Assert.AreEqual("bar", environment.GetValue("foo"));
        }

        [TestMethod]
        public void GetNullIfUnknownName()
        {
            BindingEnvironment environment = new BindingEnvironment();

            Assert.IsNull(environment.GetValue("foo"));
        }

        [TestMethod]
        public void GetValueDefinedInParent()
        {
            BindingEnvironment parent = new BindingEnvironment();
            parent.SetValue("foo", "bar");
            BindingEnvironment environment = new BindingEnvironment(parent);

            Assert.AreEqual("bar", environment.GetValue("foo"));
        }

        [TestMethod]
        public void SetValueDefinedInParent()
        {
            BindingEnvironment parent = new BindingEnvironment();
            parent.SetValue("foo", "bar");
            BindingEnvironment environment = new BindingEnvironment(parent);
            environment.SetValue("foo", "newbar");

            Assert.AreEqual("newbar", environment.GetValue("foo"));
            Assert.AreEqual("bar", parent.GetValue("foo"));
        }

        [TestMethod]
        public void SetLocalValue()
        {
            BindingEnvironment parent = new BindingEnvironment();
            parent.SetValue("foo", "bar");
            BindingEnvironment environment = new BindingEnvironment(parent);
            environment.SetValue("foo", "newbar");

            Assert.AreEqual("newbar", environment.GetValue("foo"));
            Assert.AreEqual("bar", parent.GetValue("foo"));
        }

        [TestMethod]
        public void ContainsName()
        {
            BindingEnvironment environment = new BindingEnvironment();

            Assert.IsFalse(environment.ContainsName("foo"));

            environment.SetValue("foo", null);

            Assert.IsTrue(environment.ContainsName("foo"));

            environment.SetValue("bar", "foo");

            Assert.IsTrue(environment.ContainsName("bar"));
        }

        [TestMethod]
        public void SetValueThruLocalEnviroment()
        {
            BindingEnvironment environment = new BindingEnvironment();
            environment.SetValue("one", 0);

            LocalBindingEnvironment local = new LocalBindingEnvironment(environment);
            local.SetValue("one", 1);

            Assert.AreEqual(1, local.GetValue("one"));
            Assert.AreEqual(1, environment.GetValue("one"));
        }

        [TestMethod]
        public void SetUndefinedValueUsingTwoLocalEnviroments()
        {
            BindingEnvironment environment = new BindingEnvironment();

            LocalBindingEnvironment local = new LocalBindingEnvironment(environment);
            LocalBindingEnvironment local2 = new LocalBindingEnvironment(local);

            local2.SetValue("one", 1);

            Assert.AreEqual(1, local.GetValue("one"));
            Assert.AreEqual(1, local2.GetValue("one"));
            Assert.IsNull(environment.GetValue("one"));
        }
    }
}


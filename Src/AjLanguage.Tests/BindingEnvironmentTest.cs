﻿namespace AjLanguage.Tests
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
    }
}


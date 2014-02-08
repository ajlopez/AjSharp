﻿namespace AjLanguage.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using AjLanguage;
    using AjLanguage.Expressions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class PredicatesTests
    {
        [TestMethod]
        public void IsFalsePositives()
        {
            Assert.IsTrue(Predicates.IsFalse(null));
            Assert.IsTrue(Predicates.IsFalse(false));
            Assert.IsTrue(Predicates.IsFalse(string.Empty));
            Assert.IsTrue(Predicates.IsFalse(0));
            Assert.IsTrue(Predicates.IsFalse((long)0));
            Assert.IsTrue(Predicates.IsFalse((short)0));
            Assert.IsTrue(Predicates.IsFalse((double)0.0));
            Assert.IsTrue(Predicates.IsFalse((float)0.0));
        }

        [TestMethod]
        public void IsFalseNegatives()
        {
            Assert.IsFalse(Predicates.IsFalse("0"));
            Assert.IsFalse(Predicates.IsFalse(true));
            Assert.IsFalse(Predicates.IsFalse(1));
            Assert.IsFalse(Predicates.IsFalse((long)2));
            Assert.IsFalse(Predicates.IsFalse((short)3));
            Assert.IsFalse(Predicates.IsFalse((double)4.0));
            Assert.IsFalse(Predicates.IsFalse((float)1.2));
            Assert.IsFalse(Predicates.IsFalse(new BindingEnvironment()));
        }

        [TestMethod]
        public void IsTrueNegatives()
        {
            Assert.IsFalse(Predicates.IsTrue(null));
            Assert.IsFalse(Predicates.IsTrue(false));
            Assert.IsFalse(Predicates.IsTrue(string.Empty));
            Assert.IsFalse(Predicates.IsTrue(0));
            Assert.IsFalse(Predicates.IsTrue((long)0));
            Assert.IsFalse(Predicates.IsTrue((short)0));
            Assert.IsFalse(Predicates.IsTrue((double)0.0));
            Assert.IsFalse(Predicates.IsTrue((float)0.0));
        }

        [TestMethod]
        public void IsTruePositives()
        {
            Assert.IsTrue(Predicates.IsTrue("0"));
            Assert.IsTrue(Predicates.IsTrue(true));
            Assert.IsTrue(Predicates.IsTrue(1));
            Assert.IsTrue(Predicates.IsTrue((long)2));
            Assert.IsTrue(Predicates.IsTrue((short)3));
            Assert.IsTrue(Predicates.IsTrue((double)4.0));
            Assert.IsTrue(Predicates.IsTrue((float)1.2));
            Assert.IsTrue(Predicates.IsTrue(new ConstantExpression(1)));
        }
    }
}

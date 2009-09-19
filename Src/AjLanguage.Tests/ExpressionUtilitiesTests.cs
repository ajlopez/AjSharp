namespace AjLanguage.Tests
{
    using System;
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using AjLanguage.Expressions;
    using AjLanguage.Language;

    [TestClass]
    public class ExpressionUtilitiesTests
    {
        [TestMethod]
        public void ResolveVariableExpressionToObject()
        {
            BindingEnvironment environment = new BindingEnvironment();
            IExpression expression = new VariableExpression("foo");

            object obj = ExpressionUtilities.ResolveToObject(expression, environment);

            Assert.IsNotNull(obj);
            Assert.IsInstanceOfType(obj, typeof(IObject));

            Assert.AreEqual(obj, environment.GetValue("foo"));
        }

        [TestMethod]
        public void ResolveDotExpressionToObject()
        {
            BindingEnvironment environment = new BindingEnvironment();
            IExpression expression = new DotExpression(new VariableExpression("Project"), "Title");

            object obj = ExpressionUtilities.ResolveToObject(expression, environment);

            Assert.IsNotNull(obj);
            Assert.IsInstanceOfType(obj, typeof(IObject));

            object project = environment.GetValue("Project");

            Assert.IsNotNull(project);
            Assert.IsInstanceOfType(project, typeof(IObject));

            object title = ((IObject)project).GetValue("Title");

            Assert.IsNotNull(title);
            Assert.IsInstanceOfType(title, typeof(IObject));

            Assert.AreEqual(obj, title);
        }

        [TestMethod]
        public void ResolveDotExpressionToList()
        {
            BindingEnvironment environment = new BindingEnvironment();
            IExpression expression = new DotExpression(new VariableExpression("Project"), "Entities");

            object obj = ExpressionUtilities.ResolveToList(expression, environment);

            Assert.IsNotNull(obj);
            Assert.IsInstanceOfType(obj, typeof(IList));

            object project = environment.GetValue("Project");

            Assert.IsNotNull(project);
            Assert.IsInstanceOfType(project, typeof(IObject));

            object entities = ((IObject)project).GetValue("Entities");

            Assert.IsNotNull(entities);
            Assert.IsInstanceOfType(entities, typeof(IList));

            Assert.AreEqual(obj, entities);
        }
    }
}

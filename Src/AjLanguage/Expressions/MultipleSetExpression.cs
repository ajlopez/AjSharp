namespace AjLanguage.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using AjLanguage.Language;

    public class MultipleSetExpression : IExpression
    {
        private IExpression leftObject;
        private string[] propertyNames;
        private ICollection<IExpression> expressions;

        public MultipleSetExpression(IExpression leftObject, string[] propertyNames, ICollection<IExpression> expressions)
        {
            if (propertyNames.Length != expressions.Count)
                throw new InvalidOperationException("Invalid number of names or expressions");

            this.leftObject = leftObject;
            this.propertyNames = propertyNames;
            this.expressions = expressions;
        }

        public IExpression LeftObject { get { return this.leftObject; } }

        public string[] PropertyNames { get { return this.propertyNames; } }

        public ICollection<IExpression> Expressions { get { return this.expressions; } }

        public object Evaluate(BindingEnvironment environment)
        {
            object obj = this.leftObject.Evaluate(environment);

            int k=0;

            foreach (IExpression expression in this.expressions) 
            {
                string name = propertyNames[k++];

                object value = expression.Evaluate(environment);

                ObjectUtilities.SetValue(obj, name, value);
            }

            return obj;
        }
    }
}

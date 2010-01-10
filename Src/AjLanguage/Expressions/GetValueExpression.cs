namespace AjLanguage.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using AjLanguage.Language;

    public class GetValueExpression : IExpression
    {
        private IExpression expression;

        public GetValueExpression(IExpression expression)
        {
            this.expression = expression;
        }

        public IExpression Expression { get { return this.expression; } }

        public object Evaluate(IBindingEnvironment environment)
        {
            if (this.expression == null)
                return null;

            object obj = this.expression.Evaluate(environment);

            if (obj == null)
                return null;

            return ((IReference)obj).GetValue();
        }
    }
}

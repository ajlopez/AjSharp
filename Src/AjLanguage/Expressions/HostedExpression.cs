namespace AjLanguage.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using AjLanguage.Commands;
    using AjLanguage.Language;
    using System.Collections;
    using AjLanguage.Hosting;

    [Serializable]
    public class HostedExpression : IExpression
    {
        private IExpression expression;
        private IExpression hostexpression;

        public HostedExpression(IExpression expression, IExpression hostexpression)
        {
            this.expression = expression;
            this.hostexpression = hostexpression;
        }

        public IExpression Expression { get { return this.expression; } }

        public IExpression HostExpression { get { return this.hostexpression; } }

        public object Evaluate(IBindingEnvironment environment)
        {
            IHost host = (IHost)this.hostexpression.Evaluate(environment);
            return host.Evaluate(this.expression);
        }
    }
}

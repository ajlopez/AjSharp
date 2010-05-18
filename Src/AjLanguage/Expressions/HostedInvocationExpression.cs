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
    public class HostedInvocationExpression : IExpression
    {
        private IExpression hostexpression;
        private IExpression expression;
        private ICollection<IExpression> arguments;

        public HostedInvocationExpression(IExpression expression, ICollection<IExpression> arguments, IExpression hostexpression)
        {
            this.hostexpression = hostexpression;
            this.expression = expression;
            this.arguments = arguments;
        }

        public IExpression Expression { get { return this.expression; } }

        public ICollection<IExpression> Arguments { get { return this.arguments; } }

        public IExpression HostExpression { get { return this.hostexpression; } }

        public object Evaluate(IBindingEnvironment environment)
        {
            ICallable callable;

            callable = (ICallable)this.expression.Evaluate(environment);

            List<object> parameters = new List<object>();

            foreach (IExpression expression in this.arguments)
            {
                object parameter = expression.Evaluate(environment);

                if (expression is VariableVariableExpression)
                {
                    if (parameter != null)
                        foreach (object ob in (IEnumerable) parameter)
                            parameters.Add(ob);
                }
                else
                    parameters.Add(parameter);
            }

            IHost host = (IHost)this.hostexpression.Evaluate(environment);

            return host.Invoke(callable, parameters.ToArray());
        }
    }
}

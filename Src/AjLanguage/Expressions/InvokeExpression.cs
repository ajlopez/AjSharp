namespace AjLanguage.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using AjLanguage.Commands;

    public class InvokeExpression : IExpression
    {
        private string name;
        private ICollection<IExpression> arguments;

        public InvokeExpression(string name, ICollection<IExpression> arguments)
        {
            this.name = name;
            this.arguments = arguments;
        }

        public object Evaluate(BindingEnvironment environment)
        {
            ICallable callable = (ICallable) environment.GetValue(this.name);

            List<object> parameters = new List<object>();

            foreach (IExpression expression in this.arguments)
                parameters.Add(expression.Evaluate(environment));

            // TODO get global environment here
            return callable.Invoke(environment, parameters.ToArray());
        }
    }
}

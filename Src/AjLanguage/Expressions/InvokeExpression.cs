namespace AjLanguage.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using AjLanguage.Commands;
    using AjLanguage.Language;

    public class InvokeExpression : IExpression
    {
        private string name;
        private ICollection<IExpression> arguments;

        public InvokeExpression(string name, ICollection<IExpression> arguments)
        {
            this.name = name;
            this.arguments = arguments;
        }

        public string Name { get { return this.name; } }

        public ICollection<IExpression> Arguments { get { return this.arguments; } }

        public object Evaluate(IBindingEnvironment environment)
        {
            ICallable callable = (ICallable) environment.GetValue(this.name);

            if (callable == null)
                callable = (ICallable)Machine.Current.Environment.GetValue(this.name);

            List<object> parameters = new List<object>();

            foreach (IExpression expression in this.arguments)
                parameters.Add(expression.Evaluate(environment));

            if (callable is ILocalCallable)
                return callable.Invoke(environment, parameters.ToArray());

            return callable.Invoke(parameters.ToArray());
        }
    }
}

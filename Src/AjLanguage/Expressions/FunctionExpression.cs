namespace AjLanguage.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using AjLanguage.Commands;
    using AjLanguage.Language;

    public class FunctionExpression : IExpression
    {
        private string[] parameterNames;
        private ICommand body;
        private bool isdefault;

        public FunctionExpression(string[] parameterNames, ICommand body)
            : this(parameterNames, body, false)
        {
        }

        public FunctionExpression(string[] parameterNames, ICommand body, bool isdefault)
        {
            this.parameterNames = parameterNames;
            this.body = body;
            this.isdefault = isdefault;
        }

        public string[] ParameterNames { get { return this.parameterNames; } }

        public ICommand Body { get { return this.body; } }

        public object Evaluate(IBindingEnvironment environment)
        {
            return new Function(this.parameterNames, this.body, environment, this.isdefault);
        }
    }
}

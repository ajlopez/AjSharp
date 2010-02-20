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
        private bool hasvariableparameters;

        public FunctionExpression(string[] parameterNames, ICommand body)
            : this(parameterNames, body, false, false)
        {
        }

        public FunctionExpression(string[] parameterNames, ICommand body, bool isdefault, bool hasvariableparameters)
        {
            this.parameterNames = parameterNames;
            this.body = body;
            this.isdefault = isdefault;
            this.hasvariableparameters = hasvariableparameters;
        }

        public string[] ParameterNames { get { return this.parameterNames; } }

        public ICommand Body { get { return this.body; } }

        public bool IsDefault { get { return this.isdefault; } }

        public bool HasVariableParameters { get { return this.hasvariableparameters; } }

        public object Evaluate(IBindingEnvironment environment)
        {
            return new Function(this.parameterNames, this.body, environment, this.isdefault, this.hasvariableparameters);
        }
    }
}

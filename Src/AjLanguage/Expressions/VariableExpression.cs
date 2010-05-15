namespace AjLanguage.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    [Serializable]
    public class VariableExpression : IExpression
    {
        private string name;

        public VariableExpression(string name)
        {
            this.name = name;
        }

        public string VariableName { get { return this.name; } }

        public object Evaluate(IBindingEnvironment environment)
        {
            return environment.GetValue(this.name);
        }
    }
}

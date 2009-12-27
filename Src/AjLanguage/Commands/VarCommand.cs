namespace AjLanguage.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using AjLanguage.Expressions;

    public class VarCommand : ICommand
    {
        private string name;
        private IExpression expression;

        public VarCommand(string name, IExpression expression)
        {
            this.name = name;
            this.expression = expression;
        }

        public string Name { get { return this.name; } }

        public IExpression Expression { get { return this.expression; } }

        public void Execute(IBindingEnvironment environment)
        {
            object value = null;

            if (this.expression != null)
                value = this.expression.Evaluate(environment);

            environment.SetLocalValue(this.name, value);
        }
    }
}

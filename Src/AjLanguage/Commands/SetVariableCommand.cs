namespace AjLanguage.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using AjLanguage.Expressions;

    public class SetVariableCommand : ICommand
    {
        private string name;
        private IExpression expression;

        public SetVariableCommand(string name, IExpression expression)
        {
            this.name = name;
            this.expression = expression;
        }

        public void Execute(BindingEnvironment environment)
        {
            environment.SetValue(this.name, this.expression.Evaluate(environment));
        }
    }
}

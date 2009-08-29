namespace AjLanguage.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using AjLanguage.Expressions;

    public class InvokeCommand : ICommand
    {
        private InvokeExpression expression;

        public InvokeCommand(InvokeExpression expression)
        {
            this.expression = expression;
        }

        public void Execute(BindingEnvironment environment)
        {
            this.expression.Evaluate(environment);
        }
    }
}

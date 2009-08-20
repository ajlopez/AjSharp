namespace AjLanguage.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using AjLanguage.Expressions;

    public class WhileCommand : ICommand
    {
        private IExpression condition;
        private ICommand command;

        public WhileCommand(IExpression condition, ICommand command)
        {
            this.condition = condition;
            this.command = command;
        }

        public void Execute(BindingEnvironment environment)
        {
            while (Predicates.IsTrue(this.condition.Evaluate(environment)))
                this.command.Execute(environment);
        }
    }
}

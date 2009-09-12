namespace AjLanguage.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using AjLanguage.Expressions;

    public class IfCommand : ICommand
    {
        private IExpression condition;
        private ICommand thenCommand;
        private ICommand elseCommand;

        public IfCommand(IExpression condition, ICommand thenCommand)
            : this(condition, thenCommand, null)
        {
        }

        public IfCommand(IExpression condition, ICommand thenCommand, ICommand elseCommand)
        {
            this.condition = condition;
            this.thenCommand = thenCommand;
            this.elseCommand = elseCommand;
        }

        public IExpression Condition { get { return this.condition; } }

        public ICommand ThenCommand { get { return this.thenCommand; } }

        public ICommand ElseCommand { get { return this.elseCommand; } }

        public void Execute(IBindingEnvironment environment)
        {
            object result = this.condition.Evaluate(environment);

            if (Predicates.IsTrue(result))
                this.thenCommand.Execute(environment);
            else if (this.elseCommand != null)
                this.elseCommand.Execute(environment);
        }
    }
}

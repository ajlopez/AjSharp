namespace AjLanguage.Commands
{
    using System;
    using System.Collections;
    using System.Linq;
    using System.Text;

    using AjLanguage.Expressions;

    public class ForCommand : ICommand
    {
        private ICommand initialCommand;
        private IExpression condition;
        private ICommand body;
        private ICommand endCommand;

        public ForCommand(ICommand initialCommand, IExpression condition, ICommand endCommand, ICommand body)
        {
            this.initialCommand = initialCommand;
            this.condition = condition;
            this.endCommand = endCommand;
            this.body = body;
        }

        public ICommand InitialCommand { get { return this.initialCommand; } }

        public IExpression Condition { get { return this.condition; } }

        public ICommand EndCommand { get { return this.endCommand; } }

        public ICommand Body { get { return this.body; } }

        public void Execute(IBindingEnvironment environment)
        {
            if (this.initialCommand != null)
                this.initialCommand.Execute(environment);

            while (this.condition == null || Predicates.IsTrue(this.condition.Evaluate(environment)))
            {
                if (this.body != null)
                    this.body.Execute(environment);
                if (this.endCommand != null)
                    this.endCommand.Execute(environment);
            }
        }
    }
}

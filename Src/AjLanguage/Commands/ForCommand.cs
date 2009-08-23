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

        public void Execute(BindingEnvironment environment)
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

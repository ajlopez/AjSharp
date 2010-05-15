namespace AjLanguage.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using AjLanguage.Expressions;

    [Serializable]
    public class ReturnCommand : ICommand
    {
        private IExpression expression;

        public ReturnCommand()
            : this(null)
        {
        }

        public ReturnCommand(IExpression expression)
        {
            this.expression = expression;
        }

        public IExpression Expression { get { return this.expression; } }

        public void Execute(IBindingEnvironment environment)
        {
            if (this.expression != null)
                Machine.CurrentFunctionStatus.ReturnValue = this.expression.Evaluate(environment);

            Machine.CurrentFunctionStatus.Returned = true;
        }
    }
}

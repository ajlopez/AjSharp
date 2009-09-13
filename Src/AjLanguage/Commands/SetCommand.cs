namespace AjLanguage.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using AjLanguage.Expressions;
    using AjLanguage.Language;

    public class SetCommand : ICommand
    {
        private IExpression leftValue;
        private IExpression expression;

        public SetCommand(IExpression leftValue, IExpression expression)
        {
            this.leftValue = leftValue;
            this.expression = expression;
        }

        public IExpression LeftValue { get { return this.leftValue; } }

        public IExpression Expression { get { return this.expression; } }

        public void Execute(IBindingEnvironment environment)
        {
            object value = this.expression.Evaluate(environment);

            ExpressionUtilities.SetValue(this.leftValue, value, environment);
        }
    }
}

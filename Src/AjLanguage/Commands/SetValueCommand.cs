namespace AjLanguage.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using AjLanguage.Expressions;
    using AjLanguage.Language;

    [Serializable]
    public class SetValueCommand : ICommand
    {
        private IExpression leftValue;
        private IExpression expression;

        public SetValueCommand(IExpression leftValue, IExpression expression)
        {
            this.leftValue = leftValue;
            this.expression = expression;
        }

        public IExpression LeftValue { get { return this.leftValue; } }

        public IExpression Expression { get { return this.expression; } }

        public void Execute(IBindingEnvironment environment)
        {
            object leftvalue = this.LeftValue.Evaluate(environment);
            object value = this.expression.Evaluate(environment);

            ((IReference)leftvalue).SetValue(value);
        }
    }
}

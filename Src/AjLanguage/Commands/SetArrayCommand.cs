namespace AjLanguage.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using AjLanguage.Expressions;
    using AjLanguage.Language;

    public class SetArrayCommand : ICommand
    {
        private IExpression leftValue;
        private ICollection<IExpression> arguments;
        private IExpression expression;

        public SetArrayCommand(IExpression leftValue, ICollection<IExpression> arguments, IExpression expression)
        {
            this.leftValue = leftValue;
            this.arguments = arguments;
            this.expression = expression;
        }

        public IExpression LeftValue { get { return this.leftValue; } }

        public IExpression Expression { get { return this.expression; } }

        public ICollection<IExpression> Arguments { get { return this.arguments; } }

        public void Execute(IBindingEnvironment environment)
        {
            object value = this.expression.Evaluate(environment);
            object[] indexes = null;
            List<object> values = new List<object>();

            foreach (IExpression expression in this.arguments)
                values.Add(expression.Evaluate(environment));

            indexes = values.ToArray();

            object obj = null;

            if (ObjectUtilities.IsNumber(indexes[0]))
                obj = ExpressionUtilities.ResolveToList(this.leftValue, environment);
            else
                obj = ExpressionUtilities.ResolveToDictionary(this.leftValue, environment);

            ObjectUtilities.SetIndexedValue(obj, indexes, value);
        }
    }
}

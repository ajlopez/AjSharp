namespace AjLanguage.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class ArrayExpression : IExpression
    {
        private IExpression expression;
        private ICollection<IExpression> arguments;

        public ArrayExpression(IExpression expression, ICollection<IExpression> arguments)
        {
            this.expression = expression;
            this.arguments = arguments;
        }

        public IExpression Expression { get { return this.expression; } }

        public ICollection<IExpression> Arguments { get { return this.arguments; } }

        public object Evaluate(IBindingEnvironment environment)
        {
            object obj = null;

            obj = this.Expression.Evaluate(environment);

            object[] parameters = null;

            if (this.arguments != null)
            {
                List<object> values = new List<object>();

                foreach (IExpression argument in this.arguments)
                    values.Add(argument.Evaluate(environment));

                parameters = values.ToArray();
            }

            // TODO if undefined, do nothing
            if (obj == null)
                return null;

            return ObjectUtilities.GetIndexedValue(obj, parameters);
        }

        private static Type AsType(IExpression expression)
        {
            string name = AsName(expression);

            if (name == null)
                return null;

            return TypeUtilities.AsType(name);
        }

        private static string AsName(IExpression expression)
        {
            if (expression is VariableExpression)
                return ((VariableExpression)expression).VariableName;

            if (expression is DotExpression)
            {
                DotExpression dot = (DotExpression)expression;

                return AsName(dot.Expression) + "." + dot.Name;
            }

            return null;
        }
    }
}

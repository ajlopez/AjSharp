namespace AjLanguage.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using AjLanguage.Language;

    public class NewArrayExpression : IExpression
    {
        private string typename;
        private ICollection<IExpression> arguments;

        public NewArrayExpression(string typename, ICollection<IExpression> arguments)
        {
            this.typename = typename;
            this.arguments = arguments;
        }

        public string TypeName { get { return this.typename; } }

        public ICollection<IExpression> Arguments { get { return this.arguments; } }

        public object Evaluate(IBindingEnvironment environment)
        {
            object value = environment.GetValue(this.typename);

            Type type = null;

            // TODO create a typed array for IClass instances, not IClassicObject array
            if (!(value is IClass))
                type = TypeUtilities.GetType(environment, this.typename);
            else
                type = typeof(IClassicObject);

            int[] parameters = null;

            if (this.arguments != null && this.arguments.Count > 0)
            {
                List<int> values = new List<int>();

                foreach (IExpression argument in this.arguments)
                    values.Add(Convert.ToInt32(argument.Evaluate(environment)));

                parameters = values.ToArray();
            }

            if (parameters.Length == 1)
                return System.Array.CreateInstance(type, parameters[0]);

            return System.Array.CreateInstance(type, parameters);
        }
    }
}

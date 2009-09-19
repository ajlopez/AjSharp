namespace AjLanguage.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using AjLanguage.Language;

    public class InitializeArrayExpression : IExpression
    {
        private string typename;
        private ICollection<IExpression> values;

        public InitializeArrayExpression(string typename, ICollection<IExpression> values)
        {
            this.typename = typename;
            this.values = values;
        }

        public string TypeName { get { return this.typename; } }

        public ICollection<IExpression> Values { get { return this.values; } }

        public object Evaluate(IBindingEnvironment environment)
        {
            object value = environment.GetValue(this.typename);

            Type type = null;

            // TODO create a typed array for IClass instances, not IClassicObject array
            if (!(value is IClass))
                type = TypeUtilities.GetType(environment, this.typename);
            else
                type = typeof(IClassicObject);

            List<object> elements = new List<object>();

            if (this.values != null && this.values.Count > 0)
            {
                foreach (IExpression argument in this.values)
                    elements.Add(argument.Evaluate(environment));
            }

            System.Array array = System.Array.CreateInstance(type, elements.Count);

            for (int k = 0; k < elements.Count; k++)
                array.SetValue(elements[k], k);

            return array;
        }
    }
}

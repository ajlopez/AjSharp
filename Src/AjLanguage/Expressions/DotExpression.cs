﻿namespace AjLanguage.Expressions
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    [Serializable]
    public class DotExpression : IExpression
    {
        private IExpression expression;
        private string name;
        private ICollection<IExpression> arguments;
        private Type type;

        public DotExpression(IExpression expression, string name)
            : this(expression, name, null)
        {
        }

        public DotExpression(IExpression expression, string name, ICollection<IExpression> arguments)
        {
            this.expression = expression;
            this.name = name;
            this.arguments = arguments;
            this.type = AsType(this.expression);
        }

        public string Name { get { return this.name; } }

        public IExpression Expression { get { return this.expression; } }

        public Type Type { get { return this.type; } }

        public ICollection<IExpression> Arguments { get { return this.arguments; } }

        public object Evaluate(IBindingEnvironment environment)
        {
            object obj = null;

            // TODO refactor compare to Add, case sensitive? IsListVerb(this.name)?
            if (this.type == null)
                if (this.name == "Add")
                    obj = ExpressionUtilities.ResolveToList(this.expression, environment);
                else
                    obj = this.expression.Evaluate(environment);

            object[] parameters = null;

            if (this.arguments != null)
            {
                List<object> values = new List<object>();

                foreach (IExpression argument in this.arguments)
                {
                    object value = argument.Evaluate(environment);

                    if (argument is VariableVariableExpression)
                    {
                        if (value != null)
                            foreach (object val in (IEnumerable)value)
                                values.Add(val);
                    }
                    else
                        values.Add(value);
                }

                parameters = values.ToArray();
            }

            if (this.type != null)
                return TypeUtilities.InvokeTypeMember(this.type, this.name, parameters);

            if (obj is Type)
                return TypeUtilities.InvokeTypeMember((Type)obj, this.name, parameters);

            // TODO if undefined, do nothing
            if (obj == null)
                return null;

            return ObjectUtilities.GetValue(obj, this.name, parameters);
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

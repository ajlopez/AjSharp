namespace AjLanguage.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using AjLanguage.Commands;
    using AjLanguage.Language;
    using System.Collections;

    [Serializable]
    public class InvokeExpressionExpression : IExpression
    {
        private IExpression expression;
        private ICollection<IExpression> arguments;

        public InvokeExpressionExpression(IExpression expression, ICollection<IExpression> arguments)
        {
            this.expression = expression;
            this.arguments = arguments;
        }

        public IExpression Expression { get { return this.expression; } }

        public ICollection<IExpression> Arguments { get { return this.arguments; } }

        public object Evaluate(IBindingEnvironment environment)
        {
            object obj = null;
            ICallable callable;

            if (this.expression is ArrayExpression)
                callable = (ICallable)((ArrayExpression)this.expression).Evaluate(environment, ref obj);
            else
                callable = (ICallable)this.expression.Evaluate(environment);

            List<object> parameters = new List<object>();

            foreach (IExpression expression in this.arguments)
            {
                object parameter = expression.Evaluate(environment);

                if (expression is VariableVariableExpression)
                {
                    if (parameter != null)
                        foreach (object ob in (IEnumerable) parameter)
                            parameters.Add(ob);
                }
                else
                    parameters.Add(parameter);
            }

            if (obj != null && obj is DynamicObject)
            {
                DynamicObject dobj = (DynamicObject)obj;
                return dobj.Invoke(callable, parameters.ToArray());
            }

            if (callable is ILocalCallable)
                return callable.Invoke(environment, parameters.ToArray());

            return callable.Invoke(parameters.ToArray());
        }
    }
}

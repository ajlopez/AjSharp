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

        public void Execute(BindingEnvironment environment)
        {
            object value = this.expression.Evaluate(environment);

            if (this.leftValue is VariableExpression)
                environment.SetValue(((VariableExpression)this.leftValue).VariableName, value);
            else if (this.leftValue is DotExpression)
            {
                DotExpression dot = (DotExpression)this.leftValue;

                if (dot.Arguments != null)
                    throw new InvalidOperationException("Invalid left value");

                object obj = ResolveToObject(dot.Expression, environment);

                ObjectUtilities.SetValue(obj, dot.Name, value);
            }
            else
                throw new InvalidOperationException("Invalid left value");
        }

        private static object ResolveToObject(IExpression expression, BindingEnvironment environment)
        {
            if (expression is VariableExpression)
            {
                string name = ((VariableExpression) expression).VariableName;

                object obj = environment.GetValue(name);

                if (obj == null)
                {
                    obj = new DynamicObject();
                    
                    // TODO Review if Local or not
                    environment.SetValue(name, obj);
                }

                return obj;
            }

            if (expression is DotExpression)
            {
                DotExpression dot = (DotExpression)expression;

                object obj = ResolveToObject(dot.Expression, environment);

                if (obj is DynamicObject)
                {
                    DynamicObject dynobj = (DynamicObject)obj;

                    obj = dynobj.GetValue(dot.Name);

                    if (obj == null) 
                    {
                        obj = new DynamicObject();
                        dynobj.SetValue(dot.Name, obj);
                    }

                    return obj;
                }

                return ObjectUtilities.GetValue(obj, dot.Name);
            }

            return expression.Evaluate(environment);
        }
    }
}

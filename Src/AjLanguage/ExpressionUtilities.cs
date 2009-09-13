namespace AjLanguage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using AjLanguage.Expressions;
    using AjLanguage.Language;

    public class ExpressionUtilities
    {
        public static void SetValue(IExpression expression, object value, IBindingEnvironment environment)
        {
            if (expression is VariableExpression)
            {
                SetValue((VariableExpression)expression, value, environment);
                return;
            }

            if (expression is DotExpression)
            {
                SetValue((DotExpression)expression, value, environment);
                return;
            }

            throw new InvalidOperationException("Invalid left value");
        }

        private static void SetValue(VariableExpression expression, object value, IBindingEnvironment environment)
        {
            environment.SetValue(expression.VariableName, value);
        }

        private static void SetValue(DotExpression expression, object value, IBindingEnvironment environment)
        {
            if (expression.Arguments != null)
                throw new InvalidOperationException("Invalid left value");

            object obj = ResolveToObject(expression.Expression, environment);

            ObjectUtilities.SetValue(obj, expression.Name, value);
        }

        private static object ResolveToObject(IExpression expression, IBindingEnvironment environment)
        {
            if (expression is VariableExpression)
                return ResolveToObject((VariableExpression)expression, environment);

            if (expression is DotExpression)
                return ResolveToObject((DotExpression)expression, environment);

            return expression.Evaluate(environment);
        }

        private static object ResolveToObject(VariableExpression expression, IBindingEnvironment environment)
        {
            string name = expression.VariableName;

            object obj = environment.GetValue(name);

            if (obj == null)
            {
                obj = new DynamicObject();

                // TODO Review if Local or not
                environment.SetValue(name, obj);
            }

            return obj;
        }

        private static object ResolveToObject(DotExpression expression, IBindingEnvironment environment)
        {
            object obj = ResolveToObject(expression.Expression, environment);

            if (obj is DynamicObject)
            {
                DynamicObject dynobj = (DynamicObject)obj;

                obj = dynobj.GetValue(expression.Name);

                if (obj == null)
                {
                    obj = new DynamicObject();
                    dynobj.SetValue(expression.Name, obj);
                }

                return obj;
            }

            return ObjectUtilities.GetValue(obj, expression.Name);
        }
    }
}

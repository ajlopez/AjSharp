namespace AjLanguage
{
    using System;
    using System.Collections;
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

        public static object ResolveToObject(IExpression expression, IBindingEnvironment environment)
        {
            if (expression is VariableExpression)
                return ResolveToObject((VariableExpression)expression, environment);

            if (expression is DotExpression)
                return ResolveToObject((DotExpression)expression, environment);

            return expression.Evaluate(environment);
        }

        public static object ResolveToList(IExpression expression, IBindingEnvironment environment)
        {
            if (expression is VariableExpression)
                return ResolveToList((VariableExpression)expression, environment);

            if (expression is DotExpression)
                return ResolveToList((DotExpression)expression, environment);

            return expression.Evaluate(environment);
        }

        public static IDictionary ResolveToDictionary(IExpression expression, IBindingEnvironment environment)
        {
            if (expression is VariableExpression)
                return ResolveToDictionary((VariableExpression)expression, environment);

            if (expression is DotExpression)
                return ResolveToDictionary((DotExpression)expression, environment);

            return (IDictionary)expression.Evaluate(environment);
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

        private static object ResolveToList(VariableExpression expression, IBindingEnvironment environment)
        {
            string name = expression.VariableName;

            object obj = environment.GetValue(name);

            if (obj == null)
            {
                obj = new ArrayList();

                // TODO Review if Local or not
                environment.SetValue(name, obj);
            }

            return obj;
        }

        private static IList ResolveToList(DotExpression expression, IBindingEnvironment environment)
        {
            object obj = ResolveToObject(expression.Expression, environment);

            if (obj is DynamicObject)
            {
                DynamicObject dynobj = (DynamicObject)obj;

                obj = dynobj.GetValue(expression.Name);

                if (obj == null)
                {
                    obj = new ArrayList();
                    dynobj.SetValue(expression.Name, obj);
                }

                return (IList) obj;
            }

            return (IList) ObjectUtilities.GetValue(obj, expression.Name);
        }

        private static IDictionary ResolveToDictionary(VariableExpression expression, IBindingEnvironment environment)
        {
            string name = expression.VariableName;

            object obj = environment.GetValue(name);

            if (obj == null)
            {
                obj = new Hashtable();

                // TODO Review if Local or not
                environment.SetValue(name, obj);
            }

            return (IDictionary)obj;
        }

        private static IDictionary ResolveToDictionary(DotExpression expression, IBindingEnvironment environment)
        {
            object obj = ResolveToObject(expression.Expression, environment);

            if (obj is DynamicObject)
            {
                DynamicObject dynobj = (DynamicObject)obj;

                obj = dynobj.GetValue(expression.Name);

                if (obj == null)
                {
                    obj = new Hashtable();
                    dynobj.SetValue(expression.Name, obj);
                }

                return (IDictionary)obj;
            }

            return (IDictionary)ObjectUtilities.GetValue(obj, expression.Name);
        }
    }
}

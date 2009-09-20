﻿namespace AjLanguage.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class AndExpression : IExpression
    {
        private IExpression leftExpression;
        private IExpression rigthExpression;

        public AndExpression(IExpression left, IExpression right)
        {
            this.leftExpression = left;
            this.rigthExpression = right;
        }

        public IExpression LeftExpression { get { return this.leftExpression; } }

        public IExpression RightExpression { get { return this.rigthExpression; } }

        public object Evaluate(IBindingEnvironment environment)
        {
            object leftValue = this.leftExpression.Evaluate(environment);

            if (Predicates.IsFalse(leftValue))
                return false;

            return Predicates.IsTrue(this.rigthExpression.Evaluate(environment));
        }
    }
}

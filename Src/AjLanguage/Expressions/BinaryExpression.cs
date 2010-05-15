﻿namespace AjLanguage.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    [Serializable]
    public abstract class BinaryExpression : IExpression
    {
        private IExpression leftExpression;
        private IExpression rigthExpression;

        public BinaryExpression(IExpression left, IExpression right)
        {
            this.leftExpression = left;
            this.rigthExpression = right;
        }

        public IExpression LeftExpression { get { return this.leftExpression; } }

        public IExpression RightExpression { get { return this.rigthExpression; } }

        public abstract object Apply(object leftValue, object rightValue);

        #region IExpression Members

        public object Evaluate(IBindingEnvironment environment)
        {
            object leftValue = this.leftExpression.Evaluate(environment);
            object rightValue = this.rigthExpression.Evaluate(environment);

            return this.Apply(leftValue, rightValue);
        }

        #endregion
    }
}

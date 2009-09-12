namespace AjLanguage.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using AjLanguage.Language;

    using Microsoft.VisualBasic.CompilerServices;

    public class ArithmeticBinaryExpression : BinaryExpression
    {
        private Func<object, object, object> function;
        private ArithmeticOperator operation;

        public ArithmeticBinaryExpression(ArithmeticOperator operation, IExpression left, IExpression right)
            : base(left, right)
        {
            this.operation = operation;

            switch (operation)
            {
                case ArithmeticOperator.Add:
                    this.function = AddOrConcatenateObjects;
                    break;
                case ArithmeticOperator.Subtract:
                    this.function = Operators.SubtractObject;
                    break;
                case ArithmeticOperator.Multiply:
                    this.function = Operators.MultiplyObject;
                    break;
                case ArithmeticOperator.Divide:
                    this.function = Operators.DivideObject;
                    break;
                case ArithmeticOperator.IntegerDivide:
                    this.function = Operators.IntDivideObject;
                    break;
                default:
                    throw new ArgumentException("Invalid operator");
            }
        }

        public ArithmeticOperator Operation { get { return this.operation; } }

        public override object Apply(object leftValue, object rightValue)
        {
            return this.function(leftValue, rightValue);
        }

        private static object AddOrConcatenateObjects(object left, object right)
        {
            if (ObjectUtilities.IsNumber(left) && ObjectUtilities.IsNumber(right))
                return Operators.AddObject(left, right);

            return Operators.ConcatenateObject(left, right);
        }
    }
}

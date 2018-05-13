using System;
using TSProjDemo.Bridge.Internal.Expressions;

namespace TSProjDemo.Bridge.Internal
{
    /// <summary>
    /// Represents a walker over the expression tree.
    /// </summary>
    internal class MathExpressionVisitor
    {
        /// <summary>
        /// Visits the expression and evaluates the result for it.
        /// </summary>
        public double Visit(MathExpression expr)
        {
            switch (expr.ExprKind)
            {
                case ExpressionKind.Number:
                    return VisitNumberExpression((NumberExpression) expr);

                case ExpressionKind.Addition:
                    return VisitAdditionExpression((AdditionExpression)expr);

                case ExpressionKind.Subtraction:
                    return VisitSubtractionExpression((SubtractionExpression) expr);

                case ExpressionKind.Division:
                    return VisitDivisionExpression((DivisionExpression)expr);

                case ExpressionKind.Multiplication:
                    return VisitMultiplicationExpression((MultiplicationExpression) expr);

                default:
                    throw new ArgumentOutOfRangeException($"Unexpected expression kind '{expr.ExprKind}'.");
            }
        }

        protected virtual double VisitNumberExpression(NumberExpression expr)
        {
            return expr.Value;
        }

        protected virtual double VisitAdditionExpression(AdditionExpression expr)
        {
            var arg1 = Visit(expr.Arg1);
            var arg2 = Visit(expr.Arg2);

            return arg1 + arg2;
        }

        protected virtual double VisitSubtractionExpression(SubtractionExpression expr)
        {
            var arg1 = Visit(expr.Arg1);
            var arg2 = Visit(expr.Arg2);

            return arg1 - arg2;
        }

        protected virtual double VisitMultiplicationExpression(MultiplicationExpression expr)
        {
            var arg1 = Visit(expr.Arg1);
            var arg2 = Visit(expr.Arg2);

            return arg1 * arg2;
        }

        protected virtual double VisitDivisionExpression(DivisionExpression expr)
        {
            var arg1 = Visit(expr.Arg1);
            var arg2 = Visit(expr.Arg2);

            return arg1 / arg2;
        }
    }
}
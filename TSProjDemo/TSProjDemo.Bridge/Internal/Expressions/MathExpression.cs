using System.Collections.Generic;

namespace TSProjDemo.Bridge.Internal.Expressions
{
    /// <summary>
    /// Abstract math expression.
    /// </summary>
    internal abstract class MathExpression
    {
        public ExpressionKind ExprKind { get; }

        internal IList<MathExpression> ChildExpressions { get; }

        /// <summary>
        /// Initializes a new instance of math expression.
        /// </summary>
        /// <param name="exprKind"></param>
        internal MathExpression(ExpressionKind exprKind)
            : this(exprKind, new MathExpression[0])
        {
        }

        /// <summary>
        /// Initializes a new instance of math expression.
        /// </summary>
        /// <param name="exprKind"></param>
        /// <param name="childExpressions">Child expressions.</param>
        internal MathExpression(ExpressionKind exprKind, IList<MathExpression> childExpressions)
        {
            ExprKind = exprKind;
            ChildExpressions = childExpressions;
        }
    }
}
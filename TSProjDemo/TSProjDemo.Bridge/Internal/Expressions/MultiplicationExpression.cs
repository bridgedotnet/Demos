using System.Collections.Generic;

namespace TSProjDemo.Bridge.Internal.Expressions
{
    /// <summary>
    /// Expression for multiplication operation: "arg1 * arg2"
    /// </summary>
    internal class MultiplicationExpression : MathExpression
    {
        /// <summary>
        /// Argument #1
        /// </summary>
        public MathExpression Arg1 => ChildExpressions[0];

        /// <summary>
        /// Argument #2
        /// </summary>
        public MathExpression Arg2 => ChildExpressions[1];

        internal MultiplicationExpression(IList<MathExpression> childExpressions)
            : base(ExpressionKind.Multiplication, childExpressions)
        {
        }
    }
}
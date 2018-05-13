using System.Collections.Generic;

namespace TSProjDemo.Bridge.Internal.Expressions
{
    /// <summary>
    /// Expression for division operation: "arg1 / arg2"
    /// </summary>
    internal class DivisionExpression : MathExpression
    {
        /// <summary>
        /// Argument #1
        /// </summary>
        public MathExpression Arg1 => ChildExpressions[0];

        /// <summary>
        /// Argument #2
        /// </summary>
        public MathExpression Arg2 => ChildExpressions[1];

        internal DivisionExpression(IList<MathExpression> childExpressions)
            : base(ExpressionKind.Division, childExpressions)
        {
        }
    }
}
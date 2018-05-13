using System.Collections.Generic;

namespace TSProjDemo.Bridge.Internal.Expressions
{
    /// <summary>
    /// Expression for addition operation: "arg1 + arg2"
    /// </summary>
    internal class AdditionExpression : MathExpression
    {
        /// <summary>
        /// Argument #1
        /// </summary>
        public MathExpression Arg1 => ChildExpressions[0];

        /// <summary>
        /// Argument #2
        /// </summary>
        public MathExpression Arg2 => ChildExpressions[1];

        internal AdditionExpression(IList<MathExpression> childExpressions) 
            : base(ExpressionKind.Addition, childExpressions)
        {
        }
    }
}
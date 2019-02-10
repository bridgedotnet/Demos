using System;
using Bridge;
using TSProjDemo.Bridge.Internal;
using TSProjDemo.Bridge.Internal.Parsing;

namespace TSProjDemo.Bridge
{ 
    /// <summary>
    /// Evaluator of math expressions.
    /// </summary>
    public class Evaluator
    {
        /// <summary>
        /// Evaluates the result value for the provided math expression.
        /// </summary>
        /// <param name="s">Input string (math expression in Infix notation).</param>
        /// <returns>Result of evaluation of the expression.</returns>
        public double Evaluate(string s)
        {
            if (string.IsNullOrEmpty(s)) throw new ArgumentException("Value cannot be null or empty.", nameof(s));

            var builder = new ExpressionTreeBuilder();
            var expr = builder.Build(s);

            var visitor = new MathExpressionVisitor();
            var result = visitor.Visit(expr);

            return result;
        }
    }
}
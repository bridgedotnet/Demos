namespace TSProjDemo.Bridge.Internal.Expressions
{
    /// <summary>
    /// Expression for integer number expression: [0-9]*
    /// </summary>
    internal class NumberExpression : MathExpression
    {
        public int Value { get; }

        internal NumberExpression(string token) 
            : base(ExpressionKind.Number)
        {
            Value = int.Parse(token);
        }
    }
}
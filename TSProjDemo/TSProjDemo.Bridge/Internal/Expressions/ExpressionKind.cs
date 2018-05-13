namespace TSProjDemo.Bridge.Internal.Expressions
{
    /// <summary>
    /// Math expression kind.
    /// </summary>
    internal enum ExpressionKind
    {
        /// <summary>
        /// Number expression: [0-9]*
        /// </summary>
        Number,

        /// <summary>
        /// Addition expression: "arg1 + arg2"
        /// </summary>
        Addition,

        /// <summary>
        /// Subtraction operation: "arg1 - arg2"
        /// </summary>
        Subtraction,

        /// <summary>
        /// Multiplication expression: "arg1 * arg2"
        /// </summary>
        Multiplication,

        /// <summary>
        /// Division expression: "arg1 /+ arg2"
        /// </summary>
        Division,
    }
}
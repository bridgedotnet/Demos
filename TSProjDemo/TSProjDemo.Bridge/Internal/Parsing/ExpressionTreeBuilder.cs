using System;
using System.Collections.Generic;
using System.Linq;
using TSProjDemo.Bridge.Internal.Expressions;

namespace TSProjDemo.Bridge.Internal.Parsing
{
    /// <summary>
    /// Builds math expression tree.
    /// </summary>
    internal class ExpressionTreeBuilder
    {
        /// <summary>
        /// Creates a tree for math expression represented in Infix notation.
        /// </summary>
        /// <param name="s">String representing math expression in Infix notation.</param>
        /// <returns>Built expression.</returns>
        public MathExpression Build(string s)
        {
            if (string.IsNullOrEmpty(s)) throw new ArgumentException("Value cannot be null or empty.", nameof(s));

            // Convert expression to use Reverse Polish (postfix) notation
            var notationConverter = new ExpressionNotationConverter();
            var rpnExprStr = notationConverter.ToReversePolishNotation(s);

            var stack = new Stack<MathExpression>();

            string token;
            var index = 0;
            while ((token = ReadNextToken(rpnExprStr, ref index)) != null)
            {
                MathExpression expr;

                var tokenKind = GetTokenKind(token);
                switch (tokenKind)
                {
                    case TokenKind.Number:
                        expr = new NumberExpression(token);
                        break;

                    case TokenKind.Addition:
                        expr = new AdditionExpression(GetChildExpressions(stack, 2, tokenKind));
                        break;

                    case TokenKind.Subtraction:
                        expr = new SubtractionExpression(GetChildExpressions(stack, 2, tokenKind));
                        break;

                    case TokenKind.Multiplication:
                        expr = new MultiplicationExpression(GetChildExpressions(stack, 2, tokenKind));
                        break;

                    case TokenKind.Division:
                        expr = new DivisionExpression(GetChildExpressions(stack, 2, tokenKind));
                        break;

                    default:
                        throw new ArgumentOutOfRangeException($"Unexpected token kind: '{tokenKind}'.");
                }

                stack.Push(expr);
            }

            if (stack.Count != 1)
            {
                throw new InvalidOperationException("Incorrect math expression.");
            }

            return stack.Pop();
        }

        /// <summary>
        /// Check existance and returns child expressions from the stack.
        /// </summary>
        private MathExpression[] GetChildExpressions(Stack<MathExpression> stack, int count, TokenKind tokenKind)
        {
            if (stack.Count < count)
            {
                throw new InvalidOperationException($"Error building expression for token: '{tokenKind}', not enough arguments in stack.");
            }

            var res = new List<MathExpression>();

            var toPop = count;
            while (--toPop >= 0)
            {
                res.Insert(0, stack.Pop());
            }

            return res.ToArray();
        }

        /// <summary>
        /// Reads the next token from the specified position in the input string.
        /// </summary>
        private string ReadNextToken(string s, ref int index)
        {
            while (index < s.Length && char.IsWhiteSpace(s[index]))
            {
                ++index;
            }

            if (index >= s.Length)
            {
                return null;
            }

            var digitsCount = s.Skip(index).TakeWhile(char.IsDigit).Count();
            if (digitsCount > 0)
            {
                var numberToken = s.Substring(index, digitsCount);
                index += digitsCount;
                return numberToken;
            }

            var token = s[index].ToString();
            ++index;

            return token;
        }

        /// <summary>
        /// Gets math expression token kind.
        /// </summary>
        private TokenKind GetTokenKind(string token)
        {
            switch (token[0])
            {
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    return TokenKind.Number;

                case '+':
                    return TokenKind.Addition;

                case '-':
                    return TokenKind.Subtraction;

                case '*':
                    return TokenKind.Multiplication;

                case '/':
                    return TokenKind.Division;

                default:
                    throw new InvalidOperationException($"Unexpected token '{token}' in the math expression.");

            }
        }
    }
}
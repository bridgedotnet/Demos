using System;
using System.Collections.Generic;
using System.Text;

namespace TSProjDemo.Bridge.Internal.Parsing
{
    /// <summary>
    /// Helps to convert between different math expression notations
    /// </summary>
    internal class ExpressionNotationConverter
    {
        /// <summary>
        /// Converts Infix notation -> Reverse Polish notation (postfix).
        /// </summary>
        /// <param name="s">String representing math expression in Infix notation.</param>
        /// <returns>String representing the provided math expression in RPN (postfix) notation.</returns>
        public string ToReversePolishNotation(string s)
        {
            var res = new StringBuilder();

            var stack = new Stack<char>();
            var prevCharIsDigit = false;

            foreach (var ch in s)
            {
                if (char.IsDigit(ch))
                {
                    prevCharIsDigit = true;

                    res.Append(ch);
                    continue;
                }

                if (prevCharIsDigit)
                {
                    prevCharIsDigit = false;
                    res.Append(' ');
                }

                switch (ch)
                {
                    case '(':
                        stack.Push('(');
                        break;

                    case ')':
                        while (stack.Count > 0 && stack.Peek() != '(')
                        {
                            res.Append(stack.Pop());
                        }

                        stack.Pop(); // Pop '('
                        break;

                    case '+':
                    case '-':
                    case '*':
                    case '/':
                        var priority = GetOpPriority(ch);
                        while (stack.Count > 0 && priority <= GetOpPriority(stack.Peek()))
                        {
                            res.Append(stack.Pop());
                        }

                        stack.Push(ch);
                        break;

                    case ' ':
                    case '\t':
                        continue;

                    default:
                        throw new InvalidOperationException($"Unexpected character '{ch}' in the math expression.");
                }
            }

            while (stack.Count > 0)
            {
                res.Append(stack.Pop());
            }

            return res.ToString();
        }

        /// <summary>
        /// Returns priority for an operation represented by character.
        /// </summary>
        private int GetOpPriority(char ch)
        {
            switch (ch)
            {
                case '+':
                case '-':
                    return 1;

                case '*':
                case '/':
                    return 2;

                default:
                    return 0;
            }
        }
    }
}
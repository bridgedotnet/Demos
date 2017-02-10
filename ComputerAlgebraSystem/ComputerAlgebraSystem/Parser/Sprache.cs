using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Bridge;


namespace ComputerAlgebraSystem
{
    internal static class ResultHelper
    {
        [IgnoreGeneric]
        public static IResult<U> IfSuccess<T, U>(this IResult<T> result, Func<IResult<T>, IResult<U>> next)
        {
            if (result == null) throw new ArgumentNullException("result");

            if (result.WasSuccessful)
                return next(result);

            return Result.Failure<U>(result.Remainder, result.Message, result.Expectations);
        }

        [IgnoreGeneric]
        public static IResult<T> IfFailure<T>(this IResult<T> result, Func<IResult<T>, IResult<T>> next)
        {
            if (result == null) throw new ArgumentNullException("result");

            return result.WasSuccessful
                ? result
                : next(result);
        }
    }

    public static class Result
    {

        [IgnoreGeneric]
        public static IResult<T> Success<T>(T value, IInput remainder)
        {
            return new Result<T>(value, remainder);
        }

        [IgnoreGeneric]
        public static IResult<T> Failure<T>(IInput remainder, string message, IEnumerable<string> expectations)
        {
            return new Result<T>(remainder, message, expectations);
        }
    }

    [IgnoreGeneric]
    internal class Result<T> : IResult<T>
    {
        private readonly dynamic _value;
        private readonly IInput _remainder;
        private readonly bool _wasSuccessful;
        private readonly string _message;
        private readonly IEnumerable<string> _expectations;


        public Result(T value, IInput remainder)
        {
            _value = value;
            _remainder = remainder;
            _wasSuccessful = true;
            _message = null;
            _expectations = Enumerable.Empty<string>();
        }

        public Result(IInput remainder, string message, IEnumerable<string> expectations)
        {
            _value = null;
            _remainder = remainder;
            _wasSuccessful = false;
            _message = message;
            _expectations = expectations;
        }

        public T Value
        {
            get
            {
                if (!WasSuccessful)
                    throw new InvalidOperationException("No value can be computed.");

                return _value;
            }
        }

        public bool WasSuccessful { get { return _wasSuccessful; } }

        public string Message { get { return _message; } }

        public IEnumerable<string> Expectations { get { return _expectations; } }

        public IInput Remainder { get { return _remainder; } }

        public override string ToString()
        {
            if (WasSuccessful)
                return string.Format("Successful parsing of {0}.", Value);

            var expMsg = "";

            if (Expectations.Any())
                expMsg = " expected " + Expectations.Aggregate((e1, e2) => e1 + " or " + e2);

            var recentlyConsumed = CalculateRecentlyConsumed();

            return string.Format("Parsing failure: {0};{1} ({2}); recently consumed: {3}", Message, expMsg, Remainder, recentlyConsumed);
        }

        private string CalculateRecentlyConsumed()
        {
            const int windowSize = 10;

            var totalConsumedChars = Remainder.Position;
            var windowStart = totalConsumedChars - windowSize;
            windowStart = windowStart < 0 ? 0 : windowStart;

            var numberOfRecentlyConsumedChars = totalConsumedChars - windowStart;

            return Remainder.Source.Substring(windowStart, numberOfRecentlyConsumedChars);
        }
    }

    [IgnoreGeneric]
    public class Position : IEquatable<Position>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Position" /> class.
        /// </summary>
        /// <param name="pos">The position.</param>
        /// <param name="line">The line number.</param>
        /// <param name="column">The column.</param>
        public Position(int pos, int line, int column)
        {
            Pos = pos;
            Line = line;
            Column = column;
        }

        /// <summary>
        /// Creates an new <see cref="Position"/> instance from a given <see cref="IInput"/> object.
        /// </summary>
        /// <param name="input">The current input.</param>
        /// <returns>A new <see cref="Position"/> instance.</returns>
        public static Position FromInput(IInput input)
        {
            return new Position(input.Position, input.Line, input.Column);
        }

        /// <summary>
        /// Gets the current positon.
        /// </summary>
        public int Pos
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the current line number.
        /// </summary>
        public int Line
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the current column.
        /// </summary>
        public int Column
        {
            get;
            private set;
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="Position" />.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="Position" />; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current object. </param>
        public override bool Equals(object obj)
        {
            return Equals(obj as Position);
        }

        /// <summary>
        /// Indicates whether the current <see cref="Position" /> is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(Position other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Pos == other.Pos
                && Line == other.Line
                && Column == other.Column;
        }

        /// <summary>
        /// Indicates whether the left <see cref="Position" /> is equal to the right <see cref="Position" />.
        /// </summary>
        /// <param name="left">The left <see cref="Position" />.</param>
        /// <param name="right">The right <see cref="Position" />.</param>
        /// <returns>true if both objects are equal.</returns>
        public static bool operator ==(Position left, Position right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Indicates whether the left <see cref="Position" /> is not equal to the right <see cref="Position" />.
        /// </summary>
        /// <param name="left">The left <see cref="Position" />.</param>
        /// <param name="right">The right <see cref="Position" />.</param>
        /// <returns>true if the objects are not equal.</returns>
        public static bool operator !=(Position left, Position right)
        {
            return !Equals(left, right);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="Position" />.
        /// </returns>
        public override int GetHashCode()
        {
            var h = 31;
            h = h * 13 + Pos;
            h = h * 13 + Line;
            h = h * 13 + Column;
            return h;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return string.Format("Line {0}, Column {1}", Line, Column);
        }
    }

    [IgnoreGeneric]
    public delegate IResult<T> Parser<out T>(IInput input);

    [IgnoreGeneric]
    public static class ParserExtensions
    {

        [IgnoreGeneric]
        public static IResult<T> TryParse<T>(this Parser<T> parser, string input)
        {
            if (parser == null) throw new ArgumentNullException("parser");
            if (input == null) throw new ArgumentNullException("input");

            return parser(new Input(input));
        }

        [IgnoreGeneric]
        public static T Parse<T>(this Parser<T> parser, string input)
        {
            if (parser == null) throw new ArgumentNullException("parser");
            if (input == null) throw new ArgumentNullException("input");

            var result = parser.TryParse(input);

            if (result.WasSuccessful)
                return result.Value;

            throw new ParseException(result.ToString());
        }
    }


    public class ParseException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParseException" /> class.
        /// </summary>
        public ParseException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParseException" /> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ParseException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParseException" /> class with a specified error message 
        /// and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, 
        /// or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public ParseException(string message, Exception innerException) : base(message, innerException) { }
    }

    public static partial class Parse
    {
        public static Parser<char> Char(Predicate<char> predicate, string description)
        {
            if (predicate == null) throw new ArgumentNullException("predicate");
            if (description == null) throw new ArgumentNullException("description");

            return i =>
            {
                if (!i.AtEnd)
                {
                    if (predicate(i.Current))
                        return Result.Success(i.Current, i.Advance());

                    return Result.Failure<char>(i,
                        string.Format("unexpected '{0}'", i.Current),
                        new[] { description });
                }

                return Result.Failure<char>(i,
                    "Unexpected end of input reached",
                    new[] { description });
            };
        }

        public static Parser<char> CharExcept(Predicate<char> predicate, string description)
        {
            return Char(c => !predicate(c), "any character except " + description);
        }

        public static Parser<char> Char(char c)
        {
            return Char(ch => c == ch, c.ToString());
        }


        public static Parser<char> Chars(params char[] c)
        {
            return Char(c.Contains, string.Join("|", c));
        }

        public static Parser<char> Chars(string c)
        {
            return Char(c.Contains, string.Join("|", c.ToCharArray()));
        }


        /// <summary>
        /// Parse a single character except c.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static Parser<char> CharExcept(char c)
        {
            return CharExcept(ch => c == ch, c.ToString());
        }

        /// <summary>
        /// Parses a single character except for those in the given parameters
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static Parser<char> CharExcept(IEnumerable<char> c)
        {
            var chars = c as char[] ?? c.ToArray();
            return CharExcept(chars.Contains, string.Join("|", chars));
        }

        /// <summary>
        /// Parses a single character except for those in c
        /// </summary>  
        /// <param name="c"></param>
        /// <returns></returns> 
        public static Parser<char> CharExcept(string c)
        {
            return CharExcept(c.Contains, string.Join("|", c.ToCharArray()));
        }

        /// <summary>
        /// Parse a single character in a case-insensitive fashion.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static Parser<char> IgnoreCase(char c)
        {
            return Char(ch => char.ToLower(c) == char.ToLower(ch), c.ToString());
        }

        /// <summary>
        /// Parse a string in a case-insensitive fashion.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Parser<IEnumerable<char>> IgnoreCase(string s)
        {
            if (s == null) throw new ArgumentNullException("s");

            return s
                .Select(IgnoreCase)
                .Aggregate(Return(Enumerable.Empty<char>()),
                    (Parser<IEnumerable<char>> a, Parser<char> p) => a.Concat(p.Once()))
                .Named(s);
        }

        /// <summary>
        /// Parse any character.
        /// </summary>
        public static readonly Parser<char> AnyChar = Char(c => true, "any character");

        /// <summary>
        /// Parse a whitespace.
        /// </summary>
        public static readonly Parser<char> WhiteSpace = Char(char.IsWhiteSpace, "whitespace");

        /// <summary>
        /// Parse a digit.
        /// </summary>
        public static readonly Parser<char> Digit = Char(char.IsDigit, "digit");

        /// <summary>
        /// Parse a letter.
        /// </summary>
        public static readonly Parser<char> Letter = Char(char.IsLetter, "letter");

        /// <summary>
        /// Parse a letter or digit.
        /// </summary>
        public static readonly Parser<char> LetterOrDigit = Char(char.IsLetterOrDigit, "letter or digit");

        /// <summary>
        /// Parse a lowercase letter.
        /// </summary>
        public static readonly Parser<char> Lower = Char(char.IsLower, "lowercase letter");

        /// <summary>
        /// Parse an uppercase letter.
        /// </summary>
        public static readonly Parser<char> Upper = Char(char.IsUpper, "uppercase letter");

        /// <summary>
        /// Parse a numeric character.
        /// </summary>
        public static readonly Parser<char> Numeric = Char(char.IsNumber, "numeric character");

        /// <summary>
        /// Parse a string of characters.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Parser<IEnumerable<char>> String(string s)
        {
            if (s == null) throw new ArgumentNullException("s");

            return s
                .Select(Char)
                .Aggregate(Return(new char[] { }),
                    (Parser<IEnumerable<char>> a, Parser<char> p) => a.Concat(p.Once()))
                .Named(s);
        }


        [IgnoreGeneric]
        public static Parser<object> Not<T>(this Parser<T> parser)
        {
            if (parser == null) throw new ArgumentNullException("parser");

            return i =>
            {
                var result = parser(i);

                if (result.WasSuccessful)
                {
                    var msg = string.Format("`{0}' was not expected", string.Join(", ", result.Expectations));
                    return Result.Failure<object>(i, msg, new string[0]);
                }
                return Result.Success<object>(null, i);
            };
        }

        [IgnoreGeneric]
        public static Parser<U> Then<T, U>(this Parser<T> first, Func<T, Parser<U>> second)
        {
            if (first == null) throw new ArgumentNullException("first");
            if (second == null) throw new ArgumentNullException("second");

            return i => first(i).IfSuccess(s => second(s.Value)(s.Remainder));
        }


        [IgnoreGeneric]
        public static Parser<IEnumerable<T>> Many<T>(this Parser<T> parser)
        {
            if (parser == null) throw new ArgumentNullException("parser");

            return i =>
            {
                var remainder = i;
                var result = new List<object>();
                var r = parser(i);

                while (r.WasSuccessful)
                {
                    if (remainder.Equals(r.Remainder))
                        break;

                    result.Add(r.Value);
                    remainder = r.Remainder;
                    r = parser(remainder);
                }

                return Result.Success((dynamic)result, remainder);
            };
        }


        [IgnoreGeneric]
        public static Parser<IEnumerable<T>> XMany<T>(this Parser<T> parser)
        {
            if (parser == null) throw new ArgumentNullException("parser");

            return parser.Many().Then(m => parser.Once().XOr(Return(m)));
        }


        [IgnoreGeneric]
        public static Parser<IEnumerable<T>> AtLeastOnce<T>(this Parser<T> parser)
        {
            if (parser == null) throw new ArgumentNullException("parser");

            return parser.Once().Then(t1 => parser.Many().Select(ts => t1.Concat(ts)));
        }


        [IgnoreGeneric]
        public static Parser<IEnumerable<T>> XAtLeastOnce<T>(this Parser<T> parser)
        {
            if (parser == null) throw new ArgumentNullException("parser");

            return parser.Once().Then(t1 => parser.XMany().Select(ts => t1.Concat(ts)));
        }


        [IgnoreGeneric]
        public static Parser<T> End<T>(this Parser<T> parser)
        {
            if (parser == null) throw new ArgumentNullException("parser");

            return i => parser(i).IfSuccess(s =>
                s.Remainder.AtEnd
                    ? s
                    : Result.Failure<T>(
                        s.Remainder,
                        string.Format("unexpected '{0}'", s.Remainder.Current),
                        new[] { "end of input" }));
        }

        [IgnoreGeneric]
        public static Parser<U> Select<T, U>(this Parser<T> parser, Func<T, U> convert)
        {
            if (parser == null) throw new ArgumentNullException("parser");
            if (convert == null) throw new ArgumentNullException("convert");

            return parser.Then(t => Return(convert(t)));
        }

        [IgnoreGeneric]
        public static Parser<T> Token<T>(this Parser<T> parser)
        {
            if (parser == null) throw new ArgumentNullException("parser");

            return from leading in WhiteSpace.Many()
                   from item in parser
                   from trailing in WhiteSpace.Many()
                   select item;
        }


        [IgnoreGeneric]
        public static Parser<T> Ref<T>(Func<Parser<T>> reference)
        {
            if (reference == null) throw new ArgumentNullException("reference");

            Parser<T> p = null;

            return i =>
            {
                if (p == null)
                    p = reference();

                if (i.Memos.ContainsKey(p))
                    throw new ParseException(i.Memos[p].ToString());

                i.Memos[p] = Result.Failure<T>(i,
                    "Left recursion in the grammar.",
                    new string[0]);
                var result = p(i);
                i.Memos[p] = result;
                return result;
            };
        }

        public static Parser<string> Text(this Parser<IEnumerable<char>> characters)
        {
            return characters.Select(chs => new string(chs.ToArray()));
        }

        [IgnoreGeneric]
        public static Parser<T> Or<T>(this Parser<T> first, Parser<T> second)
        {
            if (first == null) throw new ArgumentNullException("first");
            if (second == null) throw new ArgumentNullException("second");

            return i =>
            {
                var fr = first(i);
                if (!fr.WasSuccessful)
                {
                    return second(i).IfFailure(sf => DetermineBestError(fr, sf));
                }

                if (fr.Remainder.Equals(i))
                    return second(i).IfFailure(sf => fr);

                return fr;
            };
        }


        [IgnoreGeneric]
        public static Parser<T> Named<T>(this Parser<T> parser, string name)
        {
            if (parser == null) throw new ArgumentNullException("parser");
            if (name == null) throw new ArgumentNullException("name");

            return i => parser(i).IfFailure(f => f.Remainder.Equals(i) ?
                Result.Failure<T>(f.Remainder, f.Message, new[] { name }) :
                f);
        }


        [IgnoreGeneric]
        public static Parser<T> XOr<T>(this Parser<T> first, Parser<T> second)
        {
            if (first == null) throw new ArgumentNullException("first");
            if (second == null) throw new ArgumentNullException("second");

            return i =>
            {
                var fr = first(i);
                if (!fr.WasSuccessful)
                {
                    // The 'X' part
                    if (!fr.Remainder.Equals(i))
                        return fr;

                    return second(i).IfFailure(sf => DetermineBestError(fr, sf));
                }

                // This handles a zero-length successful application of first.
                if (fr.Remainder.Equals(i))
                    return second(i).IfFailure(sf => fr);

                return fr;
            };
        }

        [IgnoreGeneric]
        static IResult<T> DetermineBestError<T>(IResult<T> firstFailure, IResult<T> secondFailure)
        {
            if (secondFailure.Remainder.Position > firstFailure.Remainder.Position)
                return secondFailure;

            if (secondFailure.Remainder.Position == firstFailure.Remainder.Position)
                return Result.Failure<T>(
                    firstFailure.Remainder,
                    firstFailure.Message,
                    firstFailure.Expectations.Union(secondFailure.Expectations));

            return firstFailure;
        }

        [IgnoreGeneric]
        public static Parser<IEnumerable<T>> Once<T>(this Parser<T> parser)
        {
            if (parser == null) throw new ArgumentNullException("parser");

            return (dynamic)parser.Select(r => (dynamic)new[] { r });
        }

        [IgnoreGeneric]
        public static Parser<IEnumerable<T>> Concat<T>(this Parser<IEnumerable<T>> first, Parser<IEnumerable<T>> second)
        {
            if (first == null) throw new ArgumentNullException("first");
            if (second == null) throw new ArgumentNullException("second");

            return first.Then(f => second.Select(f.Concat));
        }


        [IgnoreGeneric]
        public static Parser<T> Return<T>(T value)
        {
            return i => Result.Success(value, i);
        }


        [IgnoreGeneric]
        public static Parser<U> Return<T, U>(this Parser<T> parser, U value)
        {
            if (parser == null) throw new ArgumentNullException("parser");
            return parser.Select(t => value);
        }

        [IgnoreGeneric]
        public static Parser<T> Except<T, U>(this Parser<T> parser, Parser<U> except)
        {
            if (parser == null) throw new ArgumentNullException("parser");
            if (except == null) throw new ArgumentNullException("except");

            // Could be more like: except.Then(s => s.Fail("..")).XOr(parser)
            return i =>
            {
                var r = except(i);
                if (r.WasSuccessful)
                    return Result.Failure<T>(i, "Excepted parser succeeded.", new[] { "other than the excepted input" });
                return parser(i);
            };
        }


        [IgnoreGeneric]
        public static Parser<IEnumerable<T>> Until<T, U>(this Parser<T> parser, Parser<U> until)
        {
            return parser.Except(until).Many().Then(r => until.Return(r));
        }


        [IgnoreGeneric]
        public static Parser<T> Where<T>(this Parser<T> parser, Func<T, bool> predicate)
        {
            if (parser == null) throw new ArgumentNullException("parser");
            if (predicate == null) throw new ArgumentNullException("predicate");

            return i => parser(i).IfSuccess(s =>
                predicate(s.Value) ? s : Result.Failure<T>(i,
                    string.Format("Unexpected {0}.", s.Value),
                    new string[0]));
        }


        [IgnoreGeneric]
        public static Parser<V> SelectMany<T, U, V>(
            this Parser<T> parser,
            Func<T, Parser<U>> selector,
            Func<T, U, V> projector)
        {
            if (parser == null) throw new ArgumentNullException("parser");
            if (selector == null) throw new ArgumentNullException("selector");
            if (projector == null) throw new ArgumentNullException("projector");

            return parser.Then(t => selector(t).Select(u => projector(t, u)));
        }

        [IgnoreGeneric]
        public static Parser<T> ChainOperator<T, TOp>(
            Parser<TOp> op,
            Parser<T> operand,
            Func<TOp, T, T, T> apply)
        {
            if (op == null) throw new ArgumentNullException("op");
            if (operand == null) throw new ArgumentNullException("operand");
            if (apply == null) throw new ArgumentNullException("apply");
            return operand.Then(first => ChainOperatorRest(first, op, operand, apply, Or));
        }

        [IgnoreGeneric]
        public static Parser<T> XChainOperator<T, TOp>(
            Parser<TOp> op,
            Parser<T> operand,
            Func<TOp, T, T, T> apply)
        {
            if (op == null) throw new ArgumentNullException("op");
            if (operand == null) throw new ArgumentNullException("operand");
            if (apply == null) throw new ArgumentNullException("apply");
            return operand.Then(first => ChainOperatorRest(first, op, operand, apply, XOr));
        }

        [IgnoreGeneric]
        static Parser<T> ChainOperatorRest<T, TOp>(
            T firstOperand,
            Parser<TOp> op,
            Parser<T> operand,
            Func<TOp, T, T, T> apply,
            Func<Parser<T>, Parser<T>, Parser<T>> or)
        {
            if (op == null) throw new ArgumentNullException("op");
            if (operand == null) throw new ArgumentNullException("operand");
            if (apply == null) throw new ArgumentNullException("apply");
            return or(op.Then(opvalue =>
                          operand.Then(operandValue =>
                              ChainOperatorRest(apply(opvalue, firstOperand, operandValue), op, operand, apply, or))),
                      Return(firstOperand));
        }

        [IgnoreGeneric]
        public static Parser<T> ChainRightOperator<T, TOp>(
            Parser<TOp> op,
            Parser<T> operand,
            Func<TOp, T, T, T> apply)
        {
            if (op == null) throw new ArgumentNullException("op");
            if (operand == null) throw new ArgumentNullException("operand");
            if (apply == null) throw new ArgumentNullException("apply");
            return operand.Then(first => ChainRightOperatorRest(first, op, operand, apply, Or));
        }


        [IgnoreGeneric]
        public static Parser<T> XChainRightOperator<T, TOp>(
            Parser<TOp> op,
            Parser<T> operand,
            Func<TOp, T, T, T> apply)
        {
            if (op == null) throw new ArgumentNullException("op");
            if (operand == null) throw new ArgumentNullException("operand");
            if (apply == null) throw new ArgumentNullException("apply");
            return operand.Then(first => ChainRightOperatorRest(first, op, operand, apply, XOr));
        }

        [IgnoreGeneric]
        static Parser<T> ChainRightOperatorRest<T, TOp>(
            T lastOperand,
            Parser<TOp> op,
            Parser<T> operand,
            Func<TOp, T, T, T> apply,
            Func<Parser<T>, Parser<T>, Parser<T>> or)
        {
            if (op == null) throw new ArgumentNullException("op");
            if (operand == null) throw new ArgumentNullException("operand");
            if (apply == null) throw new ArgumentNullException("apply");
            return or(op.Then(opvalue =>
                        operand.Then(operandValue =>
                            ChainRightOperatorRest(operandValue, op, operand, apply, or)).Then(r =>
                                Return(apply(opvalue, lastOperand, r)))),
                      Return(lastOperand));
        }

        /// <summary>
        /// Parse a number.
        /// </summary>
        public static readonly Parser<string> Number = Numeric.AtLeastOnce().Text();

        static Parser<string> DecimalWithoutLeadingDigits(CultureInfo ci = null)
        {
            return from nothing in Return("")
                       // dummy so that CultureInfo.CurrentCulture is evaluated later
                   from dot in String((ci ?? CultureInfo.CurrentCulture).NumberFormat.NumberDecimalSeparator).Text()
                   from fraction in Number
                   select dot + fraction;
        }

        static Parser<string> DecimalWithLeadingDigits(CultureInfo ci = null)
        {
            return Number.Then(n => DecimalWithoutLeadingDigits(ci).XOr(Return("")).Select(f => n + f));
        }

        /// <summary>
        /// Parse a decimal number using the current culture's separator character.
        /// </summary>
        public static readonly Parser<string> Decimal = DecimalWithLeadingDigits().XOr(DecimalWithoutLeadingDigits());

        /// <summary>
        /// Parse a decimal number with separator '.'.
        /// </summary>
        public static readonly Parser<string> DecimalInvariant = DecimalWithLeadingDigits(CultureInfo.InvariantCulture)
                                                                     .XOr(DecimalWithoutLeadingDigits(CultureInfo.InvariantCulture));
    }

    [IgnoreGeneric]
    public interface IInput : IEquatable<IInput>
    {
        /// <summary>
        /// Advances the input.
        /// </summary>
        /// <returns>A new <see cref="IInput" /> that is advanced.</returns>
        /// <exception cref="System.InvalidOperationException">The input is already at the end of the source.</exception>
        IInput Advance();

        /// <summary>
        /// Gets the whole source.
        /// </summary>
        string Source { get; }

        /// <summary>
        /// Gets the current <see cref="System.Char" />.
        /// </summary>
        char Current { get; }

        /// <summary>
        /// Gets a value indicating whether the end of the source is reached.
        /// </summary>
        bool AtEnd { get; }

        /// <summary>
        /// Gets the current positon.
        /// </summary>
        int Position { get; }

        /// <summary>
        /// Gets the current line number.
        /// </summary>
        int Line { get; }

        /// <summary>
        /// Gets the current column.
        /// </summary>
        int Column { get; }

        /// <summary>
        /// Memos used by this input
        /// </summary>
        IDictionary<object, object> Memos { get; }
    }

    [IgnoreGeneric]
    public interface IPositionAware<out T>
    {
        /// <summary>
        /// Set the start <see cref="Position"/> and the matched length.
        /// </summary>
        /// <param name="startPos">The start position</param>
        /// <param name="length">The matched length.</param>
        /// <returns>The matched result.</returns>
        T SetPos(Position startPos, int length);
    }

    [IgnoreGeneric]
    public interface IResult<out T>
    {

        T Value { get; }

        /// <summary>
        /// Gets a value indicating whether wether parsing was successful.
        /// </summary>
        bool WasSuccessful { get; }

        /// <summary>
        /// Gets the error message.
        /// </summary>
        string Message { get; }

        /// <summary>
        /// Gets the parser expectations in case of error.
        /// </summary>
        IEnumerable<string> Expectations { get; }

        /// <summary>
        /// Gets the remainder of the input.
        /// </summary>
        IInput Remainder { get; }
    }

    [IgnoreGeneric]
    public class Input : IInput
    {
        private readonly string _source;
        private readonly int _position;
        private readonly int _line;
        private readonly int _column;

        public IDictionary<object, object> Memos { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Input" /> class.
        /// </summary>
        /// <param name="source">The source.</param>
        public Input(string source)
            : this(source, 0)
        {
        }

        internal Input(string source, int position, int line = 1, int column = 1)
        {
            _source = source;
            _position = position;
            _line = line;
            _column = column;

            Memos = new Dictionary<object, object>();
        }

        /// <summary>
        /// Advances the input.
        /// </summary>
        /// <returns>A new <see cref="IInput" /> that is advanced.</returns>
        /// <exception cref="System.InvalidOperationException">The input is already at the end of the source.</exception>
        public IInput Advance()
        {
            if (AtEnd)
                throw new InvalidOperationException("The input is already at the end of the source.");

            return new Input(_source, _position + 1, Current == '\n' ? _line + 1 : _line, Current == '\n' ? 1 : _column + 1);
        }

        /// <summary>
        /// Gets the whole source.
        /// </summary>
        public string Source { get { return _source; } }

        /// <summary>
        /// Gets the current <see cref="System.Char" />.
        /// </summary>
        public char Current { get { return _source[_position]; } }

        /// <summary>
        /// Gets a value indicating whether the end of the source is reached.
        /// </summary>
        public bool AtEnd { get { return _position == _source.Length; } }

        /// <summary>
        /// Gets the current positon.
        /// </summary>
        public int Position { get { return _position; } }

        /// <summary>
        /// Gets the current line number.
        /// </summary>
        public int Line { get { return _line; } }

        /// <summary>
        /// Gets the current column.
        /// </summary>
        public int Column { get { return _column; } }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return string.Format("Line {0}, Column {1}", _line, _column);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="Input" />.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return ((_source != null ? _source.GetHashCode() : 0) * 397) ^ _position;
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="Input" />.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="Input" />; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current object. </param>
        public override bool Equals(object obj)
        {
            return Equals(obj as IInput);
        }

        /// <summary>
        /// Indicates whether the current <see cref="Input" /> is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(IInput other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(_source, other.Source) && _position == other.Position;
        }

        /// <summary>
        /// Indicates whether the left <see cref="Input" /> is equal to the right <see cref="Input" />.
        /// </summary>
        /// <param name="left">The left <see cref="Input" />.</param>
        /// <param name="right">The right <see cref="Input" />.</param>
        /// <returns>true if both objects are equal.</returns>
        public static bool operator ==(Input left, Input right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Indicates whether the left <see cref="Input" /> is not equal to the right <see cref="Input" />.
        /// </summary>
        /// <param name="left">The left <see cref="Input" />.</param>
        /// <param name="right">The right <see cref="Input" />.</param>
        /// <returns>true if the objects are not equal.</returns>
        public static bool operator !=(Input left, Input right)
        {
            return !Equals(left, right);
        }
    }

    [IgnoreGeneric]
    public interface IOption<out T>
    {
        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is defined.
        /// </summary>
        bool IsDefined { get; }

        /// <summary>
        /// Gets the matched result or a default value.
        /// </summary>
        /// <returns></returns>
        T GetOrDefault();

        /// <summary>
        /// Gets the matched result.
        /// </summary>
        T Get();
    }

    [IgnoreGeneric]
    public static class OptionExtensions
    {
        [IgnoreGeneric]
        public static T GetOrElse<T>(this IOption<T> option, T defaultValue)
        {
            if (option == null) throw new ArgumentNullException("option");
            return option.IsEmpty ? defaultValue : option.Get();
        }
    }

    [IgnoreGeneric]
    internal abstract class AbstractOption<T> : IOption<T>
    {
        public abstract bool IsEmpty { get; }

        public bool IsDefined
        {
            get { return !IsEmpty; }
        }

        [IgnoreGeneric]
        public T GetOrDefault()
        {
            return IsEmpty ? default(T) : Get();
        }

        public abstract T Get();
    }

    [IgnoreGeneric]
    internal sealed class Some<T> : AbstractOption<T>
    {
        private readonly dynamic _value;

        public Some(T value)
        {
            _value = value;
        }

        public override bool IsEmpty
        {
            get { return false; }
        }

        public override T Get()
        {
            return _value;
        }
    }

    [IgnoreGeneric]
    internal sealed class None<T> : AbstractOption<T>
    {
        public override bool IsEmpty
        {
            get { return true; }
        }

        public override T Get()
        {
            throw new InvalidOperationException("Cannot get value from None.");
        }
    }

    [IgnoreGeneric]
    partial class Parse
    {
        [IgnoreGeneric]
        public static Parser<IOption<T>> Optional<T>(this Parser<T> parser)
        {
            if (parser == null) throw new ArgumentNullException("parser");

            return i =>
            {
                var pr = parser(i);

                if (pr.WasSuccessful)
                    return Result.Success(new Some<T>(pr.Value), pr.Remainder);

                return Result.Success(new None<T>(), i);
            };
        }
    }

    [IgnoreGeneric]
    partial class Parse
    {
        [IgnoreGeneric]
        public static Parser<T> Positioned<T>(this Parser<T> parser) where T : IPositionAware<T>
        {
            return i =>
            {
                var r = parser(i);

                if (r.WasSuccessful)
                {
                    return Result.Success(r.Value.SetPos(Position.FromInput(i), r.Remainder.Position - i.Position), r.Remainder);
                }
                return r;
            };
        }

        public static Parser<string> LineEnd =
                (from r in Char('\r').Optional()
                 from n in Char('\n')
                 select r.IsDefined ? r.Get().ToString() + n : n.ToString())
                .Named("LineEnd");

        public static Parser<string> LineTerminator =
            Return("").End()
                .Or(LineEnd.End())
                .Or(LineEnd)
                .Named("LineTerminator");
    }

    [IgnoreGeneric]
    partial class Parse
    {
        public static Parser<string> Regex(string pattern, string description = null)
        {
            if (pattern == null) throw new ArgumentNullException("pattern");

            return Regex(new Regex(pattern), description);
        }

        public static Parser<string> Regex(Regex regex, string description = null)
        {
            if (regex == null) throw new ArgumentNullException("regex");

            return RegexMatch(regex, description).Then(match => Return(match.Value));
        }

        public static Parser<Match> RegexMatch(string pattern, string description = null)
        {
            if (pattern == null) throw new ArgumentNullException("pattern");

            return RegexMatch(new Regex(pattern), description);
        }

        public static Parser<Match> RegexMatch(Regex regex, string description = null)
        {
            if (regex == null) throw new ArgumentNullException("regex");

            regex = OptimizeRegex(regex);

            var expectations = description == null
                ? new string[0]
                : new[] { description };

            return i =>
            {
                if (!i.AtEnd)
                {
                    var remainder = i;
                    var input = i.Source.Substring(i.Position);
                    var match = regex.Match(input);

                    if (match.Success)
                    {
                        for (int j = 0; j < match.Length; j++)
                            remainder = remainder.Advance();

                        return Result.Success(match, remainder);
                    }

                    var found = match.Index == input.Length
                                    ? "end of source"
                                    : string.Format("`{0}'", input[match.Index]);
                    return Result.Failure<Match>(
                        remainder,
                        "string matching regex `" + regex.ToString() + "' expected but " + found + " found",
                        expectations);
                }

                return Result.Failure<Match>(i, "Unexpected end of input", expectations);
            };
        }

        private static Regex OptimizeRegex(Regex regex)
        {
            return new Regex(string.Format("^(?:{0})", regex), regex.Options);
        }
    }

    [IgnoreGeneric]
    partial class Parse
    {
        [IgnoreGeneric]
        public static Parser<IEnumerable<T>> DelimitedBy<T, U>(this Parser<T> parser, Parser<U> delimiter)
        {
            if (parser == null) throw new ArgumentNullException("parser");
            if (delimiter == null) throw new ArgumentNullException("delimiter");

            return from head in parser.Once()
                   from tail in
                       (from separator in delimiter
                        from item in parser
                        select item).Many()
                   select head.Concat(tail);
        }

        [IgnoreGeneric]
        public static Parser<IEnumerable<T>> XDelimitedBy<T, U>(this Parser<T> itemParser, Parser<U> delimiter)
        {
            if (itemParser == null) throw new ArgumentNullException("itemParser");
            if (delimiter == null) throw new ArgumentNullException("delimiter");

            return from head in itemParser.Once()
                   from tail in
                       (from separator in delimiter
                        from item in itemParser
                        select item).XMany()
                   select head.Concat(tail);
        }

        [IgnoreGeneric]
        public static Parser<IEnumerable<T>> Repeat<T>(this Parser<T> parser, int count)
        {
            return Repeat(parser, count, count);
        }


        [IgnoreGeneric]
        public static Parser<IEnumerable<T>> Repeat<T>(this Parser<T> parser, int minimumCount, int maximumCount)
        {
            if (parser == null) throw new ArgumentNullException("parser");

            return i =>
            {
                var remainder = i;
                var result = new List<T>();

                for (var n = 0; n < maximumCount; ++n)
                {
                    var r = parser(remainder);

                    if (!r.WasSuccessful && n < minimumCount)
                    {
                        var what = r.Remainder.AtEnd
                            ? "end of input"
                            : r.Remainder.Current.ToString();

                        var msg = string.Format("Unexpected '{0}'", what);
                        var exp = string.Format("'{0}' between {1} and {2} times, but found {3}", string.Join(", ", r.Expectations),
                            minimumCount,
                            maximumCount,
                            n);

                        return Result.Failure<IEnumerable<T>>(i, msg, new[] { exp });
                    }

                    if (remainder != r.Remainder)
                    {
                        result.Add(r.Value);
                    }

                    remainder = r.Remainder;
                }

                return Result.Success<IEnumerable<T>>(result, remainder);
            };
        }

        [IgnoreGeneric]
        public static Parser<T> Contained<T, U, V>(this Parser<T> parser, Parser<U> open, Parser<V> close)
        {
            if (parser == null) throw new ArgumentNullException("parser");
            if (open == null) throw new ArgumentNullException("open");
            if (close == null) throw new ArgumentNullException("close");

            return from o in open
                   from item in parser
                   from c in close
                   select item;
        }
    }
}
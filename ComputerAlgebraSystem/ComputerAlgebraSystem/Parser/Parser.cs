using System;
using System.Linq;
using System.Collections.Generic;

using ComputerAlgebraSystem;

namespace ComputerAlgebraSystem
{
    public static class Parser
    {
        public static Parser<double> Double()
        {
            return Parse.Many(Parse.Digit).Then(digits => 
                   Parse.Char('.').Then(dot => 
                   Parse.Many(Parse.Digit).Then(fractionPart =>
                   {
                       var firstPartString = new string(digits.ToArray());
                       var fractionPartString = new string(fractionPart.ToArray());
                       
                       var firstPart = double.Parse(firstPartString);
                       var fracPartDouble = double.Parse(fractionPartString);

                       return Parse.Return(firstPart + (fracPartDouble / Math.Pow(10.0, fractionPartString.Length)));

                   })));
        }

        static readonly Parser<IExpr> Constant =
                from value in Double().Or(Parse.Number.Then(input => Parse.Return(double.Parse(input))))
                select new Constant(value);

        static IExpr MakeIdentiefier(string value)
        {
            switch (value)
            {
                case "pi":
                    return Expr.Constant(Math.PI);
                case "e":
                    return Expr.Constant(Math.E);
                default:
                    return Expr.Symbol(value);
            }
        }

        static readonly Parser<IExpr> Identifier =
            from first in Parse.Letter.Once()
            from rest in Parse.LetterOrDigit.Many()
            let value = new string(first.Concat(rest).ToArray())
            select MakeIdentiefier(value);

        static readonly Parser<IExpr> ConstTimesId =
                from value in Constant
                from id in Identifier
                select Expr.Times(value, id);

        static readonly Parser<IExpr> IdPowConst =
                from id in Identifier
                from hat in Parse.Char('^')
                from pow in Constant
                select Expr.Pow(id, pow);

        static readonly Parser<IExpr> ConstPowConst =
                from factor in Constant
                from hat in Parse.Char('^')
                from pow in Constant
                select Expr.Pow(factor, pow);

        static readonly Parser<IExpr> ConstPowId =
                from factor in Constant
                from hat in Parse.Char('^')
                from id in Identifier
                select Expr.Pow(factor, id);

        static readonly Parser<IExpr> IdCoeffPow =
                from constant in Constant
                from id in Identifier
                from hat in Parse.Char('^')
                from pow in Constant
                select Expr.Times(constant, Expr.Pow(id, pow));

        static readonly Parser<IExpr> IdPowId =
                from id in Identifier
                from hat in Parse.Char('^')
                from powId in Identifier
                select Expr.Pow(id, powId);

        static readonly Parser<IExpr> Term = 
                IdCoeffPow // 5x^3
                 .Or(ConstTimesId) // 5x
                 .Or(ConstPowId) // 2^x
                 .Or(IdPowConst) // x^2
                 .Or(ConstPowConst) // 5^2
                 .Or(Identifier) // x
                 .Or(Constant); // 2

        static readonly Parser<IExpr> UnaryFunction = // funcName(arg)
               from funcName in Identifier
               from lparen in Parse.Char('(')
               from arg in Function.Or(Term)
               from rparen in Parse.Char(')')
               select new Function(funcName.SymbolValue(), arg);

        static readonly Parser<IExpr> BinaryFunction = // funcName(arg1, arg2)
                from funcName in Identifier
                from lparen in Parse.Char('(')
                from fstArg in Function.Or(Term)
                from comma in Parse.Char(',')
                from sndArg in Function.Or(Term)
                from rparen in Parse.Char(')')
                select new Function(funcName.SymbolValue(), fstArg, sndArg);

        static readonly Parser<IExpr> Function = BinaryFunction.Or(UnaryFunction);

        static readonly Parser<IExpr> Expression =
                Parse.Optional(Parse.Char('(')).Then(lparen =>
                    Function.Or(Term).Then(expr => 
                                     Parse.Optional(Parse.Char(')'))
                                           .Then(rparen => Parse.Return(expr))));

        public static IExpr ParseInput(string input)
        {
            var cleanInput = input.Replace(" ", "").ToLower();
            return Expression.Parse(cleanInput);
        }

        public static IResult<IExpr> TryParseInput(string input)
        {
            var cleanInput = input.Replace(" ", "").ToLower();
            return Expression.TryParse(cleanInput);
        }

    }
}
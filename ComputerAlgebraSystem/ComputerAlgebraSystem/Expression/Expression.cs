using System;
using System.Linq;
using System.Collections.Generic;

namespace ComputerAlgebraSystem
{
    public static class Expr
    {
        public static IExpr Evaluate(IExpr expr)
        {
            if (IsConstant(expr))
                return expr;
            if (IsSymbol(expr))
                return expr;

            var func = expr as Function;

            if (func.IsBinary)
            {
                if (IsConstant(func.FirstArgument) && IsConstant(func.SecondArgument))
                {
                    var fst = func.FirstArgument as Constant;
                    var snd = func.SecondArgument as Constant;

                    switch (func.Name)
                    {
                        case "plus":
                            return new Constant(fst.Value + snd.Value);
                        case "times":
                            return new Constant(fst.Value * snd.Value);
                        case "div":
                            return new Constant(fst.Value / snd.Value);
                        case "minus":
                            return Evaluate(new Function("plus", fst, new Constant(-snd.Value)));
                        case "pow":
                            return new Constant(Math.Pow(fst.Value, snd.Value));
                        default:
                            return func;
                    }
                }

                else if (IsFunc(func.FirstArgument) && IsFunc(func.SecondArgument))
                {
                    switch (func.Name)
                    {
                        case "plus":
                            return Evaluate(Plus(Evaluate(func.FirstArgument), Evaluate(func.SecondArgument)));
                        case "times":
                            return Evaluate(Times(Evaluate(func.FirstArgument), Evaluate(func.SecondArgument)));
                        case "div":
                            return Evaluate(Div(Evaluate(func.FirstArgument), Evaluate(func.SecondArgument)));
                        case "minus":
                            return Evaluate(Minus(Evaluate(func.FirstArgument),func.SecondArgument));
                        case "pow":
                            return Evaluate(Pow(Evaluate(func.FirstArgument), func.SecondArgument));
                        default:
                            return func;
                    }
                }
                else if (func.FirstArgument.IsConstant() && func.SecondArgument.IsFunc())
                {
                    var innerExpr = Evaluate(func.SecondArgument);
                    if (!innerExpr.IsFunc()) // had been simplified
                        return Evaluate(new Function(func.Name, func.FirstArgument, innerExpr));
                    else
                        return new Function(func.Name, func.FirstArgument, innerExpr);
                }
                else if (func.FirstArgument.IsFunc() && func.SecondArgument.IsConstant())
                {
                    var innerExpr = Evaluate(func.FirstArgument);
                    if (!innerExpr.IsFunc()) // had been simplified
                        return Evaluate(new Function(func.Name, innerExpr, func.SecondArgument));
                    else
                        return new Function(func.Name, func.FirstArgument, innerExpr);
                }
                else if (func.FirstArgument.IsFunc() && !func.SecondArgument.IsFunc())
                {
                    var innerExpr = Evaluate(func.FirstArgument);
                    if (!innerExpr.IsFunc()) // had been simplified
                        return Evaluate(new Function(func.Name, innerExpr, func.SecondArgument));

                    return new Function(func.Name, innerExpr, func.SecondArgument);
                }
                else if (func.FirstArgument.IsSymbol() && func.SecondArgument.IsFunc())
                {
                    var innerExpr = Evaluate(func.SecondArgument);
                    return new Function(func.Name, func.FirstArgument, innerExpr);
                }
                else if (func.FirstArgument.IsFunc() && func.SecondArgument.IsFunc())
                {
                    var fstArg = Evaluate(func.FirstArgument);
                    var sndArg = Evaluate(func.SecondArgument);

                    if (fstArg.IsFunc() && sndArg.IsFunc())
                        return new Function(func.Name, fstArg, sndArg);

                    return Evaluate(
                            new Function(func.Name,
                                fstArg,
                                sndArg));
                }
                else
                {
                    return func;
                }
            }
            else
            {
                if (func.FirstArgument.IsFunc())
                {
                    return Evaluate(new Function(func.Name, Evaluate(func.FirstArgument)));
                }
                else if (func.FirstArgument.IsSymbol())
                {
                    return func;
                }
                else /* if (func.FirstArgument.IsConstant) */
                {
                    var constant = func.FirstArgument as Constant;
                    switch (func.Name)
                    {
                        case "-":
                            return new Constant(-constant.Value);
                        case "sin":
                            return new Constant(Math.Sin(constant.Value));
                        case "cos":
                            return new Constant(Math.Cos(constant.Value));
                        case "tan":
                            return new Constant(Math.Tan(constant.Value));
                        case "sqrt":
                            return new Constant(Math.Sqrt(constant.Value));
                        case "log":
                            return new Constant(Math.Log(constant.Value));
                        default:
                            return func;
                    }
                }
            }
        }

        private static IExpr Minus(IExpr expr, IExpr secondArgument)
        {
            return Plus(expr, Times(Constant(-1), secondArgument));
        }

        private static IExpr Div(IExpr expr1, IExpr expr2)
        {
            return Times(expr1, Pow(expr2, Constant(-1.0)));
        }

        public static IExpr Subtitute(IExpr expr, string variable, IExpr subt)
        {
            if (expr.IsSymbol())
            {
                if (expr.SymbolValue() == variable)
                    return subt;
                else
                    return expr;
            }
            else if (expr.IsConstant())
            {
                return expr;
            }
            else
            {
                var func = expr as Function;
                if (!func.IsBinary)
                {
                    return new Function(func.Name, Subtitute(func.FirstArgument, variable, subt));
                }
                else
                {
                    return new Function(func.Name, Subtitute(func.FirstArgument, variable, subt), Subtitute(func.SecondArgument, variable, subt));
                }
            }

        }

        public static IExpr Differentiate(IExpr expr, string variable)
        {
            if (expr.IsConstant())
            {
                return Constant(0.0);
            }
            else if (expr.IsSymbol())
            {
                if (expr.SymbolValue() == variable)
                    return Constant(1.0);
                else
                    return Constant(0.0);
            }
            else
            {
                var func = expr as Function;
                if (func.IsBinary)
                {
                    switch (func.Name)
                    {
                        case "plus":
                        {
                            var left = Differentiate(func.FirstArgument, variable);
                            var right = Differentiate(func.SecondArgument, variable);
                            return Plus(left, right);
                        }
                        case "minus":
                        {
                            var left = Differentiate(func.FirstArgument, variable);
                            var right = Differentiate(func.SecondArgument, variable);
                            return Plus(left, Times(Constant(-1.0), right));
                        }
                        case "times":
                        {
                            var f = func.FirstArgument;
                            var fPrime = Differentiate(f, variable);
                            var g = func.SecondArgument;
                            var gPrime = Differentiate(g, variable);
                            return Plus(Times(f, gPrime), Times(g, fPrime));
                        }
                        case "pow":
                        {
                            var g = func.FirstArgument;
                            var h = func.SecondArgument;
                            if (h.IsConstant())
                            {
                                var pow = h.ConstantValue();
                                return Times(Times(Constant(pow), Pow(g, Constant(pow - 1.0))), Differentiate(g, variable));
                            }
                            else if (g.IsConstant() && h.IsSymbol())
                            {
                               var constant = g.ConstantValue();
                               return Times(func, Log(g));
                            }
                            else
                            {
                                    return func;
                            }
                        }
                        case "div":
                        {
                            var rewritten = Div(func.FirstArgument, func.SecondArgument);
                            return Differentiate(rewritten, variable);
                        }
                        default:
                            return func; 
                    }
                }
                else
                {
                    switch(func.Name)
                    {
                        case "sin":
                            return Times(Cos(func.FirstArgument), Differentiate(func.FirstArgument, variable));
                        case "cos":
                            return Times(Times(Constant(-1.0), Sin(func.FirstArgument)), Differentiate(func.FirstArgument, variable));
                        case "tan":
                            return Pow(Sec(func.FirstArgument), Constant(2.0));
                        case "log":
                            return Div(Differentiate(func.FirstArgument, variable), func.FirstArgument);
                        default:
                            return func;
                    }
                }
            }
        }

        public static IExpr Sec(IExpr expr)
        {
            return Pow(Cos(expr), Constant(-1.0));
        }

        public static IExpr Cos(IExpr expr)
        {
            return new Function("cos", expr);
        }

        public  static IExpr Log(IExpr g)
        {
            return new Function("log", g);
        }

        public static Func<double, double> Lambdify(IExpr expr, string variable)
        {
            //if (VariableCount(expr) != 1 || Variables(expr).Distinct().FirstOrDefault() != variable)
            //{
            //    Console.WriteLine("Variable: " + variable);
            //    Console.WriteLine("Variable count: " + VariableCount(expr));
            //    Console.WriteLine("Variables: " + string.Join(", ", Variables(expr).Distinct().ToArray()));
            //    return null;
            //}
                

            return x =>
            {
                var subtitution = Subtitute(expr, variable, new Constant(x));
                var evaluation = Evaluate(subtitution);
                if (evaluation is Constant)
                    return evaluation.ConstantValue();
                else
                {
                    Console.WriteLine("Lambdafication of the expr: " + expr.ToString() + " was not successful");
                    Console.WriteLine("After subtitution: " + subtitution.ToString());
                    Console.WriteLine("After eval: " + evaluation.ToString());
                    return double.NaN;
                }
                    
            };
        }

        public static IExpr Plus(IExpr left, IExpr right)
        {
            return new Function("plus", left, right);
        }
        public static IExpr Times(IExpr left, IExpr right)
        {
            return new Function("times", left, right);
        }
        public static IExpr Sin(IExpr arg)
        {
            return new Function("sin", arg);
        }

        public static IExpr Constant(double x)
        {
            return new Constant(x);
        }

        public static IExpr Pow(IExpr left, IExpr right)
        {
            return new Function("pow", left, right);
        }

        public static bool IsConstant (this IExpr expr)
        {
            return expr is Constant;
        }

        public static bool IsSymbol(this IExpr expr)
        {
            return expr is Symbol;
        }

        public static bool IsFunc(this IExpr expr)
        {
            return expr is Function;
        }

        public static IExpr Symbol(string value)
        {
            return new Symbol(value);
        }

        public static string SymbolValue(this IExpr expr)
        {
            return ((Symbol)expr).Value;
        }

        public static double ConstantValue(this IExpr expr)
        {
            return ((Constant)expr).Value;
        }
        public static int VariableCount(IExpr expr)
        {
            //if (expr.IsSymbol())
            //    return 1;
            //if (expr.IsConstant())
            //    return 0;

            //var func = expr as Function;

            //if (!func.IsBinary)
            //    return VariableCount(func.FirstArgument);
            //else
            //    return VariableCount(func.FirstArgument) + VariableCount(func.SecondArgument);

            return Variables(expr).Distinct().Count();
        }

        public static IEnumerable<string> Variables(IExpr expr)
        {
            if (expr.IsSymbol())
                yield return expr.SymbolValue();
            if (expr.IsFunc())
            {
                var func = expr as Function;

                if (!func.IsBinary)
                {
                    foreach(var variable in Variables(func.FirstArgument))
                        yield return variable;
                }
                else
                {
                    foreach (var variable in Variables(func.FirstArgument).Concat(Variables(func.SecondArgument)))
                        yield return variable;
                }
            }
        }
    }



}
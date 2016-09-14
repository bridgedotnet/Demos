(function (globals) {
    "use strict";

    Bridge.define('ComputerAlgebraSystem.IOption$1', {
        $interface: true
    });
    
    Bridge.define('ComputerAlgebraSystem.App', {
        statics: {
            config: {
                init: function () {
                    Bridge.ready(this.main);
                }
            },
            main: function () {
                var settings = { curves: new System.Collections.Generic.List$1(Object)(), viewport: { xMin: -2 * Math.PI, xMax: 2 * Math.PI, yMin: -5, yMax: 5 }, height: 500, width: 800, stepX: 0.01, drawXAxis: true, drawYAxis: true };
    
    
                var layout = new ComputerAlgebraSystem.Layout(settings);
    
                layout.appendTo(document.body);
            }
        },
        $entryPoint: true
    });
    
    Bridge.define('ComputerAlgebraSystem.IExpr', {
        $interface: true
    });
    
    Bridge.define('ComputerAlgebraSystem.Expr', {
        statics: {
            evaluate: function (expr) {
                if (ComputerAlgebraSystem.Expr.isConstant(expr)) {
                    return expr;
                }
                if (ComputerAlgebraSystem.Expr.isSymbol(expr)) {
                    return expr;
                }
    
                var func = Bridge.as(expr, ComputerAlgebraSystem.Function);
    
                if (func.getIsBinary()) {
                    if (ComputerAlgebraSystem.Expr.isConstant(func.getFirstArgument()) && ComputerAlgebraSystem.Expr.isConstant(func.getSecondArgument())) {
                        var fst = Bridge.as(func.getFirstArgument(), ComputerAlgebraSystem.Constant);
                        var snd = Bridge.as(func.getSecondArgument(), ComputerAlgebraSystem.Constant);
    
                        switch (func.getName()) {
                            case "plus": 
                                return new ComputerAlgebraSystem.Constant(fst.getValue() + snd.getValue());
                            case "times": 
                                return new ComputerAlgebraSystem.Constant(fst.getValue() * snd.getValue());
                            case "div": 
                                return new ComputerAlgebraSystem.Constant(fst.getValue() / snd.getValue());
                            case "minus": 
                                return ComputerAlgebraSystem.Expr.evaluate(new ComputerAlgebraSystem.Function("constructor$1", "plus", fst, new ComputerAlgebraSystem.Constant(-snd.getValue())));
                            case "pow": 
                                return new ComputerAlgebraSystem.Constant(Math.pow(fst.getValue(), snd.getValue()));
                            default: 
                                return func;
                        }
                    }
                    else  {
                        if (ComputerAlgebraSystem.Expr.isFunc(func.getFirstArgument()) && ComputerAlgebraSystem.Expr.isFunc(func.getSecondArgument())) {
                            switch (func.getName()) {
                                case "plus": 
                                    return ComputerAlgebraSystem.Expr.evaluate(ComputerAlgebraSystem.Expr.plus(ComputerAlgebraSystem.Expr.evaluate(func.getFirstArgument()), ComputerAlgebraSystem.Expr.evaluate(func.getSecondArgument())));
                                case "times": 
                                    return ComputerAlgebraSystem.Expr.evaluate(ComputerAlgebraSystem.Expr.times(ComputerAlgebraSystem.Expr.evaluate(func.getFirstArgument()), ComputerAlgebraSystem.Expr.evaluate(func.getSecondArgument())));
                                case "div": 
                                    return ComputerAlgebraSystem.Expr.evaluate(ComputerAlgebraSystem.Expr.div(ComputerAlgebraSystem.Expr.evaluate(func.getFirstArgument()), ComputerAlgebraSystem.Expr.evaluate(func.getSecondArgument())));
                                case "minus": 
                                    return ComputerAlgebraSystem.Expr.evaluate(ComputerAlgebraSystem.Expr.minus(ComputerAlgebraSystem.Expr.evaluate(func.getFirstArgument()), func.getSecondArgument()));
                                case "pow": 
                                    return ComputerAlgebraSystem.Expr.evaluate(ComputerAlgebraSystem.Expr.pow(ComputerAlgebraSystem.Expr.evaluate(func.getFirstArgument()), func.getSecondArgument()));
                                default: 
                                    return func;
                            }
                        }
                        else  {
                            if (ComputerAlgebraSystem.Expr.isConstant(func.getFirstArgument()) && ComputerAlgebraSystem.Expr.isFunc(func.getSecondArgument())) {
                                var innerExpr = ComputerAlgebraSystem.Expr.evaluate(func.getSecondArgument());
                                if (!ComputerAlgebraSystem.Expr.isFunc(innerExpr)) {
                                    return ComputerAlgebraSystem.Expr.evaluate(new ComputerAlgebraSystem.Function("constructor$1", func.getName(), func.getFirstArgument(), innerExpr));
                                }
                                else  {
                                    return new ComputerAlgebraSystem.Function("constructor$1", func.getName(), func.getFirstArgument(), innerExpr);
                                }
                            }
                            else  {
                                if (ComputerAlgebraSystem.Expr.isFunc(func.getFirstArgument()) && ComputerAlgebraSystem.Expr.isConstant(func.getSecondArgument())) {
                                    var innerExpr1 = ComputerAlgebraSystem.Expr.evaluate(func.getFirstArgument());
                                    if (!ComputerAlgebraSystem.Expr.isFunc(innerExpr1)) {
                                        return ComputerAlgebraSystem.Expr.evaluate(new ComputerAlgebraSystem.Function("constructor$1", func.getName(), innerExpr1, func.getSecondArgument()));
                                    }
                                    else  {
                                        return new ComputerAlgebraSystem.Function("constructor$1", func.getName(), func.getFirstArgument(), innerExpr1);
                                    }
                                }
                                else  {
                                    if (ComputerAlgebraSystem.Expr.isFunc(func.getFirstArgument()) && !ComputerAlgebraSystem.Expr.isFunc(func.getSecondArgument())) {
                                        var innerExpr2 = ComputerAlgebraSystem.Expr.evaluate(func.getFirstArgument());
                                        if (!ComputerAlgebraSystem.Expr.isFunc(innerExpr2)) {
                                            return ComputerAlgebraSystem.Expr.evaluate(new ComputerAlgebraSystem.Function("constructor$1", func.getName(), innerExpr2, func.getSecondArgument()));
                                        }
    
                                        return new ComputerAlgebraSystem.Function("constructor$1", func.getName(), innerExpr2, func.getSecondArgument());
                                    }
                                    else  {
                                        if (ComputerAlgebraSystem.Expr.isSymbol(func.getFirstArgument()) && ComputerAlgebraSystem.Expr.isFunc(func.getSecondArgument())) {
                                            var innerExpr3 = ComputerAlgebraSystem.Expr.evaluate(func.getSecondArgument());
                                            return new ComputerAlgebraSystem.Function("constructor$1", func.getName(), func.getFirstArgument(), innerExpr3);
                                        }
                                        else  {
                                            if (ComputerAlgebraSystem.Expr.isFunc(func.getFirstArgument()) && ComputerAlgebraSystem.Expr.isFunc(func.getSecondArgument())) {
                                                var fstArg = ComputerAlgebraSystem.Expr.evaluate(func.getFirstArgument());
                                                var sndArg = ComputerAlgebraSystem.Expr.evaluate(func.getSecondArgument());
    
                                                if (ComputerAlgebraSystem.Expr.isFunc(fstArg) && ComputerAlgebraSystem.Expr.isFunc(sndArg)) {
                                                    return new ComputerAlgebraSystem.Function("constructor$1", func.getName(), fstArg, sndArg);
                                                }
    
                                                return ComputerAlgebraSystem.Expr.evaluate(new ComputerAlgebraSystem.Function("constructor$1", func.getName(), fstArg, sndArg));
                                            }
                                            else  {
                                                return func;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else  {
                    if (ComputerAlgebraSystem.Expr.isFunc(func.getFirstArgument())) {
                        return ComputerAlgebraSystem.Expr.evaluate(new ComputerAlgebraSystem.Function("constructor", func.getName(), ComputerAlgebraSystem.Expr.evaluate(func.getFirstArgument())));
                    }
                    else  {
                        if (ComputerAlgebraSystem.Expr.isSymbol(func.getFirstArgument())) {
                            return func;
                        }
                        else  {
                            var constant = Bridge.as(func.getFirstArgument(), ComputerAlgebraSystem.Constant);
                            switch (func.getName()) {
                                case "-": 
                                    return new ComputerAlgebraSystem.Constant(-constant.getValue());
                                case "sin": 
                                    return new ComputerAlgebraSystem.Constant(Math.sin(constant.getValue()));
                                case "cos": 
                                    return new ComputerAlgebraSystem.Constant(Math.cos(constant.getValue()));
                                case "tan": 
                                    return new ComputerAlgebraSystem.Constant(Math.tan(constant.getValue()));
                                case "sqrt": 
                                    return new ComputerAlgebraSystem.Constant(Math.sqrt(constant.getValue()));
                                case "log": 
                                    return new ComputerAlgebraSystem.Constant(Math.log(constant.getValue()));
                                default: 
                                    return func;
                            }
                        }
                    }
                }
            },
            minus: function (expr, secondArgument) {
                return ComputerAlgebraSystem.Expr.plus(expr, ComputerAlgebraSystem.Expr.times(ComputerAlgebraSystem.Expr.constant(-1), secondArgument));
            },
            div: function (expr1, expr2) {
                return ComputerAlgebraSystem.Expr.times(expr1, ComputerAlgebraSystem.Expr.pow(expr2, ComputerAlgebraSystem.Expr.constant(-1.0)));
            },
            subtitute: function (expr, variable, subt) {
                if (ComputerAlgebraSystem.Expr.isSymbol(expr)) {
                    if (Bridge.referenceEquals(ComputerAlgebraSystem.Expr.symbolValue(expr), variable)) {
                        return subt;
                    }
                    else  {
                        return expr;
                    }
                }
                else  {
                    if (ComputerAlgebraSystem.Expr.isConstant(expr)) {
                        return expr;
                    }
                    else  {
                        var func = Bridge.as(expr, ComputerAlgebraSystem.Function);
                        if (!func.getIsBinary()) {
                            return new ComputerAlgebraSystem.Function("constructor", func.getName(), ComputerAlgebraSystem.Expr.subtitute(func.getFirstArgument(), variable, subt));
                        }
                        else  {
                            return new ComputerAlgebraSystem.Function("constructor$1", func.getName(), ComputerAlgebraSystem.Expr.subtitute(func.getFirstArgument(), variable, subt), ComputerAlgebraSystem.Expr.subtitute(func.getSecondArgument(), variable, subt));
                        }
                    }
                }
    
            },
            differentiate: function (expr, variable) {
                if (ComputerAlgebraSystem.Expr.isConstant(expr)) {
                    return ComputerAlgebraSystem.Expr.constant(0.0);
                }
                else  {
                    if (ComputerAlgebraSystem.Expr.isSymbol(expr)) {
                        if (Bridge.referenceEquals(ComputerAlgebraSystem.Expr.symbolValue(expr), variable)) {
                            return ComputerAlgebraSystem.Expr.constant(1.0);
                        }
                        else  {
                            return ComputerAlgebraSystem.Expr.constant(0.0);
                        }
                    }
                    else  {
                        var func = Bridge.as(expr, ComputerAlgebraSystem.Function);
                        if (func.getIsBinary()) {
                            switch (func.getName()) {
                                case "plus": 
                                    {
                                        var left = ComputerAlgebraSystem.Expr.differentiate(func.getFirstArgument(), variable);
                                        var right = ComputerAlgebraSystem.Expr.differentiate(func.getSecondArgument(), variable);
                                        return ComputerAlgebraSystem.Expr.plus(left, right);
                                    }
                                case "minus": 
                                    {
                                        var left1 = ComputerAlgebraSystem.Expr.differentiate(func.getFirstArgument(), variable);
                                        var right1 = ComputerAlgebraSystem.Expr.differentiate(func.getSecondArgument(), variable);
                                        return ComputerAlgebraSystem.Expr.plus(left1, ComputerAlgebraSystem.Expr.times(ComputerAlgebraSystem.Expr.constant(-1.0), right1));
                                    }
                                case "times": 
                                    {
                                        var f = func.getFirstArgument();
                                        var fPrime = ComputerAlgebraSystem.Expr.differentiate(f, variable);
                                        var g = func.getSecondArgument();
                                        var gPrime = ComputerAlgebraSystem.Expr.differentiate(g, variable);
                                        return ComputerAlgebraSystem.Expr.plus(ComputerAlgebraSystem.Expr.times(f, gPrime), ComputerAlgebraSystem.Expr.times(g, fPrime));
                                    }
                                case "pow": 
                                    {
                                        var g1 = func.getFirstArgument();
                                        var h = func.getSecondArgument();
                                        if (ComputerAlgebraSystem.Expr.isConstant(h)) {
                                            var pow = ComputerAlgebraSystem.Expr.constantValue(h);
                                            return ComputerAlgebraSystem.Expr.times(ComputerAlgebraSystem.Expr.times(ComputerAlgebraSystem.Expr.constant(pow), ComputerAlgebraSystem.Expr.pow(g1, ComputerAlgebraSystem.Expr.constant(pow - 1.0))), ComputerAlgebraSystem.Expr.differentiate(g1, variable));
                                        }
                                        else  {
                                            if (ComputerAlgebraSystem.Expr.isConstant(g1) && ComputerAlgebraSystem.Expr.isSymbol(h)) {
                                                var constant = ComputerAlgebraSystem.Expr.constantValue(g1);
                                                return ComputerAlgebraSystem.Expr.times(func, ComputerAlgebraSystem.Expr.log(g1));
                                            }
                                            else  {
                                                return func;
                                            }
                                        }
                                    }
                                case "div": 
                                    {
                                        var rewritten = ComputerAlgebraSystem.Expr.div(func.getFirstArgument(), func.getSecondArgument());
                                        return ComputerAlgebraSystem.Expr.differentiate(rewritten, variable);
                                    }
                                default: 
                                    return func;
                            }
                        }
                        else  {
                            switch (func.getName()) {
                                case "sin": 
                                    return ComputerAlgebraSystem.Expr.times(ComputerAlgebraSystem.Expr.cos(func.getFirstArgument()), ComputerAlgebraSystem.Expr.differentiate(func.getFirstArgument(), variable));
                                case "cos": 
                                    return ComputerAlgebraSystem.Expr.times(ComputerAlgebraSystem.Expr.times(ComputerAlgebraSystem.Expr.constant(-1.0), ComputerAlgebraSystem.Expr.sin(func.getFirstArgument())), ComputerAlgebraSystem.Expr.differentiate(func.getFirstArgument(), variable));
                                case "tan": 
                                    return ComputerAlgebraSystem.Expr.pow(ComputerAlgebraSystem.Expr.sec(func.getFirstArgument()), ComputerAlgebraSystem.Expr.constant(2.0));
                                case "log": 
                                    return ComputerAlgebraSystem.Expr.div(ComputerAlgebraSystem.Expr.differentiate(func.getFirstArgument(), variable), func.getFirstArgument());
                                default: 
                                    return func;
                            }
                        }
                    }
                }
            },
            sec: function (expr) {
                return ComputerAlgebraSystem.Expr.pow(ComputerAlgebraSystem.Expr.cos(expr), ComputerAlgebraSystem.Expr.constant(-1.0));
            },
            cos: function (expr) {
                return new ComputerAlgebraSystem.Function("constructor", "cos", expr);
            },
            log: function (g) {
                return new ComputerAlgebraSystem.Function("constructor", "log", g);
            },
            lambdify: function (expr, variable) {
                //if (VariableCount(expr) != 1 || Variables(expr).Distinct().FirstOrDefault() != variable)
                //{
                //    Console.WriteLine("Variable: " + variable);
                //    Console.WriteLine("Variable count: " + VariableCount(expr));
                //    Console.WriteLine("Variables: " + string.Join(", ", Variables(expr).Distinct().ToArray()));
                //    return null;
                //}
    
    
                return function (x) {
                    var subtitution = ComputerAlgebraSystem.Expr.subtitute(expr, variable, new ComputerAlgebraSystem.Constant(x));
                    var evaluation = ComputerAlgebraSystem.Expr.evaluate(subtitution);
                    if (Bridge.is(evaluation, ComputerAlgebraSystem.Constant)) {
                        return ComputerAlgebraSystem.Expr.constantValue(evaluation);
                    }
                    else  {
                        System.Console.log("Lambdafication of the expr: " + expr.toString() + " was not successful");
                        System.Console.log("After subtitution: " + subtitution.toString());
                        System.Console.log("After eval: " + evaluation.toString());
                        return Number.NaN;
                    }
    
                };
            },
            plus: function (left, right) {
                return new ComputerAlgebraSystem.Function("constructor$1", "plus", left, right);
            },
            times: function (left, right) {
                return new ComputerAlgebraSystem.Function("constructor$1", "times", left, right);
            },
            sin: function (arg) {
                return new ComputerAlgebraSystem.Function("constructor", "sin", arg);
            },
            constant: function (x) {
                return new ComputerAlgebraSystem.Constant(x);
            },
            pow: function (left, right) {
                return new ComputerAlgebraSystem.Function("constructor$1", "pow", left, right);
            },
            isConstant: function (expr) {
                return Bridge.is(expr, ComputerAlgebraSystem.Constant);
            },
            isSymbol: function (expr) {
                return Bridge.is(expr, ComputerAlgebraSystem.Symbol);
            },
            isFunc: function (expr) {
                return Bridge.is(expr, ComputerAlgebraSystem.Function);
            },
            symbol: function (value) {
                return new ComputerAlgebraSystem.Symbol(value);
            },
            symbolValue: function (expr) {
                return Bridge.cast(expr, ComputerAlgebraSystem.Symbol).getValue();
            },
            constantValue: function (expr) {
                return Bridge.cast(expr, ComputerAlgebraSystem.Constant).getValue();
            },
            variableCount: function (expr) {
                //if (expr.IsSymbol())
                //    return 1;
                //if (expr.IsConstant())
                //    return 0;
    
                //var func = expr as Function;
    
                //if (!func.IsBinary)
                //    return VariableCount(func.FirstArgument);
                //else
                //    return VariableCount(func.FirstArgument) + VariableCount(func.SecondArgument);
    
                return System.Linq.Enumerable.from(ComputerAlgebraSystem.Expr.variables(expr)).distinct().count();
            },
            variables: function (expr) {
                var $t, $t1;
                var $yield = [];
                if (ComputerAlgebraSystem.Expr.isSymbol(expr)) {
                    $yield.push(ComputerAlgebraSystem.Expr.symbolValue(expr));
                }
                if (ComputerAlgebraSystem.Expr.isFunc(expr)) {
                    var func = Bridge.as(expr, ComputerAlgebraSystem.Function);
    
                    if (!func.getIsBinary()) {
                        $t = Bridge.getEnumerator(ComputerAlgebraSystem.Expr.variables(func.getFirstArgument()));
                        while ($t.moveNext()) {
                            var variable = $t.getCurrent();
                            $yield.push(variable);
                        }
                    }
                    else  {
                        $t1 = Bridge.getEnumerator(System.Linq.Enumerable.from(ComputerAlgebraSystem.Expr.variables(func.getFirstArgument())).concat(ComputerAlgebraSystem.Expr.variables(func.getSecondArgument())));
                        while ($t1.moveNext()) {
                            var variable1 = $t1.getCurrent();
                            $yield.push(variable1);
                        }
                    }
                }
                return System.Array.toEnumerable($yield);
            }
        }
    });
    
    Bridge.define('ComputerAlgebraSystem.Extensions', {
        statics: {
            withinTableDataCell: function (el) {
                var td = document.createElement('td');
                td.setAttribute("valign", "top");
                td.appendChild(el);
                return td;
            },
            appendChildren: function (parent, elems) {
                var $t;
                if (elems === void 0) { elems = []; }
                $t = Bridge.getEnumerator(elems);
                while ($t.moveNext()) {
                    var element = $t.getCurrent();
                    parent.appendChild(element);
                }
            }
        }
    });
    
    Bridge.define('ComputerAlgebraSystem.IInput', {
        inherits: function () { return [System.IEquatable$1(ComputerAlgebraSystem.IInput)]; },
        $interface: true
    });
    
    Bridge.define('ComputerAlgebraSystem.IPositionAware$1', {
        $interface: true
    });
    
    Bridge.define('ComputerAlgebraSystem.IResult$1', {
        $interface: true
    });
    
    Bridge.define('ComputerAlgebraSystem.Layout', {
        container: null,
        constructor: function (settings) {
            var plotter = new ComputerAlgebraSystem.TinyPlotter(settings);
    
            var exprInput = Bridge.merge(document.createElement('input'), {
                type: "text"
            } );
            exprInput.value = "sin(3x)";
    
            var evalInput = Bridge.merge(document.createElement('input'), {
                type: "text"
            } );
            evalInput.value = "plus(5^2, sin(div(pi, 2)))";
    
            var variableInput = Bridge.merge(document.createElement('input'), {
                type: "text"
            } );
            variableInput.value = "x";
    
            var deltaXInput = Bridge.merge(document.createElement('input'), {
                type: "text"
            } );
            deltaXInput.value = "0.005";
    
            var xminInput = Bridge.merge(document.createElement('input'), {
                type: "text"
            } );
            xminInput.value = System.Double.format(settings.viewport.xMin, 'G');
    
            var xmaxInput = Bridge.merge(document.createElement('input'), {
                type: "text"
            } );
            xmaxInput.value = System.Double.format(settings.viewport.xMax, 'G');
    
            var yminInput = Bridge.merge(document.createElement('input'), {
                type: "text"
            } );
            yminInput.value = System.Double.format(settings.viewport.yMin, 'G');
    
            var ymaxInput = Bridge.merge(document.createElement('input'), {
                type: "text"
            } );
            ymaxInput.value = System.Double.format(settings.viewport.yMax, 'G');
    
            var resultDiv = document.createElement('div');
            resultDiv.style.fontSize = "18px";
            resultDiv.style.maxWidth = "300px";
    
            var btnPlot = Bridge.merge(document.createElement('button'), {
                innerHTML: "Plot with derivative",
                onclick: Bridge.fn.bind(this, function (ev) {
                    var IsNaN = $_.ComputerAlgebraSystem.Layout.f1;
    
                    var isNotValid = Bridge.referenceEquals(exprInput.value, "") || Bridge.referenceEquals(variableInput.value, "") || IsNaN(deltaXInput) || IsNaN(xminInput) || IsNaN(xmaxInput) || IsNaN(yminInput) || IsNaN(ymaxInput);
    
    
                    if (isNotValid) {
                        this.write("<h1 style='color:red'>Input is not valid!</h1>", resultDiv);
                        return;
                    }
    
                    var result = ComputerAlgebraSystem.Parser.tryParseInput(exprInput.value);
                    if (result.getWasSuccessful()) {
                        // set the settings
                        plotter.settings.stepX = parseFloat(deltaXInput.value);
                        plotter.settings.viewport.xMin = parseFloat(xminInput.value);
                        plotter.settings.viewport.xMax = parseFloat(xmaxInput.value);
                        plotter.settings.viewport.yMin = parseFloat(yminInput.value);
                        plotter.settings.viewport.yMax = parseFloat(ymaxInput.value);
    
                        resultDiv.innerHTML = "";
                        var f = result.getValue();
                        var df = ComputerAlgebraSystem.Expr.differentiate(f, variableInput.value);
    
                        var fLambda = ComputerAlgebraSystem.Expr.lambdify(f, variableInput.value);
                        var dfLambda = ComputerAlgebraSystem.Expr.lambdify(df, variableInput.value);
                        var curveColor = this.randomColor();
    
                        plotter.settings.curves.clear();
                        plotter.settings.curves.add({ map: fLambda, color: curveColor });
                        plotter.settings.curves.add({ map: dfLambda, color: this.grayscale(curveColor) });
                        plotter.draw();
    
                        var rgbCurveColor = this.rGB(curveColor);
                        var rgbGrayColor = this.rGB(this.grayscale(curveColor));
    
                        var msgParsed = "<strong style='color:" + rgbCurveColor + "'>" + f.toString() + "</strong>";
                        var derivative = "<strong style='color:" + rgbGrayColor + "'>" + df.toString() + "</strong>";
                        this.write("<hr /> Parsed: <br />" + msgParsed + "<br /> Derivative: <br /> " + derivative + "<hr />", resultDiv);
                    }
                    else  {
                        var error = Bridge.toArray(result.getExpectations()).join("<br />");
                        this.write("<h1 style='color:red'>" + error + "</h1>", resultDiv);
                    }
    
                })
            } );
    
    
            var btnEvaluate = Bridge.merge(document.createElement('button'), {
                innerHTML: "Evaluate",
                onclick: Bridge.fn.bind(this, function (ev) {
                    if (Bridge.referenceEquals(evalInput.value, "")) {
                        this.write("<h1 style='color:red'>Input is not valid!</h1>", resultDiv);
                        return;
                    }
    
                    var result = ComputerAlgebraSystem.Parser.tryParseInput(evalInput.value);
                    if (result.getWasSuccessful()) {
                        resultDiv.innerHTML = "";
                        var expression = result.getValue();
                        var $eval = ComputerAlgebraSystem.Expr.evaluate(expression);
    
                        this.write("<h4 style='color:green'>Parsed: " + expression.toString() + "<br />" + "Answer: " + $eval.toString() + "</h4>", resultDiv);
                    }
                    else  {
                        var error = Bridge.toArray(result.getExpectations()).join("<br />");
                        this.write("<h1 style='color:red'>" + error + "</h1>", resultDiv);
                    }
    
    
                })
            } );
    
            var slider = Bridge.merge(document.createElement('input'), {
                type: "range"
            } );
    
            btnEvaluate.style.width = "90%";
            btnEvaluate.style.margin = "5px";
            btnEvaluate.style.height = "40px";
            btnPlot.style.margin = "5px";
            btnPlot.style.height = "40px";
            btnPlot.style.width = "90%";
    
            var layout = this.table([this.row$1([this.table([this.row$1([this.label("Expression"), exprInput]), this.row$1([this.label("Variable"), variableInput]), this.row$1([this.label("XAxis step"), deltaXInput]), this.row$1([this.label("XMin"), xminInput]), this.row$1([this.label("XMax"), xmaxInput]), this.row$1([this.label("YMin"), yminInput]), this.row$1([this.label("YMax"), ymaxInput]), this.row(btnPlot, 2), this.row(document.createElement('hr'), 2), this.row$1([this.label("Expression"), evalInput]), this.row(btnEvaluate, 2), this.row(resultDiv, 2)]), this.table([this.row$1([plotter.canvas])])])]);
    
            this.container = layout;
        },
        appendTo: function (el) {
            el.appendChild(this.container);
        },
        table: function (rows) {
            if (rows === void 0) { rows = []; }
            var table = document.createElement('table');
            ComputerAlgebraSystem.Extensions.appendChildren(table, rows);
            return table;
        },
        label: function (value) {
            var lbl = document.createElement('label');
            lbl.innerHTML = value;
            lbl.style.margin = "5px";
            lbl.style.fontSize = "18px";
            return lbl;
        },
        row$1: function (elements) {
            var $t;
            if (elements === void 0) { elements = []; }
            var row = document.createElement('tr');
            $t = Bridge.getEnumerator(elements);
            while ($t.moveNext()) {
                var el = $t.getCurrent();
                row.appendChild(ComputerAlgebraSystem.Extensions.withinTableDataCell(el));
            }
            return row;
        },
        row: function (el, colspan) {
            var row = document.createElement('tr');
            var td = document.createElement('td');
            td.setAttribute("colspan", colspan.toString());
            td.appendChild(el);
            row.appendChild(td);
            return row;
        },
        write: function (expr, el) {
            el.innerHTML = expr;
        },
        randomColor: function () {
            var random = new System.Random("constructor");
            return { blue: ((random.next$2(0, 255)) & 255), green: ((random.next$2(0, 255)) & 255), red: ((random.next$2(0, 50)) & 255) };
        },
        grayscale: function (color) {
            var avg = Bridge.Int.clipu8((((((color.red + color.green) | 0) + color.blue) | 0)) / 3.0);
            return { blue: avg, green: avg, red: avg };
        },
        rGB: function (color) {
            return System.String.format("rgba({0}, {1}, {2}, 255)", color.red, color.green, color.blue);
        }
    });
    
    var $_ = {};
    
    Bridge.ns("ComputerAlgebraSystem.Layout", $_);
    
    Bridge.apply($_.ComputerAlgebraSystem.Layout, {
        f1: function (x) {
            return isNaN(parseFloat(x.value));
        }
    });
    
    Bridge.define('ComputerAlgebraSystem.OptionExtensions', {
        statics: {
            getOrElse: function (option, defaultValue) {
                if (option == null) {
                    throw new System.ArgumentNullException("option");
                }
                return option.getIsEmpty() ? defaultValue : option.get();
            }
        }
    });
    
    Bridge.define('ComputerAlgebraSystem.Parse', {
        statics: {
            anyChar: null,
            whiteSpace: null,
            digit: null,
            letter: null,
            letterOrDigit: null,
            lower: null,
            upper: null,
            numeric: null,
            number: null,
            decimal: null,
            decimalInvariant: null,
            lineEnd: null,
            lineTerminator: null,
            config: {
                init: function () {
                    this.anyChar = ComputerAlgebraSystem.Parse.char$1($_.ComputerAlgebraSystem.Parse.f1, "any character");
                    this.whiteSpace = ComputerAlgebraSystem.Parse.char$1(function (ch) { return System.Char.isWhiteSpace(String.fromCharCode(ch)); }, "whitespace");
                    this.digit = ComputerAlgebraSystem.Parse.char$1(function (ch) { return System.Char.isDigit(ch); }, "digit");
                    this.letter = ComputerAlgebraSystem.Parse.char$1(function (ch) { return System.Char.isLetter(ch); }, "letter");
                    this.letterOrDigit = ComputerAlgebraSystem.Parse.char$1(function (ch) { return (System.Char.isDigit(ch) || System.Char.isLetter(ch)); }, "letter or digit");
                    this.lower = ComputerAlgebraSystem.Parse.char$1(function (ch) { return Bridge.isLower(ch); }, "lowercase letter");
                    this.upper = ComputerAlgebraSystem.Parse.char$1(function (ch) { return Bridge.isUpper(ch); }, "uppercase letter");
                    this.numeric = ComputerAlgebraSystem.Parse.char$1(function (ch) { return System.Char.isNumber(ch); }, "numeric character");
                    this.number = ComputerAlgebraSystem.Parse.text(ComputerAlgebraSystem.Parse.atLeastOnce(ComputerAlgebraSystem.Parse.numeric));
                    this.decimal = ComputerAlgebraSystem.Parse.xOr(ComputerAlgebraSystem.Parse.decimalWithLeadingDigits(), ComputerAlgebraSystem.Parse.decimalWithoutLeadingDigits());
                    this.decimalInvariant = ComputerAlgebraSystem.Parse.xOr(ComputerAlgebraSystem.Parse.decimalWithLeadingDigits(System.Globalization.CultureInfo.invariantCulture), ComputerAlgebraSystem.Parse.decimalWithoutLeadingDigits(System.Globalization.CultureInfo.invariantCulture));
                    this.lineEnd = ComputerAlgebraSystem.Parse.named((ComputerAlgebraSystem.Parse.selectMany(ComputerAlgebraSystem.Parse.optional(ComputerAlgebraSystem.Parse.char(13)), $_.ComputerAlgebraSystem.Parse.f2, $_.ComputerAlgebraSystem.Parse.f3)), "LineEnd");
                    this.lineTerminator = ComputerAlgebraSystem.Parse.named(ComputerAlgebraSystem.Parse.or(ComputerAlgebraSystem.Parse.or(ComputerAlgebraSystem.Parse.end(ComputerAlgebraSystem.Parse.return("")), ComputerAlgebraSystem.Parse.end(ComputerAlgebraSystem.Parse.lineEnd)), ComputerAlgebraSystem.Parse.lineEnd), "LineTerminator");
                }
            },
            char$1: function (predicate, description) {
                if (Bridge.staticEquals(predicate, null)) {
                    throw new System.ArgumentNullException("predicate");
                }
                if (description == null) {
                    throw new System.ArgumentNullException("description");
                }
                return function (i) {
                    if (!i.getAtEnd()) {
                        if (predicate(i.getCurrent())) {
                            return ComputerAlgebraSystem.Result.success(i.getCurrent(), i.advance());
                        }
                        return ComputerAlgebraSystem.Result.failure(i, System.String.format("unexpected '{0}'", i.getCurrent()), [description]);
                    }
                    return ComputerAlgebraSystem.Result.failure(i, "Unexpected end of input reached", [description]);
                };
            },
            char: function (c) {
                return ComputerAlgebraSystem.Parse.char$1(function (ch) {
                    return c === ch;
                }, String.fromCharCode(c));
            },
            charExcept$2: function (predicate, description) {
                return ComputerAlgebraSystem.Parse.char$1(function (c) {
                    return !predicate(c);
                }, "any character except " + description);
            },
            charExcept: function (c) {
                return ComputerAlgebraSystem.Parse.charExcept$2(function (ch) {
                    return c === ch;
                }, String.fromCharCode(c));
            },
            charExcept$1: function (c) {
                var $t;
                var chars = ($t = Bridge.as(c, Array), $t != null ? $t : System.Linq.Enumerable.from(c).toArray());
                return ComputerAlgebraSystem.Parse.charExcept$2(function (TSource, value) { return System.Linq.Enumerable.from(chars).contains(value); }, Bridge.toArray(chars).join("|"));
            },
            charExcept$3: function (c) {
                return ComputerAlgebraSystem.Parse.charExcept$2(function (TSource, value) { return System.Linq.Enumerable.from(c).contains(value); }, Bridge.toArray(System.String.toCharArray(c, 0, c.length)).join("|"));
            },
            chars: function (c) {
                if (c === void 0) { c = []; }
                return ComputerAlgebraSystem.Parse.char$1(function (TSource, value) { return System.Linq.Enumerable.from(c).contains(value); }, Bridge.toArray(c).join("|"));
            },
            chars$1: function (c) {
                return ComputerAlgebraSystem.Parse.char$1(function (TSource, value) { return System.Linq.Enumerable.from(c).contains(value); }, Bridge.toArray(System.String.toCharArray(c, 0, c.length)).join("|"));
            },
            ignoreCase: function (c) {
                return ComputerAlgebraSystem.Parse.char$1(function (ch) {
                    return String.fromCharCode(c).toLowerCase().charCodeAt(0) === String.fromCharCode(ch).toLowerCase().charCodeAt(0);
                }, String.fromCharCode(c));
            },
            ignoreCase$1: function (s) {
                if (s == null) {
                    throw new System.ArgumentNullException("s");
                }
                return ComputerAlgebraSystem.Parse.named(System.Linq.Enumerable.from(s).select(ComputerAlgebraSystem.Parse.ignoreCase).aggregate(ComputerAlgebraSystem.Parse.return(System.Linq.Enumerable.empty()), $_.ComputerAlgebraSystem.Parse.f4), s);
            },
            string: function (s) {
                if (s == null) {
                    throw new System.ArgumentNullException("s");
                }
                return ComputerAlgebraSystem.Parse.named(System.Linq.Enumerable.from(s).select(ComputerAlgebraSystem.Parse.char).aggregate(ComputerAlgebraSystem.Parse.return([]), $_.ComputerAlgebraSystem.Parse.f4), s);
            },
            not: function (parser) {
                if (Bridge.staticEquals(parser, null)) {
                    throw new System.ArgumentNullException("parser");
                }
                return function (i) {
                    var result = parser(i);
                    if (result.getWasSuccessful()) {
                        var msg = System.String.format("`{0}' was not expected", Bridge.toArray(result.getExpectations()).join(", "));
                        return ComputerAlgebraSystem.Result.failure(i, msg, System.Array.init(0, null));
                    }
                    return ComputerAlgebraSystem.Result.success(null, i);
                };
            },
            then: function (first, second) {
                if (Bridge.staticEquals(first, null)) {
                    throw new System.ArgumentNullException("first");
                }
                if (Bridge.staticEquals(second, null)) {
                    throw new System.ArgumentNullException("second");
                }
                return function (i) {
                    return ComputerAlgebraSystem.ResultHelper.ifSuccess(first(i), function (s) {
                        return second(s.getValue())(s.getRemainder());
                    });
                };
            },
            many: function (parser) {
                if (Bridge.staticEquals(parser, null)) {
                    throw new System.ArgumentNullException("parser");
                }
                return function (i) {
                    var remainder = i;
                    var result = new System.Collections.Generic.List$1(Object)();
                    var r = parser(i);
                    while (r.getWasSuccessful()) {
                        if (Bridge.equalsT(remainder, r.getRemainder())) {
                            break;
                        }
                        result.add(r.getValue());
                        remainder = r.getRemainder();
                        r = parser(remainder);
                    }
                    return ComputerAlgebraSystem.Result.success(result, remainder);
                };
            },
            xMany: function (parser) {
                if (Bridge.staticEquals(parser, null)) {
                    throw new System.ArgumentNullException("parser");
                }
                return ComputerAlgebraSystem.Parse.then(ComputerAlgebraSystem.Parse.many(parser), function (m) {
                    return ComputerAlgebraSystem.Parse.xOr(ComputerAlgebraSystem.Parse.once(parser), ComputerAlgebraSystem.Parse.return(m));
                });
            },
            atLeastOnce: function (parser) {
                if (Bridge.staticEquals(parser, null)) {
                    throw new System.ArgumentNullException("parser");
                }
                return ComputerAlgebraSystem.Parse.then(ComputerAlgebraSystem.Parse.once(parser), function (t1) {
                    return ComputerAlgebraSystem.Parse.select(ComputerAlgebraSystem.Parse.many(parser), function (ts) {
                        return System.Linq.Enumerable.from(t1).concat(ts);
                    });
                });
            },
            xAtLeastOnce: function (parser) {
                if (Bridge.staticEquals(parser, null)) {
                    throw new System.ArgumentNullException("parser");
                }
                return ComputerAlgebraSystem.Parse.then(ComputerAlgebraSystem.Parse.once(parser), function (t1) {
                    return ComputerAlgebraSystem.Parse.select(ComputerAlgebraSystem.Parse.xMany(parser), function (ts) {
                        return System.Linq.Enumerable.from(t1).concat(ts);
                    });
                });
            },
            end: function (parser) {
                if (Bridge.staticEquals(parser, null)) {
                    throw new System.ArgumentNullException("parser");
                }
                return function (i) {
                    return ComputerAlgebraSystem.ResultHelper.ifSuccess(parser(i), function (s) {
                        return s.getRemainder().getAtEnd() ? s : ComputerAlgebraSystem.Result.failure(s.getRemainder(), System.String.format("unexpected '{0}'", s.getRemainder().getCurrent()), ["end of input"]);
                    });
                };
            },
            select: function (parser, convert) {
                if (Bridge.staticEquals(parser, null)) {
                    throw new System.ArgumentNullException("parser");
                }
                if (Bridge.staticEquals(convert, null)) {
                    throw new System.ArgumentNullException("convert");
                }
                return ComputerAlgebraSystem.Parse.then(parser, function (t) {
                    return ComputerAlgebraSystem.Parse.return(convert(t));
                });
            },
            token: function (parser) {
                if (Bridge.staticEquals(parser, null)) {
                    throw new System.ArgumentNullException("parser");
                }
                return ComputerAlgebraSystem.Parse.selectMany(ComputerAlgebraSystem.Parse.selectMany(ComputerAlgebraSystem.Parse.many(ComputerAlgebraSystem.Parse.whiteSpace), function (leading) {
                    return parser;
                }, $_.ComputerAlgebraSystem.Parse.f5), $_.ComputerAlgebraSystem.Parse.f6, $_.ComputerAlgebraSystem.Parse.f7);
            },
            ref: function (reference) {
                if (Bridge.staticEquals(reference, null)) {
                    throw new System.ArgumentNullException("reference");
                }
                var p = null;
                return function (i) {
                    if (Bridge.staticEquals(p, null)) {
                        p = reference();
                    }
                    if (i.getMemos().containsKey(p)) {
                        throw new ComputerAlgebraSystem.ParseException("constructor$1", i.getMemos().getItem(p).toString());
                    }
                    i.getMemos().setItem(p, ComputerAlgebraSystem.Result.failure(i, "Left recursion in the grammar.", System.Array.init(0, null)));
                    var result = p(i);
                    i.getMemos().setItem(p, result);
                    return result;
                };
            },
            text: function (characters) {
                return ComputerAlgebraSystem.Parse.select(characters, $_.ComputerAlgebraSystem.Parse.f8);
            },
            or: function (first, second) {
                if (Bridge.staticEquals(first, null)) {
                    throw new System.ArgumentNullException("first");
                }
                if (Bridge.staticEquals(second, null)) {
                    throw new System.ArgumentNullException("second");
                }
                return function (i) {
                    var fr = first(i);
                    if (!fr.getWasSuccessful()) {
                        return ComputerAlgebraSystem.ResultHelper.ifFailure(second(i), function (sf) {
                            return ComputerAlgebraSystem.Parse.determineBestError(fr, sf);
                        });
                    }
                    if (Bridge.equalsT(fr.getRemainder(), i)) {
                        return ComputerAlgebraSystem.ResultHelper.ifFailure(second(i), function (sf) {
                            return fr;
                        });
                    }
                    return fr;
                };
            },
            named: function (parser, name) {
                if (Bridge.staticEquals(parser, null)) {
                    throw new System.ArgumentNullException("parser");
                }
                if (name == null) {
                    throw new System.ArgumentNullException("name");
                }
                return function (i) {
                    return ComputerAlgebraSystem.ResultHelper.ifFailure(parser(i), function (f) {
                        return Bridge.equalsT(f.getRemainder(), i) ? ComputerAlgebraSystem.Result.failure(f.getRemainder(), f.getMessage(), [name]) : f;
                    });
                };
            },
            xOr: function (first, second) {
                if (Bridge.staticEquals(first, null)) {
                    throw new System.ArgumentNullException("first");
                }
                if (Bridge.staticEquals(second, null)) {
                    throw new System.ArgumentNullException("second");
                }
                return function (i) {
                    var fr = first(i);
                    if (!fr.getWasSuccessful()) {
                        if (!Bridge.equalsT(fr.getRemainder(), i)) {
                            return fr;
                        }
                        return ComputerAlgebraSystem.ResultHelper.ifFailure(second(i), function (sf) {
                            return ComputerAlgebraSystem.Parse.determineBestError(fr, sf);
                        });
                    }
                    if (Bridge.equalsT(fr.getRemainder(), i)) {
                        return ComputerAlgebraSystem.ResultHelper.ifFailure(second(i), function (sf) {
                            return fr;
                        });
                    }
                    return fr;
                };
            },
            determineBestError: function (firstFailure, secondFailure) {
                if (secondFailure.getRemainder().getPosition() > firstFailure.getRemainder().getPosition()) {
                    return secondFailure;
                }
                if (secondFailure.getRemainder().getPosition() === firstFailure.getRemainder().getPosition()) {
                    return ComputerAlgebraSystem.Result.failure(firstFailure.getRemainder(), firstFailure.getMessage(), System.Linq.Enumerable.from(firstFailure.getExpectations()).union(secondFailure.getExpectations()));
                }
                return firstFailure;
            },
            once: function (parser) {
                if (Bridge.staticEquals(parser, null)) {
                    throw new System.ArgumentNullException("parser");
                }
                return ComputerAlgebraSystem.Parse.select(parser, $_.ComputerAlgebraSystem.Parse.f9);
            },
            concat: function (first, second) {
                if (Bridge.staticEquals(first, null)) {
                    throw new System.ArgumentNullException("first");
                }
                if (Bridge.staticEquals(second, null)) {
                    throw new System.ArgumentNullException("second");
                }
                return ComputerAlgebraSystem.Parse.then(first, function (f) {
                    return ComputerAlgebraSystem.Parse.select(second, function (TSource, second) { return System.Linq.Enumerable.from(f).concat(second); });
                });
            },
            return: function (value) {
                return function (i) {
                    return ComputerAlgebraSystem.Result.success(value, i);
                };
            },
            return$1: function (parser, value) {
                if (Bridge.staticEquals(parser, null)) {
                    throw new System.ArgumentNullException("parser");
                }
                return ComputerAlgebraSystem.Parse.select(parser, function (t) {
                    return value;
                });
            },
            except: function (parser, except) {
                if (Bridge.staticEquals(parser, null)) {
                    throw new System.ArgumentNullException("parser");
                }
                if (Bridge.staticEquals(except, null)) {
                    throw new System.ArgumentNullException("except");
                }
                return function (i) {
                    var r = except(i);
                    if (r.getWasSuccessful()) {
                        return ComputerAlgebraSystem.Result.failure(i, "Excepted parser succeeded.", ["other than the excepted input"]);
                    }
                    return parser(i);
                };
            },
            until: function (parser, until) {
                return ComputerAlgebraSystem.Parse.then(ComputerAlgebraSystem.Parse.many(ComputerAlgebraSystem.Parse.except(parser, until)), function (r) {
                    return ComputerAlgebraSystem.Parse.return$1(until, r);
                });
            },
            where: function (parser, predicate) {
                if (Bridge.staticEquals(parser, null)) {
                    throw new System.ArgumentNullException("parser");
                }
                if (Bridge.staticEquals(predicate, null)) {
                    throw new System.ArgumentNullException("predicate");
                }
                return function (i) {
                    return ComputerAlgebraSystem.ResultHelper.ifSuccess(parser(i), function (s) {
                        return predicate(s.getValue()) ? s : ComputerAlgebraSystem.Result.failure(i, System.String.format("Unexpected {0}.", s.getValue()), System.Array.init(0, null));
                    });
                };
            },
            selectMany: function (parser, selector, projector) {
                if (Bridge.staticEquals(parser, null)) {
                    throw new System.ArgumentNullException("parser");
                }
                if (Bridge.staticEquals(selector, null)) {
                    throw new System.ArgumentNullException("selector");
                }
                if (Bridge.staticEquals(projector, null)) {
                    throw new System.ArgumentNullException("projector");
                }
                return ComputerAlgebraSystem.Parse.then(parser, function (t) {
                    return ComputerAlgebraSystem.Parse.select(selector(t), function (u) {
                        return projector(t, u);
                    });
                });
            },
            chainOperator: function (op, operand, apply) {
                if (Bridge.staticEquals(op, null)) {
                    throw new System.ArgumentNullException("op");
                }
                if (Bridge.staticEquals(operand, null)) {
                    throw new System.ArgumentNullException("operand");
                }
                if (Bridge.staticEquals(apply, null)) {
                    throw new System.ArgumentNullException("apply");
                }
                return ComputerAlgebraSystem.Parse.then(operand, function (first) {
                    return ComputerAlgebraSystem.Parse.chainOperatorRest(first, op, operand, apply, ComputerAlgebraSystem.Parse.or);
                });
            },
            xChainOperator: function (op, operand, apply) {
                if (Bridge.staticEquals(op, null)) {
                    throw new System.ArgumentNullException("op");
                }
                if (Bridge.staticEquals(operand, null)) {
                    throw new System.ArgumentNullException("operand");
                }
                if (Bridge.staticEquals(apply, null)) {
                    throw new System.ArgumentNullException("apply");
                }
                return ComputerAlgebraSystem.Parse.then(operand, function (first) {
                    return ComputerAlgebraSystem.Parse.chainOperatorRest(first, op, operand, apply, ComputerAlgebraSystem.Parse.xOr);
                });
            },
            chainOperatorRest: function (firstOperand, op, operand, apply, or) {
                if (Bridge.staticEquals(op, null)) {
                    throw new System.ArgumentNullException("op");
                }
                if (Bridge.staticEquals(operand, null)) {
                    throw new System.ArgumentNullException("operand");
                }
                if (Bridge.staticEquals(apply, null)) {
                    throw new System.ArgumentNullException("apply");
                }
                return or(ComputerAlgebraSystem.Parse.then(op, function (opvalue) {
                    return ComputerAlgebraSystem.Parse.then(operand, function (operandValue) {
                        return ComputerAlgebraSystem.Parse.chainOperatorRest(apply(opvalue, firstOperand, operandValue), op, operand, apply, or);
                    });
                }), ComputerAlgebraSystem.Parse.return(firstOperand));
            },
            chainRightOperator: function (op, operand, apply) {
                if (Bridge.staticEquals(op, null)) {
                    throw new System.ArgumentNullException("op");
                }
                if (Bridge.staticEquals(operand, null)) {
                    throw new System.ArgumentNullException("operand");
                }
                if (Bridge.staticEquals(apply, null)) {
                    throw new System.ArgumentNullException("apply");
                }
                return ComputerAlgebraSystem.Parse.then(operand, function (first) {
                    return ComputerAlgebraSystem.Parse.chainRightOperatorRest(first, op, operand, apply, ComputerAlgebraSystem.Parse.or);
                });
            },
            xChainRightOperator: function (op, operand, apply) {
                if (Bridge.staticEquals(op, null)) {
                    throw new System.ArgumentNullException("op");
                }
                if (Bridge.staticEquals(operand, null)) {
                    throw new System.ArgumentNullException("operand");
                }
                if (Bridge.staticEquals(apply, null)) {
                    throw new System.ArgumentNullException("apply");
                }
                return ComputerAlgebraSystem.Parse.then(operand, function (first) {
                    return ComputerAlgebraSystem.Parse.chainRightOperatorRest(first, op, operand, apply, ComputerAlgebraSystem.Parse.xOr);
                });
            },
            chainRightOperatorRest: function (lastOperand, op, operand, apply, or) {
                if (Bridge.staticEquals(op, null)) {
                    throw new System.ArgumentNullException("op");
                }
                if (Bridge.staticEquals(operand, null)) {
                    throw new System.ArgumentNullException("operand");
                }
                if (Bridge.staticEquals(apply, null)) {
                    throw new System.ArgumentNullException("apply");
                }
                return or(ComputerAlgebraSystem.Parse.then(op, function (opvalue) {
                    return ComputerAlgebraSystem.Parse.then(ComputerAlgebraSystem.Parse.then(operand, function (operandValue) {
                        return ComputerAlgebraSystem.Parse.chainRightOperatorRest(operandValue, op, operand, apply, or);
                    }), function (r) {
                        return ComputerAlgebraSystem.Parse.return(apply(opvalue, lastOperand, r));
                    });
                }), ComputerAlgebraSystem.Parse.return(lastOperand));
            },
            decimalWithoutLeadingDigits: function (ci) {
                if (ci === void 0) { ci = null; }
                return ComputerAlgebraSystem.Parse.selectMany(ComputerAlgebraSystem.Parse.selectMany(ComputerAlgebraSystem.Parse.return(""), function (nothing) {
                    var $t;
                    return ComputerAlgebraSystem.Parse.text(ComputerAlgebraSystem.Parse.string((($t = ci, $t != null ? $t : System.Globalization.CultureInfo.getCurrentCulture())).numberFormat.numberDecimalSeparator));
                }, $_.ComputerAlgebraSystem.Parse.f10), $_.ComputerAlgebraSystem.Parse.f11, $_.ComputerAlgebraSystem.Parse.f12);
            },
            decimalWithLeadingDigits: function (ci) {
                if (ci === void 0) { ci = null; }
                return ComputerAlgebraSystem.Parse.then(ComputerAlgebraSystem.Parse.number, function (n) {
                    return ComputerAlgebraSystem.Parse.select(ComputerAlgebraSystem.Parse.xOr(ComputerAlgebraSystem.Parse.decimalWithoutLeadingDigits(ci), ComputerAlgebraSystem.Parse.return("")), function (f) {
                        return n + f;
                    });
                });
            },
            optional: function (parser) {
                if (Bridge.staticEquals(parser, null)) {
                    throw new System.ArgumentNullException("parser");
                }
                return function (i) {
                    var pr = parser(i);
                    if (pr.getWasSuccessful()) {
                        return ComputerAlgebraSystem.Result.success(new ComputerAlgebraSystem.Some$1(pr.getValue()), pr.getRemainder());
                    }
                    return ComputerAlgebraSystem.Result.success(new ComputerAlgebraSystem.None$1(), i);
                };
            },
            positioned: function (parser) {
                return function (i) {
                    var r = parser(i);
                    if (r.getWasSuccessful()) {
                        return ComputerAlgebraSystem.Result.success(r.getValue().setPos(ComputerAlgebraSystem.Position.fromInput(i), ((r.getRemainder().getPosition() - i.getPosition()) | 0)), r.getRemainder());
                    }
                    return r;
                };
            },
            regex: function (pattern, description) {
                if (description === void 0) { description = null; }
                if (pattern == null) {
                    throw new System.ArgumentNullException("pattern");
                }
                return ComputerAlgebraSystem.Parse.regex$1(new System.Text.RegularExpressions.Regex("constructor", pattern), description);
            },
            regex$1: function (regex, description) {
                if (description === void 0) { description = null; }
                if (regex == null) {
                    throw new System.ArgumentNullException("regex");
                }
                return ComputerAlgebraSystem.Parse.then(ComputerAlgebraSystem.Parse.regexMatch$1(regex, description), $_.ComputerAlgebraSystem.Parse.f13);
            },
            regexMatch: function (pattern, description) {
                if (description === void 0) { description = null; }
                if (pattern == null) {
                    throw new System.ArgumentNullException("pattern");
                }
                return ComputerAlgebraSystem.Parse.regexMatch$1(new System.Text.RegularExpressions.Regex("constructor", pattern), description);
            },
            regexMatch$1: function (regex, description) {
                if (description === void 0) { description = null; }
                if (regex == null) {
                    throw new System.ArgumentNullException("regex");
                }
                regex = ComputerAlgebraSystem.Parse.optimizeRegex(regex);
                var expectations = description == null ? System.Array.init(0, null) : [description];
                return function (i) {
                    if (!i.getAtEnd()) {
                        var remainder = i;
                        var input = i.getSource().substr(i.getPosition());
                        var match = regex.match(input);
                        if (match.getSuccess()) {
                            for (var j = 0; j < match.getLength(); j = (j + 1) | 0) {
                                remainder = remainder.advance();
                            }
                            return ComputerAlgebraSystem.Result.success(match, remainder);
                        }
                        var found = match.getIndex() === input.length ? "end of source" : System.String.format("`{0}'", input.charCodeAt(match.getIndex()));
                        return ComputerAlgebraSystem.Result.failure(remainder, "string matching regex `" + regex.toString() + "' expected but " + found + " found", expectations);
                    }
                    return ComputerAlgebraSystem.Result.failure(i, "Unexpected end of input", expectations);
                };
            },
            optimizeRegex: function (regex) {
                return new System.Text.RegularExpressions.Regex("constructor$1", System.String.format("^(?:{0})", regex), regex.getOptions());
            },
            delimitedBy: function (parser, delimiter) {
                if (Bridge.staticEquals(parser, null)) {
                    throw new System.ArgumentNullException("parser");
                }
                if (Bridge.staticEquals(delimiter, null)) {
                    throw new System.ArgumentNullException("delimiter");
                }
    
                return ComputerAlgebraSystem.Parse.selectMany(ComputerAlgebraSystem.Parse.once(parser), function (head) {
                    return ComputerAlgebraSystem.Parse.many((ComputerAlgebraSystem.Parse.selectMany(delimiter, function (separator) {
                        return parser;
                    }, $_.ComputerAlgebraSystem.Parse.f14)));
                }, $_.ComputerAlgebraSystem.Parse.f15);
            },
            xDelimitedBy: function (itemParser, delimiter) {
                if (Bridge.staticEquals(itemParser, null)) {
                    throw new System.ArgumentNullException("itemParser");
                }
                if (Bridge.staticEquals(delimiter, null)) {
                    throw new System.ArgumentNullException("delimiter");
                }
    
                return ComputerAlgebraSystem.Parse.selectMany(ComputerAlgebraSystem.Parse.once(itemParser), function (head) {
                    return ComputerAlgebraSystem.Parse.xMany((ComputerAlgebraSystem.Parse.selectMany(delimiter, function (separator) {
                        return itemParser;
                    }, $_.ComputerAlgebraSystem.Parse.f14)));
                }, $_.ComputerAlgebraSystem.Parse.f15);
            },
            repeat: function (parser, count) {
                return ComputerAlgebraSystem.Parse.repeat$1(parser, count, count);
            },
            repeat$1: function (parser, minimumCount, maximumCount) {
                if (Bridge.staticEquals(parser, null)) {
                    throw new System.ArgumentNullException("parser");
                }
    
                return function (i) {
                    var remainder = i;
                    var result = new System.Collections.Generic.List$1(T)();
    
                    for (var n = 0; n < maximumCount; n = (n + 1) | 0) {
                        var r = parser(remainder);
    
                        if (!r.getWasSuccessful() && n < minimumCount) {
                            var what = r.getRemainder().getAtEnd() ? "end of input" : String.fromCharCode(r.getRemainder().getCurrent());
    
                            var msg = System.String.format("Unexpected '{0}'", what);
                            var exp = System.String.format("'{0}' between {1} and {2} times, but found {3}", Bridge.toArray(r.getExpectations()).join(", "), minimumCount, maximumCount, n);
    
                            return ComputerAlgebraSystem.Result.failure(i, msg, [exp]);
                        }
    
                        if (!Bridge.referenceEquals(remainder, r.getRemainder())) {
                            result.add(r.getValue());
                        }
    
                        remainder = r.getRemainder();
                    }
    
                    return ComputerAlgebraSystem.Result.success(result, remainder);
                };
            },
            contained: function (parser, open, close) {
                if (Bridge.staticEquals(parser, null)) {
                    throw new System.ArgumentNullException("parser");
                }
                if (Bridge.staticEquals(open, null)) {
                    throw new System.ArgumentNullException("open");
                }
                if (Bridge.staticEquals(close, null)) {
                    throw new System.ArgumentNullException("close");
                }
    
                return ComputerAlgebraSystem.Parse.selectMany(ComputerAlgebraSystem.Parse.selectMany(open, function (o) {
                    return parser;
                }, $_.ComputerAlgebraSystem.Parse.f16), function (x4) {
                    return close;
                }, $_.ComputerAlgebraSystem.Parse.f17);
            }
        }
    });
    
    Bridge.define("$AnonymousType$1", $_, {
        constructor: function (leading, item) {
            this.leading = leading;
            this.item = item;
        },
        getleading : function () {
            return this.leading;
        },
        getitem : function () {
            return this.item;
        },
        equals: function (o) {
            if (!Bridge.is(o, $_.$AnonymousType$1)) {
                return false;
            }
            return Bridge.equals(this.leading, o.leading) && Bridge.equals(this.item, o.item);
        },
        getHashCode: function () {
            var hash = 17;
            hash = hash * 23 + 1305346792;
            hash = hash * 23 + (this.leading == null ? 0 : Bridge.getHashCode(this.leading));
            hash = hash * 23 + (this.item == null ? 0 : Bridge.getHashCode(this.item));
            return hash;
        },
        toJSON: function () {
            return {
                leading : this.leading,
                item : this.item
            };
        }
    });
    
    Bridge.define("$AnonymousType$2", $_, {
        constructor: function (nothing, dot) {
            this.nothing = nothing;
            this.dot = dot;
        },
        getnothing : function () {
            return this.nothing;
        },
        getdot : function () {
            return this.dot;
        },
        equals: function (o) {
            if (!Bridge.is(o, $_.$AnonymousType$2)) {
                return false;
            }
            return Bridge.equals(this.nothing, o.nothing) && Bridge.equals(this.dot, o.dot);
        },
        getHashCode: function () {
            var hash = 17;
            hash = hash * 23 + -260737149;
            hash = hash * 23 + (this.nothing == null ? 0 : Bridge.getHashCode(this.nothing));
            hash = hash * 23 + (this.dot == null ? 0 : Bridge.getHashCode(this.dot));
            return hash;
        },
        toJSON: function () {
            return {
                nothing : this.nothing,
                dot : this.dot
            };
        }
    });
    
    Bridge.define("$AnonymousType$3", $_, {
        constructor: function (o, item) {
            this.o = o;
            this.item = item;
        },
        geto : function () {
            return this.o;
        },
        getitem : function () {
            return this.item;
        },
        equals: function (o) {
            if (!Bridge.is(o, $_.$AnonymousType$3)) {
                return false;
            }
            return Bridge.equals(this.o, o.o) && Bridge.equals(this.item, o.item);
        },
        getHashCode: function () {
            var hash = 17;
            hash = hash * 23 + -1826821090;
            hash = hash * 23 + (this.o == null ? 0 : Bridge.getHashCode(this.o));
            hash = hash * 23 + (this.item == null ? 0 : Bridge.getHashCode(this.item));
            return hash;
        },
        toJSON: function () {
            return {
                o : this.o,
                item : this.item
            };
        }
    });
    
    Bridge.ns("ComputerAlgebraSystem.Parse", $_);
    
    Bridge.apply($_.ComputerAlgebraSystem.Parse, {
        f1: function (c) {
            return true;
        },
        f2: function (r) {
            return ComputerAlgebraSystem.Parse.char(10);
        },
        f3: function (r, n) {
            return r.getIsDefined() ? String.fromCharCode(r.get()) + String.fromCharCode(n) : String.fromCharCode(n);
        },
        f4: function (a, p) {
            return ComputerAlgebraSystem.Parse.concat(a, ComputerAlgebraSystem.Parse.once(p));
        },
        f5: function (leading, item) {
            return new $_.$AnonymousType$1(leading, item);
        },
        f6: function (x0) {
            return ComputerAlgebraSystem.Parse.many(ComputerAlgebraSystem.Parse.whiteSpace);
        },
        f7: function (x1, trailing) {
            return x1.item;
        },
        f8: function (chs) {
            return String.fromCharCode.apply(null, System.Linq.Enumerable.from(chs).toArray());
        },
        f9: function (r) {
            return [r];
        },
        f10: function (nothing, dot) {
            return new $_.$AnonymousType$2(nothing, dot);
        },
        f11: function (x2) {
            return ComputerAlgebraSystem.Parse.number;
        },
        f12: function (x3, fraction) {
            return x3.dot + fraction;
        },
        f13: function (match) {
            return ComputerAlgebraSystem.Parse.return(match.getValue());
        },
        f14: function (separator, item) {
            return item;
        },
        f15: function (head, tail) {
            return System.Linq.Enumerable.from(head).concat(tail);
        },
        f16: function (o, item) {
            return new $_.$AnonymousType$3(o, item);
        },
        f17: function (x5, c) {
            return x5.item;
        }
    });
    
    Bridge.define('ComputerAlgebraSystem.ParseException', {
        inherits: [System.Exception],
        constructor: function () {
            System.Exception.prototype.$constructor.call(this);
    
        },
        constructor$1: function (message) {
            System.Exception.prototype.$constructor.call(this, message);
    
        },
        constructor$2: function (message, innerException) {
            System.Exception.prototype.$constructor.call(this, message, innerException);
    
        }
    });
    
    Bridge.define('ComputerAlgebraSystem.Parser', {
        statics: {
            constant: null,
            identifier: null,
            constTimesId: null,
            idPowConst: null,
            constPowConst: null,
            constPowId: null,
            idCoeffPow: null,
            idPowId: null,
            term: null,
            unaryFunction: null,
            binaryFunction: null,
            function: null,
            expression: null,
            config: {
                init: function () {
                    this.constant = ComputerAlgebraSystem.Parse.select(ComputerAlgebraSystem.Parse.or(ComputerAlgebraSystem.Parser.double(), ComputerAlgebraSystem.Parse.then(ComputerAlgebraSystem.Parse.number, $_.ComputerAlgebraSystem.Parser.f1)), $_.ComputerAlgebraSystem.Parser.f2);
                    this.identifier = ComputerAlgebraSystem.Parse.select(ComputerAlgebraSystem.Parse.select(ComputerAlgebraSystem.Parse.selectMany(ComputerAlgebraSystem.Parse.once(ComputerAlgebraSystem.Parse.letter), $_.ComputerAlgebraSystem.Parser.f3, $_.ComputerAlgebraSystem.Parser.f4), $_.ComputerAlgebraSystem.Parser.f5), $_.ComputerAlgebraSystem.Parser.f6);
                    this.constTimesId = ComputerAlgebraSystem.Parse.selectMany(ComputerAlgebraSystem.Parser.constant, $_.ComputerAlgebraSystem.Parser.f7, $_.ComputerAlgebraSystem.Parser.f8);
                    this.idPowConst = ComputerAlgebraSystem.Parse.selectMany(ComputerAlgebraSystem.Parse.selectMany(ComputerAlgebraSystem.Parser.identifier, $_.ComputerAlgebraSystem.Parser.f9, $_.ComputerAlgebraSystem.Parser.f10), $_.ComputerAlgebraSystem.Parser.f11, $_.ComputerAlgebraSystem.Parser.f12);
                    this.constPowConst = ComputerAlgebraSystem.Parse.selectMany(ComputerAlgebraSystem.Parse.selectMany(ComputerAlgebraSystem.Parser.constant, $_.ComputerAlgebraSystem.Parser.f13, $_.ComputerAlgebraSystem.Parser.f14), $_.ComputerAlgebraSystem.Parser.f15, $_.ComputerAlgebraSystem.Parser.f16);
                    this.constPowId = ComputerAlgebraSystem.Parse.selectMany(ComputerAlgebraSystem.Parse.selectMany(ComputerAlgebraSystem.Parser.constant, $_.ComputerAlgebraSystem.Parser.f13, $_.ComputerAlgebraSystem.Parser.f14), $_.ComputerAlgebraSystem.Parser.f17, $_.ComputerAlgebraSystem.Parser.f18);
                    this.idCoeffPow = ComputerAlgebraSystem.Parse.selectMany(ComputerAlgebraSystem.Parse.selectMany(ComputerAlgebraSystem.Parse.selectMany(ComputerAlgebraSystem.Parser.constant, $_.ComputerAlgebraSystem.Parser.f19, $_.ComputerAlgebraSystem.Parser.f20), $_.ComputerAlgebraSystem.Parser.f21, $_.ComputerAlgebraSystem.Parser.f22), $_.ComputerAlgebraSystem.Parser.f23, $_.ComputerAlgebraSystem.Parser.f24);
                    this.idPowId = ComputerAlgebraSystem.Parse.selectMany(ComputerAlgebraSystem.Parse.selectMany(ComputerAlgebraSystem.Parser.identifier, $_.ComputerAlgebraSystem.Parser.f9, $_.ComputerAlgebraSystem.Parser.f10), $_.ComputerAlgebraSystem.Parser.f25, $_.ComputerAlgebraSystem.Parser.f26);
                    this.term = ComputerAlgebraSystem.Parse.or(ComputerAlgebraSystem.Parse.or(ComputerAlgebraSystem.Parse.or(ComputerAlgebraSystem.Parse.or(ComputerAlgebraSystem.Parse.or(ComputerAlgebraSystem.Parse.or(ComputerAlgebraSystem.Parser.idCoeffPow, ComputerAlgebraSystem.Parser.constTimesId), ComputerAlgebraSystem.Parser.constPowId), ComputerAlgebraSystem.Parser.idPowConst), ComputerAlgebraSystem.Parser.constPowConst), ComputerAlgebraSystem.Parser.identifier), ComputerAlgebraSystem.Parser.constant);
                    this.unaryFunction = ComputerAlgebraSystem.Parse.selectMany(ComputerAlgebraSystem.Parse.selectMany(ComputerAlgebraSystem.Parse.selectMany(ComputerAlgebraSystem.Parser.identifier, $_.ComputerAlgebraSystem.Parser.f27, $_.ComputerAlgebraSystem.Parser.f28), $_.ComputerAlgebraSystem.Parser.f29, $_.ComputerAlgebraSystem.Parser.f30), $_.ComputerAlgebraSystem.Parser.f31, $_.ComputerAlgebraSystem.Parser.f32);
                    this.binaryFunction = ComputerAlgebraSystem.Parse.selectMany(ComputerAlgebraSystem.Parse.selectMany(ComputerAlgebraSystem.Parse.selectMany(ComputerAlgebraSystem.Parse.selectMany(ComputerAlgebraSystem.Parse.selectMany(ComputerAlgebraSystem.Parser.identifier, $_.ComputerAlgebraSystem.Parser.f27, $_.ComputerAlgebraSystem.Parser.f28), $_.ComputerAlgebraSystem.Parser.f33, $_.ComputerAlgebraSystem.Parser.f34), $_.ComputerAlgebraSystem.Parser.f35, $_.ComputerAlgebraSystem.Parser.f36), $_.ComputerAlgebraSystem.Parser.f37, $_.ComputerAlgebraSystem.Parser.f38), $_.ComputerAlgebraSystem.Parser.f39, $_.ComputerAlgebraSystem.Parser.f40);
                    this.function = ComputerAlgebraSystem.Parse.or(ComputerAlgebraSystem.Parser.binaryFunction, ComputerAlgebraSystem.Parser.unaryFunction);
                    this.expression = ComputerAlgebraSystem.Parse.then(ComputerAlgebraSystem.Parse.optional(ComputerAlgebraSystem.Parse.char(40)), $_.ComputerAlgebraSystem.Parser.f42);
                }
            },
            double: function () {
                return ComputerAlgebraSystem.Parse.then(ComputerAlgebraSystem.Parse.many(ComputerAlgebraSystem.Parse.digit), $_.ComputerAlgebraSystem.Parser.f43);
            },
            makeIdentiefier: function (value) {
                switch (value) {
                    case "pi": 
                        return ComputerAlgebraSystem.Expr.constant(Math.PI);
                    case "e": 
                        return ComputerAlgebraSystem.Expr.constant(Math.E);
                    default: 
                        return ComputerAlgebraSystem.Expr.symbol(value);
                }
            },
            parseInput: function (input) {
                var cleanInput = System.String.replaceAll(input, " ", "").toLowerCase();
                return ComputerAlgebraSystem.ParserExtensions.parse(ComputerAlgebraSystem.Parser.expression, cleanInput);
            },
            tryParseInput: function (input) {
                var cleanInput = System.String.replaceAll(input, " ", "").toLowerCase();
                return ComputerAlgebraSystem.ParserExtensions.tryParse(ComputerAlgebraSystem.Parser.expression, cleanInput);
            }
        }
    });
    
    Bridge.define("$AnonymousType$4", $_, {
        constructor: function (first, rest) {
            this.first = first;
            this.rest = rest;
        },
        getfirst : function () {
            return this.first;
        },
        getrest : function () {
            return this.rest;
        },
        equals: function (o) {
            if (!Bridge.is(o, $_.$AnonymousType$4)) {
                return false;
            }
            return Bridge.equals(this.first, o.first) && Bridge.equals(this.rest, o.rest);
        },
        getHashCode: function () {
            var hash = 17;
            hash = hash * 23 + 545831905;
            hash = hash * 23 + (this.first == null ? 0 : Bridge.getHashCode(this.first));
            hash = hash * 23 + (this.rest == null ? 0 : Bridge.getHashCode(this.rest));
            return hash;
        },
        toJSON: function () {
            return {
                first : this.first,
                rest : this.rest
            };
        }
    });
    
    Bridge.define("$AnonymousType$5", $_, {
        constructor: function (x0, value) {
            this.x0 = x0;
            this.value = value;
        },
        getx0 : function () {
            return this.x0;
        },
        getvalue : function () {
            return this.value;
        },
        equals: function (o) {
            if (!Bridge.is(o, $_.$AnonymousType$5)) {
                return false;
            }
            return Bridge.equals(this.x0, o.x0) && Bridge.equals(this.value, o.value);
        },
        getHashCode: function () {
            var hash = 17;
            hash = hash * 23 + -1020252036;
            hash = hash * 23 + (this.x0 == null ? 0 : Bridge.getHashCode(this.x0));
            hash = hash * 23 + (this.value == null ? 0 : Bridge.getHashCode(this.value));
            return hash;
        },
        toJSON: function () {
            return {
                x0 : this.x0,
                value : this.value
            };
        }
    });
    
    Bridge.define("$AnonymousType$6", $_, {
        constructor: function (id, hat) {
            this.id = id;
            this.hat = hat;
        },
        getid : function () {
            return this.id;
        },
        gethat : function () {
            return this.hat;
        },
        equals: function (o) {
            if (!Bridge.is(o, $_.$AnonymousType$6)) {
                return false;
            }
            return Bridge.equals(this.id, o.id) && Bridge.equals(this.hat, o.hat);
        },
        getHashCode: function () {
            var hash = 17;
            hash = hash * 23 + 1708631319;
            hash = hash * 23 + (this.id == null ? 0 : Bridge.getHashCode(this.id));
            hash = hash * 23 + (this.hat == null ? 0 : Bridge.getHashCode(this.hat));
            return hash;
        },
        toJSON: function () {
            return {
                id : this.id,
                hat : this.hat
            };
        }
    });
    
    Bridge.define("$AnonymousType$7", $_, {
        constructor: function (factor, hat) {
            this.factor = factor;
            this.hat = hat;
        },
        getfactor : function () {
            return this.factor;
        },
        gethat : function () {
            return this.hat;
        },
        equals: function (o) {
            if (!Bridge.is(o, $_.$AnonymousType$7)) {
                return false;
            }
            return Bridge.equals(this.factor, o.factor) && Bridge.equals(this.hat, o.hat);
        },
        getHashCode: function () {
            var hash = 17;
            hash = hash * 23 + 142547378;
            hash = hash * 23 + (this.factor == null ? 0 : Bridge.getHashCode(this.factor));
            hash = hash * 23 + (this.hat == null ? 0 : Bridge.getHashCode(this.hat));
            return hash;
        },
        toJSON: function () {
            return {
                factor : this.factor,
                hat : this.hat
            };
        }
    });
    
    Bridge.define("$AnonymousType$8", $_, {
        constructor: function (constant, id) {
            this.constant = constant;
            this.id = id;
        },
        getconstant : function () {
            return this.constant;
        },
        getid : function () {
            return this.id;
        },
        equals: function (o) {
            if (!Bridge.is(o, $_.$AnonymousType$8)) {
                return false;
            }
            return Bridge.equals(this.constant, o.constant) && Bridge.equals(this.id, o.id);
        },
        getHashCode: function () {
            var hash = 17;
            hash = hash * 23 + -1779766923;
            hash = hash * 23 + (this.constant == null ? 0 : Bridge.getHashCode(this.constant));
            hash = hash * 23 + (this.id == null ? 0 : Bridge.getHashCode(this.id));
            return hash;
        },
        toJSON: function () {
            return {
                constant : this.constant,
                id : this.id
            };
        }
    });
    
    Bridge.define("$AnonymousType$9", $_, {
        constructor: function (x9, hat) {
            this.x9 = x9;
            this.hat = hat;
        },
        getx9 : function () {
            return this.x9;
        },
        gethat : function () {
            return this.hat;
        },
        equals: function (o) {
            if (!Bridge.is(o, $_.$AnonymousType$9)) {
                return false;
            }
            return Bridge.equals(this.x9, o.x9) && Bridge.equals(this.hat, o.hat);
        },
        getHashCode: function () {
            var hash = 17;
            hash = hash * 23 + 949116432;
            hash = hash * 23 + (this.x9 == null ? 0 : Bridge.getHashCode(this.x9));
            hash = hash * 23 + (this.hat == null ? 0 : Bridge.getHashCode(this.hat));
            return hash;
        },
        toJSON: function () {
            return {
                x9 : this.x9,
                hat : this.hat
            };
        }
    });
    
    Bridge.define("$AnonymousType$10", $_, {
        constructor: function (funcName, lparen) {
            this.funcName = funcName;
            this.lparen = lparen;
        },
        getfuncName : function () {
            return this.funcName;
        },
        getlparen : function () {
            return this.lparen;
        },
        equals: function (o) {
            if (!Bridge.is(o, $_.$AnonymousType$10)) {
                return false;
            }
            return Bridge.equals(this.funcName, o.funcName) && Bridge.equals(this.lparen, o.lparen);
        },
        getHashCode: function () {
            var hash = 17;
            hash = hash * 23 + 1891500776;
            hash = hash * 23 + (this.funcName == null ? 0 : Bridge.getHashCode(this.funcName));
            hash = hash * 23 + (this.lparen == null ? 0 : Bridge.getHashCode(this.lparen));
            return hash;
        },
        toJSON: function () {
            return {
                funcName : this.funcName,
                lparen : this.lparen
            };
        }
    });
    
    Bridge.define("$AnonymousType$11", $_, {
        constructor: function (x15, arg) {
            this.x15 = x15;
            this.arg = arg;
        },
        getx15 : function () {
            return this.x15;
        },
        getarg : function () {
            return this.arg;
        },
        equals: function (o) {
            if (!Bridge.is(o, $_.$AnonymousType$11)) {
                return false;
            }
            return Bridge.equals(this.x15, o.x15) && Bridge.equals(this.arg, o.arg);
        },
        getHashCode: function () {
            var hash = 17;
            hash = hash * 23 + -447151384;
            hash = hash * 23 + (this.x15 == null ? 0 : Bridge.getHashCode(this.x15));
            hash = hash * 23 + (this.arg == null ? 0 : Bridge.getHashCode(this.arg));
            return hash;
        },
        toJSON: function () {
            return {
                x15 : this.x15,
                arg : this.arg
            };
        }
    });
    
    Bridge.define("$AnonymousType$12", $_, {
        constructor: function (x19, fstArg) {
            this.x19 = x19;
            this.fstArg = fstArg;
        },
        getx19 : function () {
            return this.x19;
        },
        getfstArg : function () {
            return this.fstArg;
        },
        equals: function (o) {
            if (!Bridge.is(o, $_.$AnonymousType$12)) {
                return false;
            }
            return Bridge.equals(this.x19, o.x19) && Bridge.equals(this.fstArg, o.fstArg);
        },
        getHashCode: function () {
            var hash = 17;
            hash = hash * 23 + 1509163752;
            hash = hash * 23 + (this.x19 == null ? 0 : Bridge.getHashCode(this.x19));
            hash = hash * 23 + (this.fstArg == null ? 0 : Bridge.getHashCode(this.fstArg));
            return hash;
        },
        toJSON: function () {
            return {
                x19 : this.x19,
                fstArg : this.fstArg
            };
        }
    });
    
    Bridge.define("$AnonymousType$13", $_, {
        constructor: function (x21, comma) {
            this.x21 = x21;
            this.comma = comma;
        },
        getx21 : function () {
            return this.x21;
        },
        getcomma : function () {
            return this.comma;
        },
        equals: function (o) {
            if (!Bridge.is(o, $_.$AnonymousType$13)) {
                return false;
            }
            return Bridge.equals(this.x21, o.x21) && Bridge.equals(this.comma, o.comma);
        },
        getHashCode: function () {
            var hash = 17;
            hash = hash * 23 + -829488408;
            hash = hash * 23 + (this.x21 == null ? 0 : Bridge.getHashCode(this.x21));
            hash = hash * 23 + (this.comma == null ? 0 : Bridge.getHashCode(this.comma));
            return hash;
        },
        toJSON: function () {
            return {
                x21 : this.x21,
                comma : this.comma
            };
        }
    });
    
    Bridge.define("$AnonymousType$14", $_, {
        constructor: function (x23, sndArg) {
            this.x23 = x23;
            this.sndArg = sndArg;
        },
        getx23 : function () {
            return this.x23;
        },
        getsndArg : function () {
            return this.sndArg;
        },
        equals: function (o) {
            if (!Bridge.is(o, $_.$AnonymousType$14)) {
                return false;
            }
            return Bridge.equals(this.x23, o.x23) && Bridge.equals(this.sndArg, o.sndArg);
        },
        getHashCode: function () {
            var hash = 17;
            hash = hash * 23 + 1126826728;
            hash = hash * 23 + (this.x23 == null ? 0 : Bridge.getHashCode(this.x23));
            hash = hash * 23 + (this.sndArg == null ? 0 : Bridge.getHashCode(this.sndArg));
            return hash;
        },
        toJSON: function () {
            return {
                x23 : this.x23,
                sndArg : this.sndArg
            };
        }
    });
    
    Bridge.ns("ComputerAlgebraSystem.Parser", $_);
    
    Bridge.apply($_.ComputerAlgebraSystem.Parser, {
        f1: function (input) {
            return ComputerAlgebraSystem.Parse.return(System.Double.parse(input));
        },
        f2: function (value) {
            return new ComputerAlgebraSystem.Constant(value);
        },
        f3: function (first) {
            return ComputerAlgebraSystem.Parse.many(ComputerAlgebraSystem.Parse.letterOrDigit);
        },
        f4: function (first, rest) {
            return new $_.$AnonymousType$4(first, rest);
        },
        f5: function (x0) {
            return new $_.$AnonymousType$5(x0, String.fromCharCode.apply(null, System.Linq.Enumerable.from(x0.first).concat(x0.rest).toArray()));
        },
        f6: function (x1) {
            return ComputerAlgebraSystem.Parser.makeIdentiefier(x1.value);
        },
        f7: function (value) {
            return ComputerAlgebraSystem.Parser.identifier;
        },
        f8: function (value, id) {
            return ComputerAlgebraSystem.Expr.times(value, id);
        },
        f9: function (id) {
            return ComputerAlgebraSystem.Parse.char(94);
        },
        f10: function (id, hat) {
            return new $_.$AnonymousType$6(id, hat);
        },
        f11: function (x2) {
            return ComputerAlgebraSystem.Parser.constant;
        },
        f12: function (x3, pow) {
            return ComputerAlgebraSystem.Expr.pow(x3.id, pow);
        },
        f13: function (factor) {
            return ComputerAlgebraSystem.Parse.char(94);
        },
        f14: function (factor, hat) {
            return new $_.$AnonymousType$7(factor, hat);
        },
        f15: function (x4) {
            return ComputerAlgebraSystem.Parser.constant;
        },
        f16: function (x5, pow) {
            return ComputerAlgebraSystem.Expr.pow(x5.factor, pow);
        },
        f17: function (x6) {
            return ComputerAlgebraSystem.Parser.identifier;
        },
        f18: function (x7, id) {
            return ComputerAlgebraSystem.Expr.pow(x7.factor, id);
        },
        f19: function (constant) {
            return ComputerAlgebraSystem.Parser.identifier;
        },
        f20: function (constant, id) {
            return new $_.$AnonymousType$8(constant, id);
        },
        f21: function (x8) {
            return ComputerAlgebraSystem.Parse.char(94);
        },
        f22: function (x9, hat) {
            return new $_.$AnonymousType$9(x9, hat);
        },
        f23: function (x10) {
            return ComputerAlgebraSystem.Parser.constant;
        },
        f24: function (x11, pow) {
            return ComputerAlgebraSystem.Expr.times(x11.x9.constant, ComputerAlgebraSystem.Expr.pow(x11.x9.id, pow));
        },
        f25: function (x12) {
            return ComputerAlgebraSystem.Parser.identifier;
        },
        f26: function (x13, powId) {
            return ComputerAlgebraSystem.Expr.pow(x13.id, powId);
        },
        f27: function (funcName) {
            return ComputerAlgebraSystem.Parse.char(40);
        },
        f28: function (funcName, lparen) {
            return new $_.$AnonymousType$10(funcName, lparen);
        },
        f29: function (x14) {
            return ComputerAlgebraSystem.Parse.or(ComputerAlgebraSystem.Parser.function, ComputerAlgebraSystem.Parser.term);
        },
        f30: function (x15, arg) {
            return new $_.$AnonymousType$11(x15, arg);
        },
        f31: function (x16) {
            return ComputerAlgebraSystem.Parse.char(41);
        },
        f32: function (x17, rparen) {
            return new ComputerAlgebraSystem.Function("constructor", ComputerAlgebraSystem.Expr.symbolValue(x17.x15.funcName), x17.arg);
        },
        f33: function (x18) {
            return ComputerAlgebraSystem.Parse.or(ComputerAlgebraSystem.Parser.function, ComputerAlgebraSystem.Parser.term);
        },
        f34: function (x19, fstArg) {
            return new $_.$AnonymousType$12(x19, fstArg);
        },
        f35: function (x20) {
            return ComputerAlgebraSystem.Parse.char(44);
        },
        f36: function (x21, comma) {
            return new $_.$AnonymousType$13(x21, comma);
        },
        f37: function (x22) {
            return ComputerAlgebraSystem.Parse.or(ComputerAlgebraSystem.Parser.function, ComputerAlgebraSystem.Parser.term);
        },
        f38: function (x23, sndArg) {
            return new $_.$AnonymousType$14(x23, sndArg);
        },
        f39: function (x24) {
            return ComputerAlgebraSystem.Parse.char(41);
        },
        f40: function (x25, rparen) {
            return new ComputerAlgebraSystem.Function("constructor$1", ComputerAlgebraSystem.Expr.symbolValue(x25.x23.x21.x19.funcName), x25.x23.x21.fstArg, x25.sndArg);
        },
        f41: function (expr) {
            return ComputerAlgebraSystem.Parse.then(ComputerAlgebraSystem.Parse.optional(ComputerAlgebraSystem.Parse.char(41)), function (rparen) {
                return ComputerAlgebraSystem.Parse.return(expr);
            });
        },
        f42: function (lparen) {
            return ComputerAlgebraSystem.Parse.then(ComputerAlgebraSystem.Parse.or(ComputerAlgebraSystem.Parser.function, ComputerAlgebraSystem.Parser.term), $_.ComputerAlgebraSystem.Parser.f41);
        },
        f43: function (digits) {
            return ComputerAlgebraSystem.Parse.then(ComputerAlgebraSystem.Parse.char(46), function (dot) {
                return ComputerAlgebraSystem.Parse.then(ComputerAlgebraSystem.Parse.many(ComputerAlgebraSystem.Parse.digit), function (fractionPart) {
                    var firstPartString = String.fromCharCode.apply(null, System.Linq.Enumerable.from(digits).toArray());
                    var fractionPartString = String.fromCharCode.apply(null, System.Linq.Enumerable.from(fractionPart).toArray());
    
                    var firstPart = System.Double.parse(firstPartString);
                    var fracPartDouble = System.Double.parse(fractionPartString);
    
                    return ComputerAlgebraSystem.Parse.return(firstPart + (fracPartDouble / Math.pow(10.0, fractionPartString.length)));
    
                });
            });
        }
    });
    
    Bridge.define('ComputerAlgebraSystem.ParserExtensions', {
        statics: {
            tryParse: function (parser, input) {
                if (Bridge.staticEquals(parser, null)) {
                    throw new System.ArgumentNullException("parser");
                }
                if (input == null) {
                    throw new System.ArgumentNullException("input");
                }
    
                return parser(new ComputerAlgebraSystem.Input("constructor", input));
            },
            parse: function (parser, input) {
                if (Bridge.staticEquals(parser, null)) {
                    throw new System.ArgumentNullException("parser");
                }
                if (input == null) {
                    throw new System.ArgumentNullException("input");
                }
    
                var result = ComputerAlgebraSystem.ParserExtensions.tryParse(parser, input);
    
                if (result.getWasSuccessful()) {
                    return result.getValue();
                }
    
                throw new ComputerAlgebraSystem.ParseException("constructor$1", result.toString());
            }
        }
    });
    
    Bridge.define('ComputerAlgebraSystem.Position', {
        inherits: function () { return [System.IEquatable$1(ComputerAlgebraSystem.Position)]; },
        statics: {
            fromInput: function (input) {
                return new ComputerAlgebraSystem.Position(input.getPosition(), input.getLine(), input.getColumn());
            },
            op_Equality: function (left, right) {
                return Bridge.equals(left, right);
            },
            op_Inequality: function (left, right) {
                return !Bridge.equals(left, right);
            }
        },
        config: {
            properties: {
                Pos: 0,
                Line: 0,
                Column: 0
            }
        },
        constructor: function (pos, line, column) {
            this.setPos(pos);
            this.setLine(line);
            this.setColumn(column);
        },
        equals: function (obj) {
            return this.equalsT(Bridge.as(obj, ComputerAlgebraSystem.Position));
        },
        equalsT: function (other) {
            if (Bridge.referenceEquals(null, other)) {
                return false;
            }
            if (Bridge.referenceEquals(this, other)) {
                return true;
            }
            return this.getPos() === other.getPos() && this.getLine() === other.getLine() && this.getColumn() === other.getColumn();
        },
        getHashCode: function () {
            var h = 31;
            h = (((h * 13) | 0) + this.getPos()) | 0;
            h = (((h * 13) | 0) + this.getLine()) | 0;
            h = (((h * 13) | 0) + this.getColumn()) | 0;
            return h;
        },
        toString: function () {
            return System.String.format("Line {0}, Column {1}", this.getLine(), this.getColumn());
        }
    });
    
    Bridge.define('ComputerAlgebraSystem.Result', {
        statics: {
            success: function (value, remainder) {
                return new ComputerAlgebraSystem.Result$1("constructor", value, remainder);
            },
            failure: function (remainder, message, expectations) {
                return new ComputerAlgebraSystem.Result$1("constructor$1", remainder, message, expectations);
            }
        }
    });
    
    Bridge.define('ComputerAlgebraSystem.ResultHelper', {
        statics: {
            ifSuccess: function (result, next) {
                if (result == null) {
                    throw new System.ArgumentNullException("result");
                }
    
                if (result.getWasSuccessful()) {
                    return next(result);
                }
    
                return ComputerAlgebraSystem.Result.failure(result.getRemainder(), result.getMessage(), result.getExpectations());
            },
            ifFailure: function (result, next) {
                if (result == null) {
                    throw new System.ArgumentNullException("result");
                }
    
                return result.getWasSuccessful() ? result : next(result);
            }
        }
    });
    
    Bridge.define('ComputerAlgebraSystem.TinyPlotter', {
        settings: null,
        canvas: null,
        constructor: function (settings) {
            this.settings = settings;
            this.canvas = document.createElement('canvas');
            this.canvas.height = this.settings.height;
            this.canvas.width = this.settings.width;
            this.canvas.style.border = "1px solid black";
            var ctx = this.canvas.getContext("2d");
            var image = ctx.createImageData(this.canvas.width, this.canvas.height);
            if (this.settings.drawXAxis) {
                this.drawXAxis(image);
            }
            if (this.settings.drawXAxis) {
                this.drawYAxis(image);
            }
            ctx.putImageData(image, 0, 0);
        },
        draw: function () {
            var $t;
            var ctx = this.canvas.getContext("2d");
            var image = ctx.createImageData(this.canvas.width, this.canvas.height);
    
            if (this.settings.drawXAxis) {
                this.drawXAxis(image);
            }
    
            if (this.settings.drawXAxis) {
                this.drawYAxis(image);
            }
    
            $t = Bridge.getEnumerator(this.settings.curves);
            while ($t.moveNext()) {
                var curve = $t.getCurrent();
                this.drawCurve(curve, image);
            }
    
            ctx.putImageData(image, 0, 0);
        },
        drawXAxis: function (image) {
            var xmin = this.settings.viewport.xMin;
            var xmax = this.settings.viewport.xMax;
            var ymin = this.settings.viewport.yMin;
            var ymax = this.settings.viewport.yMax;
            var step = this.settings.stepX;
    
            for (var x = xmin; x <= xmax; x += 0.01) {
                var point = { x: x, y: 0 };
                var pointFromPlain = this.fromPointOnPlain(point, xmin, xmax, ymin, ymax, this.canvas.height, this.canvas.width);
                this.setPixel(image, Bridge.Int.clip32(pointFromPlain.x), Bridge.Int.clip32(pointFromPlain.y), { red: 0, blue: 0, green: 0 });
            }
        },
        drawYAxis: function (image) {
            var xmin = this.settings.viewport.xMin;
            var xmax = this.settings.viewport.xMax;
            var ymin = this.settings.viewport.yMin;
            var ymax = this.settings.viewport.yMax;
            var step = this.settings.stepX;
    
            for (var y = ymin; y <= ymax; y += 0.01) {
                var point = { x: 0, y: y };
                var pointFromPlain = this.fromPointOnPlain(point, xmin, xmax, ymin, ymax, this.canvas.height, this.canvas.width);
                this.setPixel(image, Bridge.Int.clip32(pointFromPlain.x), Bridge.Int.clip32(pointFromPlain.y), { red: 0, blue: 0, green: 0 });
            }
        },
        drawCurve: function (curve, image) {
            var xmin = this.settings.viewport.xMin;
            var xmax = this.settings.viewport.xMax;
            var ymin = this.settings.viewport.yMin;
            var ymax = this.settings.viewport.yMax;
            var step = this.settings.stepX;
    
            for (var x = xmin; x <= xmax; x += step) {
                var y = curve.map(x);
                if (y < ymin || y > ymax || isNaN(y)) {
                    continue;
                }
    
                var point = { x: x, y: y };
                var pointFromPlain = this.fromPointOnPlain(point, xmin, xmax, ymin, ymax, this.canvas.height, this.canvas.width);
                this.setPixel(image, Bridge.Int.clip32(pointFromPlain.x), Bridge.Int.clip32(pointFromPlain.y), curve.color);
            }
        },
        rescale: function (value, realRange, projection) {
            if (value > realRange.to || value < realRange.from) {
                throw new System.ArgumentException("value is not the real range");
            }
    
            var percentageOfProjection = (Math.abs(projection.to - projection.from) * Math.abs(value - realRange.from)) / Math.abs(realRange.to - realRange.from);
    
            return percentageOfProjection + projection.from;
        },
        fromPointOnPlain: function (p, xmin, xmax, ymin, ymax, height, width) {
            return { x: this.rescale(p.x, { from: xmin, to: xmax }, { from: 0, to: width }), y: height - this.rescale(p.y, { from: ymin, to: ymax }, { from: 0, to: height }) };
        },
        setPixel: function (img, x, y, color) {
            var index = ((((x + ((y * (img.width | 0)) | 0)) | 0)) * 4) | 0;
            img.data[index] = color.red;
            img.data[((index + 1) | 0)] = color.green;
            img.data[((index + 2) | 0)] = color.blue;
            img.data[((index + 3) | 0)] = 255; // alpha
        }
    });
    
    Bridge.define('ComputerAlgebraSystem.AbstractOption$1', {
        inherits: [ComputerAlgebraSystem.IOption$1],
        getIsDefined: function () {
            return !this.getIsEmpty();
        },
        getOrDefault: function () {
            return this.getIsEmpty() ? Bridge.getDefaultValue(T) : this.get();
        }
    });
    
    Bridge.define('ComputerAlgebraSystem.Constant', {
        inherits: [ComputerAlgebraSystem.IExpr],
        config: {
            properties: {
                Value: 0
            }
        },
        constructor: function (value) {
            this.setValue(value);
        },
        toString: function () {
            return System.Double.format(this.getValue(), 'G');
        }
    });
    
    Bridge.define('ComputerAlgebraSystem.Function', {
        inherits: [ComputerAlgebraSystem.IExpr],
        config: {
            properties: {
                FirstArgument: null,
                SecondArgument: null,
                IsBinary: false,
                Name: null
            }
        },
        constructor: function (name, fstExpr) {
            this.setIsBinary(false);
            this.setName(name);
            this.setFirstArgument(fstExpr);
        },
        constructor$1: function (name, fst, snd) {
            this.setIsBinary(true);
            this.setName(name);
            this.setFirstArgument(fst);
            this.setSecondArgument(snd);
        },
        toString: function () {
            return this.getIsBinary() ? System.String.format("{0}({1}, {2})", this.getName(), this.getFirstArgument(), this.getSecondArgument()) : System.String.format("{0}({1})", this.getName(), this.getFirstArgument());
        }
    });
    
    Bridge.define('ComputerAlgebraSystem.Input', {
        inherits: [ComputerAlgebraSystem.IInput],
        statics: {
            op_Equality: function (left, right) {
                return Bridge.equals(left, right);
            },
            op_Inequality: function (left, right) {
                return !Bridge.equals(left, right);
            }
        },
        _source: null,
        _position: 0,
        _line: 0,
        _column: 0,
        config: {
            properties: {
                Memos: null
            }
        },
        constructor: function (source) {
            ComputerAlgebraSystem.Input.prototype.constructor$1.call(this, source, 0);
    
        },
        constructor$1: function (source, position, line, column) {
            if (line === void 0) { line = 1; }
            if (column === void 0) { column = 1; }
    
            this._source = source;
            this._position = position;
            this._line = line;
            this._column = column;
            this.setMemos(new System.Collections.Generic.Dictionary$2(Object,Object)());
        },
        getSource: function () {
            return this._source;
        },
        getCurrent: function () {
            return this._source.charCodeAt(this._position);
        },
        getAtEnd: function () {
            return this._position === this._source.length;
        },
        getPosition: function () {
            return this._position;
        },
        getLine: function () {
            return this._line;
        },
        getColumn: function () {
            return this._column;
        },
        advance: function () {
            if (this.getAtEnd()) {
                throw new System.InvalidOperationException("The input is already at the end of the source.");
            }
            return new ComputerAlgebraSystem.Input("constructor$1", this._source, ((this._position + 1) | 0), this.getCurrent() === 10 ? ((this._line + 1) | 0) : this._line, this.getCurrent() === 10 ? 1 : ((this._column + 1) | 0));
        },
        toString: function () {
            return System.String.format("Line {0}, Column {1}", this._line, this._column);
        },
        getHashCode: function () {
            return ((((this._source != null ? Bridge.getHashCode(this._source) : 0) * 397) | 0)) ^ this._position;
        },
        equals: function (obj) {
            return this.equalsT(Bridge.as(obj, ComputerAlgebraSystem.IInput));
        },
        equalsT: function (other) {
            if (Bridge.referenceEquals(null, other)) {
                return false;
            }
            if (Bridge.referenceEquals(this, other)) {
                return true;
            }
            return System.String.equals(this._source, other.getSource()) && this._position === other.getPosition();
        }
    });
    
    Bridge.define('ComputerAlgebraSystem.Result$1', {
        inherits: [ComputerAlgebraSystem.IResult$1],
        _value: null,
        _remainder: null,
        _wasSuccessful: false,
        _message: null,
        _expectations: null,
        constructor: function (value, remainder) {
            this._value = value;
            this._remainder = remainder;
            this._wasSuccessful = true;
            this._message = null;
            this._expectations = System.Linq.Enumerable.empty();
        },
        constructor$1: function (remainder, message, expectations) {
            this._value = null;
            this._remainder = remainder;
            this._wasSuccessful = false;
            this._message = message;
            this._expectations = expectations;
        },
        getValue: function () {
            if (!this.getWasSuccessful()) {
                throw new System.InvalidOperationException("No value can be computed.");
            }
    
            return this._value;
        },
        getWasSuccessful: function () {
            return this._wasSuccessful;
        },
        getMessage: function () {
            return this._message;
        },
        getExpectations: function () {
            return this._expectations;
        },
        getRemainder: function () {
            return this._remainder;
        },
        toString: function () {
            if (this.getWasSuccessful()) {
                return System.String.format("Successful parsing of {0}.", this.getValue());
            }
    
            var expMsg = "";
    
            if (System.Linq.Enumerable.from(this.getExpectations()).any()) {
                expMsg = " expected " + System.Linq.Enumerable.from(this.getExpectations()).aggregate($_.ComputerAlgebraSystem.Result$1.f1);
            }
    
            var recentlyConsumed = this.calculateRecentlyConsumed();
    
            return System.String.format("Parsing failure: {0};{1} ({2}); recently consumed: {3}", this.getMessage(), expMsg, this.getRemainder(), recentlyConsumed);
        },
        calculateRecentlyConsumed: function () {
            var windowSize = 10;
    
            var totalConsumedChars = this.getRemainder().getPosition();
            var windowStart = (totalConsumedChars - windowSize) | 0;
            windowStart = windowStart < 0 ? 0 : windowStart;
    
            var numberOfRecentlyConsumedChars = (totalConsumedChars - windowStart) | 0;
    
            return this.getRemainder().getSource().substr(windowStart, numberOfRecentlyConsumedChars);
        }
    });
    
    Bridge.ns("ComputerAlgebraSystem.Result$1", $_);
    
    Bridge.apply($_.ComputerAlgebraSystem.Result$1, {
        f1: function (e1, e2) {
            return e1 + " or " + e2;
        }
    });
    
    Bridge.define('ComputerAlgebraSystem.Symbol', {
        inherits: [ComputerAlgebraSystem.IExpr],
        config: {
            properties: {
                Value: null
            }
        },
        constructor: function (symbol) {
            this.setValue(symbol);
        },
        toString: function () {
            return this.getValue().toString();
        }
    });
    
    Bridge.define('ComputerAlgebraSystem.None$1', {
        inherits: [ComputerAlgebraSystem.AbstractOption$1],
        getIsEmpty: function () {
            return true;
        },
        get: function () {
            throw new System.InvalidOperationException("Cannot get value from None.");
        }
    });
    
    Bridge.define('ComputerAlgebraSystem.Some$1', {
        inherits: [ComputerAlgebraSystem.AbstractOption$1],
        _value: null,
        constructor: function (value) {
            ComputerAlgebraSystem.AbstractOption$1.prototype.$constructor.call(this);
    
            this._value = value;
        },
        getIsEmpty: function () {
            return false;
        },
        get: function () {
            return this._value;
        }
    });
    
    Bridge.init();
})(this);

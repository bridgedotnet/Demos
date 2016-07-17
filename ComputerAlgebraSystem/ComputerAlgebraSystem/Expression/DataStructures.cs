namespace ComputerAlgebraSystem
{
    public interface IExpr
    {
    }

    public class Constant : IExpr
    {

        public Constant(double value)
        {
            this.Value = value;
        }

        public double Value { get; }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    public class Symbol : IExpr
    {
        public string Value { get; }

        public Symbol(string symbol)
        {
            this.Value = symbol;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    public class Function : IExpr
    {
        public IExpr FirstArgument { get; }
        public IExpr SecondArgument { get; }
        public bool IsBinary { get; }
        public string Name { get; }

        public Function(string name, IExpr fstExpr)
        {
            this.IsBinary = false;
            this.Name = name;
            this.FirstArgument = fstExpr;
        }

        public Function(string name, IExpr fst, IExpr snd)
        {
            this.IsBinary = true;
            this.Name = name;
            this.FirstArgument = fst;
            this.SecondArgument = snd;
        }

        public override string ToString()
        {
            return IsBinary ?
                string.Format("{0}({1}, {2})", Name, FirstArgument, SecondArgument)
              : string.Format("{0}({1})", Name, FirstArgument);
        }

    }
}
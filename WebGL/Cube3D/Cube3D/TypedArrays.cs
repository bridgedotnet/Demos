using Bridge;
using Bridge.Html5;

namespace Html5.TypedArrays
{
    // No support in Bridge.NET yet - https://github.com/bridgedotnet/Bridge/issues/182
    [Ignore]
    [Name("Float32Array")]
    public class Float32Array : ArrayBuffer
    {
        public Float32Array(float[] array) { }

        public Float32Array(double[] array) { }
    }

    [Ignore]
    [Name("Uint16Array")]
    public class Uint16Array : ArrayBuffer
    {
        public Uint16Array(int[] array) { }
    }
}
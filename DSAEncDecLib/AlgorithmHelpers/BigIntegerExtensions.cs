namespace DSAEncDecLib.AlgorithmHelpers
{
    using System;
    using System.Numerics;

    public static class BigIntegerExtensions
    {
        public static double Log2(this BigInteger n)
        {
            return Math.Ceiling(BigInteger.Log10(n) / BigInteger.Log10(2));
        }
    }
}
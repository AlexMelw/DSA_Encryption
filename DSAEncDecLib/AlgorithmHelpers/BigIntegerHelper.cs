namespace DSAEncDecLib.AlgorithmHelpers
{
    using System;
    using System.Numerics;
    using System.Security.Cryptography;

    public static class BigIntegerHelper
    {
        public static BigInteger NextBigInteger(BigInteger min, BigInteger max)
        {
            if (min > max) throw new ArgumentOutOfRangeException(nameof(min));
            if (min == max) return min;

            using (var rng = new RNGCryptoServiceProvider())
            {
                //byte[] data = new byte[((int)Log2(max))/8 + 1];
                byte[] data = new byte[max.ToByteArray().Length];
                rng.GetBytes(data);

                BigInteger generatedValue = BigInteger.Abs(new BigInteger(data));

                BigInteger diff = max - min;
                BigInteger mod = generatedValue % diff;
                BigInteger normalizedNumber = min + mod;

                return normalizedNumber;
            }
        }

        private static double Log2(BigInteger n)
        {
            return BigInteger.Log10(n) / Math.Log10(2);
        }
    }
}
namespace DSAEncDecLib.AlgorithmHelpers
{
    using System;
    using System.Numerics;
    using System.Security.Cryptography;
    using System.Threading;

    public static class BigIntegerPrimeExtensions
    {
        // Random generator (thread safe)
        private static readonly ThreadLocal<Random> SGen = new ThreadLocal<Random>(() => new Random());

        // Random generator (thread safe)
        private static Random Gen => SGen.Value;

        /// <summary>
        ///     Returns <see langword="true" /> if this BigInteger is probably prime, <see langword="false" /> if it's definitely
        ///     composite. If certainty is &lt;= 0, <see langword="true" /> is returned.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="witnesses">
        ///     Is a measure of the uncertainty that the caller is willing to tolerate: if the call returns
        ///     <see langword="true" /> the probability that this BigInteger is prime exceeds ( 1 - 1/(2^certainty) ). The
        ///     execution time of this method is proportional to the value of this parameter.
        /// </param>
        /// <returns>
        ///     <see langword="true" /> if this BigInteger is probably prime, <see langword="false" /> if it's definitely a
        ///     composite one.
        /// </returns>
        public static bool IsProbablyPrime(this BigInteger value, int witnesses = 10)
        {
            if (value <= 1)
                return false;

            if (witnesses <= 0)
                witnesses = 10;

            BigInteger d = value - 1;
            int s = 0;

            while (d % 2 == 0)
            {
                d /= 2;
                s += 1;
            }

            byte[] bytes = new byte[value.ToByteArray().LongLength];

            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                for (int i = 0; i < witnesses; i++)
                {
                    BigInteger a;
                    do
                    {
                        rng.GetBytes(bytes);

                        a = new BigInteger(bytes);
                    } while (a < 2 || a >= value - 2);

                    BigInteger x = BigInteger.ModPow(a, d, value);
                    if (x == 1 || x == value - 1)
                        continue;

                    for (int r = 1; r < s; r++)
                    {
                        x = BigInteger.ModPow(x, 2, value);

                        if (x == 1)
                            return false;
                        if (x == value - 1)
                            break;
                    }

                    if (x != value - 1)
                        return false;
                }
            }

            return true;
        }
    }
}
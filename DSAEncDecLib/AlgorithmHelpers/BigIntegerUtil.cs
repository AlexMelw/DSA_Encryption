namespace DSAEncDecLib.AlgorithmHelpers
{
    using System;
    using System.Numerics;
    using System.Security.Cryptography;

    public static class BigIntegerUtil
    {
        /// <summary>Returns a random <see cref="BigInteger" /> that is within a specified range.</summary>
        /// <param name="min">The inclusive lower bound of the random number returned.</param>
        /// <param name="max">
        ///     The exclusive upper bound of the random number returned. <paramref name="max" /> must be greater than or equal to
        ///     <paramref name="min" />.
        /// </param>
        /// <returns>
        ///     A signed <see cref="BigInteger" /> greater than or equal to <paramref name="min" /> and less than
        ///     <paramref name="min" />; that is, the range of return values includes <paramref name="min" /> but not
        ///     <paramref name="max" />. If <paramref name="min" /> equals <paramref name="max" />, <paramref name="min" /> is
        ///     returned.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="min" /> is greater than <paramref name="max" />.</exception>
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

        /// <summary>
        ///     Returns a random fixed size <see cref="BigInteger" /> number, which length is equal to
        ///     <param name="bitSize">.</param>
        /// </summary>
        /// <param name="bitSize">The length of generated <see cref="BigInteger" /> number.</param>
        /// <returns>
        ///     Random fixed size <see cref="BigInteger" /> number, which length is equal to
        ///     <param name="bitSize">.</param>
        /// </returns>
        public static BigInteger RandomFixedSizeBigInteger(int bitSize)
        {
            if (bitSize % 8 != 0)
            {
                throw new InvalidOperationException($"{nameof(bitSize)} should be a multiple of eight.");
            }

            using (var rng = new RNGCryptoServiceProvider())
            {
                int bytesCount = bitSize / 8;
                byte[] data = new byte[bytesCount];
                rng.GetBytes(data);

                return new BigInteger(data);
            }
        }

        /// <summary>
        ///     Returns random fixed size unsigned <see cref="BigInteger" /> number, which length is equal to
        ///     <param name="bitSize">.</param>
        /// </summary>
        /// <param name="bitSize">The length of generated unsigned <see cref="BigInteger" /> number.</param>
        /// <returns>
        ///     Random fixed size unsigned <see cref="BigInteger" /> number, which length is equal to
        ///     <param name="bitSize">.</param>
        /// </returns>
        public static BigInteger RandomPositiveFixedSizeBigInteger(int bitSize)
        {
            return BigInteger.Abs(RandomFixedSizeBigInteger(bitSize));
        }
    }
}
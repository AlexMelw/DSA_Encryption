using System;
using System.Numerics;

namespace Maurer
{
    using EasySharp.NHelpers.Utils.Cryptography;

    class HCSRAlgorithm
    {
        private Random _randomNumberGenerator;

        public HCSRAlgorithm(int seed)
        {
            _randomNumberGenerator = new Random(seed);
        }

        public bool Composite(BigInteger n, int t)
        {
            if (n == 2 || n == 3)
                return false;

            BigInteger m = n % 2;

            if (m == 0)
                return true;

            BigInteger n1 = n - 1;
            BigInteger r = n1;
            long s = 0;

            m = r % 2;

            while (m == 0)
            {
                r = r / 2;
                m = r % 2;
                s++;
            }

            BigInteger n2 = n - 2;

            for (int i = 1; i <= t; i++)
            {
                BigInteger a = RandomRange(2, n2);
                BigInteger y = BigInteger.ModPow(a, r, n);

                if (y != 1 && y != n1)
                {
                    int j = 1;

                    while (j <= s && y != n1)
                    {
                        y = BigInteger.ModPow(y, 2, n);

                        if (y == 1)
                            return true;

                        j++;
                    }

                    if (y != n1)
                        return true;
                }
            }

            return false;
        }

        private int Jacobi(BigInteger a, BigInteger n)
        {
            if (a == 0)
                return 0;

            if (a == 1)
                return 1;

            int e = 0;
            BigInteger a1 = a;
            BigInteger quotient = a1 / 2;
            BigInteger remainder = a1 % 2;

            while (remainder == 0)
            {
                e++;
                a1 = quotient;
                quotient = a1 / 2;
                remainder = a1 % 2;
            }

            int s = 0;

            if (e % 2 == 0)
                s = 1;

            else
            {
                BigInteger mod8 = n % 8;

                if (mod8 == 1 || mod8 == 7)
                    s = +1;

                if (mod8 == 3 || mod8 == 5)
                    s = -1;
            }

            BigInteger mod4 = n % 4;
            BigInteger a14 = a1 % 4;

            if (mod4 == 3 && a14 == 3)
                s = -s;

            BigInteger n1 = n % a1;

            return s * Jacobi(n1, a1);
        }

        public BigInteger RandomRange(BigInteger lower, BigInteger upper)
        {
            if (lower <= long.MaxValue && upper <= long.MaxValue && lower < upper)
            {
                BigInteger r;

                while (true)
                {
                    r = lower + (long) (((long) upper - (long) lower) * _randomNumberGenerator.NextDouble());

                    if (r >= lower && r <= upper)
                        return r;
                }
            }

            BigInteger delta = upper - lower;
            byte[] deltaBytes = delta.ToByteArray();
            byte[] buffer = new byte[deltaBytes.Length];

            while (true)
            {
                _randomNumberGenerator.NextBytes(buffer);

                BigInteger r = new BigInteger(buffer) + lower;

                if (r >= lower && r <= upper)
                    return r;
            }
        }

        public BigInteger SquareRootModPrime(BigInteger a, BigInteger p)
            // returns square root of a modulo p if it exists 0 otherwise
        {
            long e = 0, r, s;
            BigInteger b = 0, bp = 0, q = p - 1, m = 0, n = 0;
            BigInteger p1 = p - 1, t = 0, x = 0, y = 0, z = 0;

            // p - 1 = 2 ^ e * q with q odd

            m = q % 2;

            while (m == 0)
            {
                q = q / 2;
                m = q % 2;
                e++;
            }

            // find generator

            int jac = 0;

            do
            {
                n = RandomRange(1, p1);
                jac = Jacobi(n, p);
            } while (jac == -1);

            z = BigInteger.ModPow(n, q, p);
            y = z;
            r = e;
            x = BigInteger.ModPow(a, (q - 1) / 2, p);
            b = (((a * x) % p) * x) % p;
            x = (a * x) % p;

            while (true)
            {
                if (b == 1 || b == p1)
                    return x;

                s = 1;

                do
                {
                    bp = BigInteger.ModPow(b, (long) Math.Pow(2, s), p);
                    s++;
                } while (bp != 1 && bp != p1 && s < r);

                if (s == r)
                    return 0;

                t = BigInteger.ModPow(y, (long) Math.Pow(2, r - s - 1), p);
                y = (t * t) % p;
                x = (x * t) % p;
                b = (b * y) % p;
                r = s;
            }
        }
    }
}
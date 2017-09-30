namespace Maurer
{
    using System;
    using System.Collections.Generic;
    using System.Numerics;
    using EasySharp.NHelpers.Utils.Cryptography;

    class MaurerAlgorithm
    {
        private Random _randomNumberGenerator;
        private int _seed;

        public static MaurerAlgorithm Instance
        {
            get {
                int seed = RNGUtil.GenerateRandomInt();
                return new MaurerAlgorithm(seed);
            }
        }

        #region CONSTRUCTORS

        private MaurerAlgorithm(int seed)
        {
            _seed = seed;
            _randomNumberGenerator = new Random(seed);
        }

        #endregion

        public double Log2(BigInteger n)
        {
            return BigInteger.Log10(n) / Math.Log10(2);
        }

        public BigInteger ProvablePrime(int k)
        {
            BigInteger N = 0;
            List<long> primes = null;
            HCSRAlgorithm hc = new HCSRAlgorithm(_seed);
            //Random random = new Random(seed);

            if (k <= 20)
            {
                bool composite = true;

                while (composite)
                {
                    long n = 1 << (k - 1);

                    for (int i = 0; i < k - 1; i++)
                        n |= (long) _randomNumberGenerator.Next(2) << i;

                    long bound = (long) Math.Sqrt(n);

                    Sieve(bound, out primes);
                    composite = false;

                    for (int i = 0; !composite && i < primes.Count; i++)
                        composite = n % primes[i] == 0;

                    if (!composite)
                        N = n;
                }
            }

            else
            {
                double c = 0.1;
                int m = 20;

                double r = 0.5;

                if (k > 2 * m)
                {
                    bool done = false;

                    while (!done)
                    {
                        double s = _randomNumberGenerator.NextDouble();

                        r = Math.Pow(2, s - 1);
                        done = k - r * k > m;
                    }
                }

                BigInteger q = ProvablePrime((int) Math.Floor(r * k) + 1);
                BigInteger t = 2;
                BigInteger p = BigInteger.Pow(t, k - 1);
                BigInteger Q = t * q;
                BigInteger I = p / Q;
                BigInteger S = p % Q;
                bool success = false;
                long B = (long) (c * k * k);

                Sieve(B, out primes);

                while (!success)
                {
                    bool done = false;
                    long[] o = { 1, 1 };
                    long[] z = { 1, 0 };
                    BigInteger J = I + 1;
                    BigInteger K = 2 * I;
                    BigInteger R = hc.RandomRange(J, K);

                    N = 2 * R;
                    N = N * q;
                    N = N + 1;

                    for (int i = 0; !done && i < primes.Count; i++)
                    {
                        BigInteger mod = N % primes[i];

                        done = mod == 0;
                    }

                    if (!done)
                    {
                        if (!hc.Composite(N, 20))
                        {
                            BigInteger a = hc.RandomRange(t, N - t);
                            BigInteger b = BigInteger.ModPow(a, N - 1, N), d = 0;

                            if (b == 1)
                            {
                                b = BigInteger.ModPow(a, 2 * R, N);
                                d = BigInteger.GreatestCommonDivisor(b - 1, N);
                                success = d == 1;
                            }
                        }
                    }
                }
            }

            return N;
        }

        private void Sieve(long B0, out List<long> primes)
        {
            // Sieve of Eratosthenes
            // find all prime numbers
            // less than or equal B0

            bool[] sieve = new bool[B0 + 1];
            long c = 3, i, inc;

            sieve[2] = true;

            for (i = 3; i <= B0; i++)
                if (i % 2 == 1)
                    sieve[i] = true;

            do
            {
                i = c * c;
                inc = c + c;

                while (i <= B0)
                {
                    sieve[i] = false;

                    i += inc;
                }

                c += 2;

                while (!sieve[c])
                    c++;
            } while (c * c <= B0);

            primes = new List<long>();

            for (i = 2; i <= B0; i++)
                if (sieve[i])
                    primes.Add(i);
        }
    }
}
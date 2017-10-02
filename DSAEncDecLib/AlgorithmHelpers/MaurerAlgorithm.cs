namespace DSAEncDecLib.AlgorithmHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Numerics;
    using System.Threading.Tasks;
    using EasySharp.NHelpers.Utils.Cryptography;

    public class MaurerAlgorithm
    {
        private readonly Random _randomNumberGenerator;
        private readonly int _seed;

        private MaurerAlgorithm(int seed)
        {
            _seed = seed;
            _randomNumberGenerator = new Random(seed);
        }

        public static MaurerAlgorithm Instance
        {
            get {
                int seed = RNGUtil.GenerateRandomInt();
                return new MaurerAlgorithm(seed);
            }
        }

        public double Log2(BigInteger n)
        {
            return BigInteger.Log10(n) / Math.Log10(2);
        }

        public async Task<BigInteger> GetProvablePrimeAsync(int bitsSize)
        {
            BigInteger N = 0;
            List<long> primes = null;
            HCSRAlgorithm hc = new HCSRAlgorithm(_seed);
            //Random random = new Random(seed);

            if (bitsSize <= 20)
            {
                bool composite = true;

                while (composite)
                {
                    long n = 1 << (bitsSize - 1);

                    for (int i = 0; i < bitsSize - 1; i++)
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

                if (bitsSize > 2 * m)
                {
                    bool done = false;

                    while (!done)
                    {
                        double s = _randomNumberGenerator.NextDouble();

                        r = Math.Pow(2, s - 1);
                        done = (bitsSize - r * bitsSize) > m;
                    }
                }

                BigInteger q = await GetProvablePrimeAsync((int) Math.Floor(r * bitsSize) + 1).ConfigureAwait(false);
                BigInteger t = 2;
                BigInteger p = BigInteger.Pow(t, bitsSize - 1);
                BigInteger Q = t * q;
                BigInteger I = p / Q; //BigInteger S = p % Q;

                bool success = false;
                long B = (long) (c * bitsSize * bitsSize);

                Sieve(B, out primes);

                while (!success)
                {
                    bool done = false;

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
                            BigInteger b = BigInteger.ModPow(a, N - 1, N);

                            if (b == 1)
                            {
                                b = BigInteger.ModPow(a, 2 * R, N);
                                BigInteger d = BigInteger.GreatestCommonDivisor(b - 1, N);
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
            long c = 3;

            sieve[2] = true;

            for (long i = 3; i <= B0; i++)
            {
                if (i % 2 == 1)
                {
                    sieve[i] = true;
                }
            }

            do
            {
                long j = c * c;
                long inc = c + c;

                while (j <= B0)
                {
                    sieve[j] = false;

                    j += inc;
                }

                c += 2;
                while (!sieve[c]) c++;

            } while (c * c <= B0);

            primes = new List<long>();

            for (long i = 2; i <= B0; i++)
            {
                if (sieve[i])
                {
                    primes.Add(i);
                }
            }
        }
    }
}
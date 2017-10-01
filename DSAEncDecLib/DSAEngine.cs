namespace DSAEncDecLib
{
    using System;
    using System.Numerics;
    using System.Threading.Tasks;
    using AlgorithmHelpers;
    using Interfaces;
    using SpecificTypes;

    class DSAEngine : IKeygen, ISignatureCreator, ISignatureValidator
    {
        public DSAPublicKey PublicKey { get; private set; }

        public DSAPrivateKey PrivateKey { get; private set; }

        public void ImportPublicKey(DSAPublicKey publicKey)
        {
            PublicKey = publicKey;
        }

        public void ImportPrivateKey(DSAPrivateKey privateKey)
        {
            PrivateKey = privateKey;
        }

        public async Task<(DSAPublicKey, DSAPrivateKey)> GenerateKeyPairAsync()
        {
            var (p, q) = await GeneratePQPairAsync().ConfigureAwait(false);
            var alpha = GenerateAlpha(p, q);
            var d = BigIntegerUtil.NextBigInteger(1, q);
            var beta = BigInteger.ModPow(alpha, d, p);

            var dsaPublicKey = new DSAPublicKey(p, q, alpha, beta);
            var dsaPrivateKey = new DSAPrivateKey(d);

            Console.Out.WriteLine("q | p-1 = {0}", (p - 1) % q == 0);

            return (dsaPublicKey, dsaPrivateKey);
        }

        public DSASignature CreateSignature(byte[] hashOfDataToSign)
        {
            BigInteger ephemeralKey = BigIntegerUtil.NextBigInteger(1, PublicKey.Q);

            BigInteger r = BigInteger.ModPow(
                value: BigInteger.ModPow(PublicKey.Alpha, ephemeralKey, PublicKey.P),
                exponent: 1,
                modulus: PublicKey.Q);


            BigInteger ephemeralKeyMultiplicativeInverse =
                ComputeModularMultiplicativeInverse(ephemeralKey, PublicKey.Q);

            var hashNumber = new BigInteger(hashOfDataToSign);
            BigInteger s = BigInteger.ModPow(
                value: (hashNumber + PrivateKey.D * r) * ephemeralKeyMultiplicativeInverse,
                exponent: 1,
                modulus: PublicKey.Q);

            var signature = new DSASignature(r, s);

            return signature;
        }

        public bool VerifySignature(byte[] hashOfSignedData, DSASignature signature)
        {
            BigInteger modularMultiplicativeInversS = ComputeModularMultiplicativeInverse(signature.S, PublicKey.Q);

            BigInteger u1 = ComputeFirstAuxiliaryNumber(hashOfSignedData, signature, modularMultiplicativeInversS);
            BigInteger u2 = ComputeSecondAuxiliaryNumber(signature, modularMultiplicativeInversS);
            BigInteger v = ComputeV(u1, u2);

            return v == signature.R;
        }

        private async Task<(BigInteger p, BigInteger q)> GeneratePQPairAsync()
        {
            Task<(BigInteger, BigInteger)> pqPairFirstCompletedTask =
                await Task.WhenAny(
                        GeneratePQPrimesAsync(),
                        GeneratePQPrimesAsync(),
                        GeneratePQPrimesAsync(),
                        GeneratePQPrimesAsync())
                    .ConfigureAwait(false);

            BigInteger p = pqPairFirstCompletedTask.Result.Item1;
            BigInteger q = pqPairFirstCompletedTask.Result.Item2;

            return (p, q);
        }

        private static Task<(BigInteger, BigInteger)> GeneratePQPrimesAsync()
        {
            BigInteger p = -1;
            BigInteger q = -1;

            bool found = false;
            while (!found)
            {
                q = MaurerAlgorithm.Instance.ProvablePrimeAsync(160).Result;

                for (int i = 0; i < 4096; i++)
                {
                    BigInteger m = BigIntegerUtil.RandomPositiveFixedSizeBigInteger(1024);
                    BigInteger mr = m % (2 * q);
                    p = m - mr + 1;

                    if (p.IsProbablyPrime(10))
                    {
                        Console.Out.WriteLine($"Number of iterations performed: {i + 1}");
                        Console.Out.WriteLine("m = {0}", m);
                        Console.Out.WriteLine($"m len = {m.ToByteArray().Length * 8}");

                        Console.Out.WriteLine("mr = {0}", mr);
                        Console.Out.WriteLine($"mr len = {mr.ToByteArray().Length * 8}");
                        found = true;
                        break;
                    }
                }
            }

            return Task.FromResult((p, q));
        }

        private BigInteger GenerateAlpha(BigInteger p, BigInteger q)
        {
            BigInteger alpha;
            do
            {
                var g = BigIntegerUtil.NextBigInteger(2, p);
                alpha = BigInteger.ModPow(g, (p - 1) / q, p);
            } while (alpha == 1);
            return alpha;
        }

        private BigInteger ComputeV(BigInteger u1, BigInteger u2)
        {
            var leftOperand = BigInteger.ModPow(PublicKey.Alpha, u1, PublicKey.P);
            var rightOperand = BigInteger.ModPow(PublicKey.Beta, u2, PublicKey.P);

            var innerMod = BigInteger.ModPow(leftOperand * rightOperand, 1, PublicKey.P);
            var outerMod = BigInteger.ModPow(innerMod, 1, PublicKey.Q);

            return outerMod;
        }

        private BigInteger ComputeFirstAuxiliaryNumber(byte[] hashOfSignedData, DSASignature signature,
            BigInteger modularMultiplicativeInversS)
        {
            var hash = new BigInteger(hashOfSignedData);

            BigInteger u1 = BigInteger.ModPow(
                value: modularMultiplicativeInversS * hash,
                exponent: 1,
                modulus: PublicKey.Q);

            return u1;
        }

        private BigInteger ComputeSecondAuxiliaryNumber(DSASignature signature, BigInteger modularMultiplicativeInversS)
        {
            BigInteger u2 = BigInteger.ModPow(
                value: modularMultiplicativeInversS * signature.R,
                exponent: 1,
                modulus: PublicKey.Q);

            return u2;
        }


        private BigInteger ComputeModularMultiplicativeInverse(BigInteger number, BigInteger modulus)
        {
            BigInteger modularMultiplicativeInverse = Extended_GCD(modulus, number);

            modularMultiplicativeInverse = NormalizeY(modulus, modularMultiplicativeInverse);

            return modularMultiplicativeInverse;
        }

        private static BigInteger NormalizeY(BigInteger modulus, BigInteger y) => y < 0 ? y + modulus : y;

        private static BigInteger Extended_GCD(BigInteger A, BigInteger B)
        {
            bool reverse = false;

            //if A less than B, switch them
            if (A < B)
            {
                Swap(ref A, ref B);
                reverse = true;
            }

            BigInteger r = B;
            BigInteger q = 0;

            BigInteger x0 = 1;
            BigInteger y0 = 0;

            BigInteger x1 = 0;
            BigInteger y1 = 1;

            BigInteger x = 0;
            BigInteger y = 0;

            while (A % B != 0)
            {
                r = A % B;
                q = A / B;

                x = x0 - q * x1;
                y = y0 - q * y1;

                x0 = x1;
                y0 = y1;

                x1 = x;
                y1 = y;

                A = B;
                B = r;
            }

            var resultR = r;

            return reverse ? x : y;

            void Swap(ref BigInteger X, ref BigInteger Y)
            {
                var temp = X;
                X = Y;
                Y = temp;
            }
        }
    }
}
namespace DSAEncDecLib.Engine
{
    using System;
    using System.Diagnostics;
    using System.Numerics;
    using System.Threading.Tasks;
    using AlgorithmHelpers;
    using Interfaces;
    using SpecificTypes;

    partial class DSAEngine : IKeygen, ISignatureCreator, ISignatureValidator
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

        public async Task<(DSAPublicKey, DSAPrivateKey)> GenerateKeyPairAsync(int keySize)
        {
            Stopwatch overallStopwatch = Stopwatch.StartNew();

            var (p, q) = await GeneratePQPairAsync(keySize).ConfigureAwait(false);
            var alpha = GenerateAlpha(p, q);
            var d = BigIntegerUtil.NextBigInteger(1, q);
            var beta = BigInteger.ModPow(alpha, d, p);

            var dsaPublicKey = new DSAPublicKey(p, q, alpha, beta);
            var dsaPrivateKey = new DSAPrivateKey(d);

            TimeSpan elapsed = overallStopwatch.Elapsed;
            await Console.Out.WriteLineAsync($"Key generation status: operation completed in " +
                                             $"{elapsed.Minutes} min, " +
                                             $"{elapsed.Seconds} sec, " +
                                             $"{elapsed.Milliseconds} ms.").ConfigureAwait(false);

            //Console.Out.WriteLine("q | p-1 = {0}", (p - 1) % q == 0);

            return (dsaPublicKey, dsaPrivateKey);
        }

        public DSASignature CreateSignature(byte[] hashOfDataToSign)
        {
            (BigInteger r, BigInteger s) = default((BigInteger, BigInteger));

            do
            {
                BigInteger ephemeralKey = BigIntegerUtil.NextBigInteger(1, PublicKey.Q);

                r = BigInteger.ModPow(
                    value: BigInteger.ModPow(PublicKey.Alpha, ephemeralKey, PublicKey.P),
                    exponent: 1,
                    modulus: PublicKey.Q);

                BigInteger ephemeralKeyMultiplicativeInverse =
                    ComputeModularMultiplicativeInverse(ephemeralKey, PublicKey.Q);

                var hashNumber = new BigInteger(hashOfDataToSign);

                s = BigInteger.ModPow(
                    value: (hashNumber + PrivateKey.D * r) * ephemeralKeyMultiplicativeInverse,
                    exponent: 1,
                    modulus: PublicKey.Q);

            } while (r == 0 || s == 0);

            var signature = new DSASignature(r, s);

            return signature;
        }

        public bool VerifySignature(byte[] hashOfSignedData, DSASignature signature)
        {
            if (signature.S <= 0 || signature.R <= 0 || signature.R >= PublicKey.Q || signature.S >= PublicKey.Q)
            {
                return false;
            }

            BigInteger modularMultiplicativeInverseS = ComputeModularMultiplicativeInverse(signature.S, PublicKey.Q);

            BigInteger u1 = ComputeFirstAuxiliaryNumber(hashOfSignedData, signature, modularMultiplicativeInverseS);
            BigInteger u2 = ComputeSecondAuxiliaryNumber(signature, modularMultiplicativeInverseS);
            BigInteger v = ComputeV(u1, u2);

            return v == signature.R;
        }
    }
}
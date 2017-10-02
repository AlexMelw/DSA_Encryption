namespace DSAEncDecLib.Engine
{
    using System;
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
    }
}
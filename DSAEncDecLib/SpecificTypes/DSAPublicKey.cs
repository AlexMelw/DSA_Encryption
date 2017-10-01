namespace DSAEncDecLib.SpecificTypes
{
    using System.Numerics;

    public struct DSAPublicKey
    {
        public BigInteger P { get; }
        public BigInteger Q { get; }
        public BigInteger Alpha { get; }
        public BigInteger Beta { get; }

        public DSAPublicKey(BigInteger p, BigInteger q, BigInteger alpha, BigInteger beta)
        {
            P = p;
            Q = q;
            Alpha = alpha;
            Beta = beta;
        }
    }
}
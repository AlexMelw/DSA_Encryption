namespace DSAEncDecLib.SpecificTypes
{
    using System.Numerics;

    public struct DSAPublicKey
    {
        public BigInteger P { get; }
        public BigInteger Q { get; }
        public BigInteger Alpha { get; }
        public BigInteger Beta { get; }

        public DSAPublicKey((BigInteger, BigInteger, BigInteger, BigInteger) key)
        {
            P = key.Item1;
            Q = key.Item2;
            Alpha = key.Item3;
            Beta = key.Item4;
        }
    }
}
namespace DSAEncDecLib.SpecificTypes
{
    using System.Numerics;

    public struct DSASignature
    {
        public BigInteger R { get; }
        public BigInteger S { get; }

        public DSASignature(BigInteger r, BigInteger s)
        {
            R = r;
            S = s;
        }
    }
}
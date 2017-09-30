namespace DSAEncDecLib.SpecificTypes
{
    using System.Numerics;

    public struct DSAPrivateKey
    {
        public BigInteger D { get; set; }

        public DSAPrivateKey(BigInteger key)
        {
            D = key;
        }
    }
}
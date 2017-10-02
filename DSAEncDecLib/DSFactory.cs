namespace DSAEncDecLib
{
    using Engine;
    using Interfaces;

    public static class DSFactory
    {
        public static IKeygen CreateKeygen()
        {
            return new DSAEngine();
        }

        public static ISignatureCreator CreateDigitalSigner()
        {
            return new DSAEngine();
        }


        public static ISignatureValidator CreateSignatureValidator()
        {
            return new DSAEngine();
        }
    }
}
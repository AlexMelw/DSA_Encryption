namespace DSAEncDecLib.Interfaces
{
    using SpecificTypes;

    public interface ISignatureCreator
    {
        void ImportPublicKey(DSAPublicKey publicKey);
        void ImportPrivateKey(DSAPrivateKey privateKey);
        DSASignature CreateSignature(byte[] hashOfDataToSign);
    }
}
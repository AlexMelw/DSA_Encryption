namespace DSAEncDecLib.Interfaces
{
    using SpecificTypes;

    public interface ISignatureValidator
    {
        void ImportPublicKey(DSAPublicKey publicKey);
        bool VerifySignature(byte[] hashOfSignedData, DSASignature signature);
    }
}
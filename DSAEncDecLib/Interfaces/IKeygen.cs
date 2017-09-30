namespace DSAEncDecLib.Interfaces
{
    using System.Threading.Tasks;

    public interface IKeygen
    {
        Task<(byte[] modulus, byte[] encryptionExponent, byte[] decryptionExponent)>
            GenerateKeysAsync(int keySizeBits);
    }
}
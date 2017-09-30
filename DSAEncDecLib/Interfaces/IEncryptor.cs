namespace DSAEncDecLib.Interfaces
{
    public interface IEncryptor
    {
        byte[] EncryptData(byte[] plainText);
        void ImportPublicKey(byte[] encryptionExp, byte[] modulus);
    }
}
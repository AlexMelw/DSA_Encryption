namespace DSAEncDecLib.Interfaces
{
    public interface IDecryptor
    {
        byte[] DecryptData(byte[] cipherText);
        void ImportPrivateKey(byte[] decryptionExp, byte[] modulus);
    }
}
namespace DSAEncDecLib.Interfaces
{
    using System.Threading.Tasks;
    using SpecificTypes;

    public interface IKeygen
    {
        Task<(DSAPublicKey, DSAPrivateKey)> GenerateKeyPairAsync();
    }
}
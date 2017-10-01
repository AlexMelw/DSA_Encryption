namespace DSAcli
{
    using CommandLine;

    static partial class Program
    {
        static void Main(string[] args)
        {
            Parser.Default
                .ParseArguments<GenerateDSAKeyPair, SignDataVerbOptions, VerifyDigitalSignatureVerbOptions>(args)
                .WithParsed<SignDataVerbOptions>(ProcessSignDataCommand)
                .WithParsed<VerifyDigitalSignatureVerbOptions>(ProcessVerifySignatureCommand)
                .WithParsed<GenerateDSAKeyPair>(ProcessGenerateDSAKeyPairCommand);
        }
    }
}
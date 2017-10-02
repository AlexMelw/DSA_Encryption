namespace DSAcli.Facade
{
    using System;
    using System.Threading.Tasks;
    using DSAEncDecLib;
    using DSAEncDecLib.Interfaces;
    using DSAEncDecLib.SpecificTypes;
    using EasySharp.NHelpers.CustomExMethods;
    using Options;

    static partial class Program
    {
        private static void ProcessGenerateDSAKeyPairCommand(GenerateDSAKeyPair options)
        {
            Task.Run(async () =>
            {
                IKeygen keygen = DSFactory.CreateKeygen();

                (DSAPublicKey, DSAPrivateKey) keyPair = await keygen.GenerateKeyPairAsync().ContinueOnCapturedContext();

                string timeStamp = CreateTimeStamp();

                string publicKeyFileName = AggregateFileNameConstituentParts(options, KeyType.Public, timeStamp);
                string privateKeyFileName = AggregateFileNameConstituentParts(options, KeyType.Private, timeStamp);

                PersistPublicKeyToFile(publicKeyFileName, keyPair.Item1);
                PersistPrivateKeyToFile(privateKeyFileName, keyPair.Item2);
            }).Wait();
        }

        private static void ProcessSignDataCommand(SignDataVerbOptions options)
        {
            ISignatureCreator signatureCreator = DSFactory.CreateDigitalSigner();

            byte[] dataHash = ComputeDataHash(options);

            DSAPrivateKey privateKey = LoadPrivateKey(options);
            signatureCreator.ImportPrivateKey(privateKey);

            DSAPublicKey publicKey = LoadPublicKey(options);
            signatureCreator.ImportPublicKey(publicKey);

            DSASignature signature = signatureCreator.CreateSignature(dataHash);

            GenerateOutputFileNameIfNotSet(options);
            PersistDigitalSignatureToFile(options.OutputFilePath, signature);
        }

        private static void ProcessVerifySignatureCommand(VerifyDigitalSignatureVerbOptions options)
        {
            ISignatureValidator signatureValidator = DSFactory.CreateSignatureValidator();

            byte[] dataHash = ComputeDataHash(options);

            DSAPublicKey publicKey = LoadPublicKey(options);
            signatureValidator.ImportPublicKey(publicKey);

            DSASignature signature = LoadDataDigitalSignature(options);

            bool valid = signatureValidator.VerifySignature(dataHash, signature);

            Console.Out.WriteLine(valid
                ? "The signature [ IS ] valid for the given data."
                : "The signature is [ NOT ] valid for the given data.");
        }
    }
}
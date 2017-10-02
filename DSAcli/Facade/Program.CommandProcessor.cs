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
                #region Previous Version

                //IKeygen keygen = DSFactory.CreateKeygen();

                //(byte[] modulus, byte[] encryptionExponent, byte[] decryptionExponent) =
                //    await keygen.GenerateKeysAsync(options.KeyBitLength).ConfigureAwait(true);

                //string timeStamp = CreateTimeStamp();

                //string privateKeyFileName = AggregateFileNameConstituentParts(options, KeyType.Private, timeStamp);
                //string publicKeyFileName = AggregateFileNameConstituentParts(options, KeyType.Public, timeStamp);

                //PersistPublicKeyToFile(publicKeyFileName, encryptionExponent, modulus);
                //PersistPublicKeyToFile(privateKeyFileName, decryptionExponent, modulus);

                #endregion

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
            #region Previous Version

            //byte[] inputByteArray = File.ReadAllBytes(options.InputFilePath);
            //byte[] encryptionExponent;
            //byte[] modulus;

            //using (StreamReader keyStreamReader = File.OpenText(options.KeyPath))
            //{
            //    encryptionExponent = Convert.FromBase64String(keyStreamReader.ReadLine());
            //    modulus = Convert.FromBase64String(keyStreamReader.ReadLine());
            //}

            //IEncryptor rsaEncryptor = DSFactory.CreateEncryptor();
            //rsaEncryptor.ImportPublicKey(encryptionExponent, modulus);
            //byte[] encryptedData = rsaEncryptor.EncryptData(inputByteArray);

            //GenerateOutputFileNameIfNotSet(options);
            //FileStream outputFileStream = File.OpenWrite(options.OutputFilePath);
            //outputFileStream.Write(encryptedData, 0, encryptedData.Length);

            //Console.Out.WriteLine($"The result file is: {Path.GetFileName(options.OutputFilePath)}");

            #endregion

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
            #region Previous Version

            //byte[] inputByteArray = File.ReadAllBytes(options.InputFilePath);
            //byte[] decryptionExponent;
            //byte[] modulus;

            //using (StreamReader keyStreamReader = File.OpenText(options.KeyPath))
            //{
            //    decryptionExponent = Convert.FromBase64String(keyStreamReader.ReadLine());
            //    modulus = Convert.FromBase64String(keyStreamReader.ReadLine());
            //}

            //IDecryptor rsaDecryptor = DSFactory.CreateDecryptor();
            //rsaDecryptor.ImportPrivateKey(decryptionExponent, modulus);
            //byte[] decryptedData = rsaDecryptor.DecryptData(inputByteArray);

            //GenerateOutputFileNameIfNotSet(options);
            //FileStream outputFileStream = File.OpenWrite(options.OutputFilePath);
            //outputFileStream.Write(decryptedData, 0, decryptedData.Length);

            //Console.Out.WriteLine($"The result file is: {Path.GetFileName(options.OutputFilePath)}");

            #endregion

            ISignatureValidator signatureValidator = DSFactory.CreateSignatureValidator();

            byte[] dataHash = ComputeDataHash(options);

            DSAPublicKey publicKey = LoadPublicKey(options);
            signatureValidator.ImportPublicKey(publicKey);

            DSASignature signature = LoadDataDigitalSignature(options);

            bool valid = signatureValidator.VerifySignature(dataHash, signature);

            Console.Out.WriteLine(valid
                ? "The signature is valid for the given data."
                : "The signature is NOT valid for the given data.");
        }
    }
}
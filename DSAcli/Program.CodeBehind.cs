namespace DSAcli
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using DSAEncDecLib;
    using DSAEncDecLib.Interfaces;
    using DSAEncDecLib.SpecificTypes;
    using EasySharp.NHelpers.CustomExMethods;

    static partial class Program
    {
        private static void ProcessGenerateDSAKeyPairCommand(GenerateDSAKeyPair options)
        {
            Task.Run(async () =>
            {
                //IKeygen keygen = DSFactory.CreateKeygen();

                //(byte[] modulus, byte[] encryptionExponent, byte[] decryptionExponent) =
                //    await keygen.GenerateKeysAsync(options.KeyBitLength).ConfigureAwait(true);

                //string timeStamp = CreateTimeStamp();

                //string privateKeyFileName = AggregateFileNameConstituentParts(options, KeyType.Private, timeStamp);
                //string publicKeyFileName = AggregateFileNameConstituentParts(options, KeyType.Public, timeStamp);

                //PersistPublicKeyToFile(publicKeyFileName, encryptionExponent, modulus);
                //PersistPublicKeyToFile(privateKeyFileName, decryptionExponent, modulus);

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
        }

        private static void ProcessVerifySignatureCommand(VerifyDigitalSignatureVerbOptions options)
        {
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
        }

        private static string CreateTimeStamp()
        {
            DateTime now = DateTime.Now;
            string timeStamp = $"{now.Year}-{now.Month}-{now.Day}_{now.Hour}{now.Minute}{now.Second}{now.Millisecond}";
            return timeStamp;
        }

        private static void PersistPublicKeyToFile(string keyFileName, DSAPublicKey key)
        {
            using (StreamWriter keyOutputFileStreamWriter =
                new StreamWriter(File.Open(keyFileName, FileMode.OpenOrCreate, FileAccess.Write)))
            {
                keyOutputFileStreamWriter.WriteLine(Convert.ToBase64String(key.P.ToByteArray()));
                keyOutputFileStreamWriter.WriteLine(Convert.ToBase64String(key.Q.ToByteArray()));
                keyOutputFileStreamWriter.WriteLine(Convert.ToBase64String(key.Alpha.ToByteArray()));
                keyOutputFileStreamWriter.WriteLine(Convert.ToBase64String(key.Beta.ToByteArray()));
            }

            Console.Out.WriteLine($"The result file is: {keyFileName}");
        }

        private static void PersistPrivateKeyToFile(string keyFileName, DSAPrivateKey key)
        {
            using (StreamWriter keyOutputFileStreamWriter =
                new StreamWriter(File.Open(keyFileName, FileMode.OpenOrCreate, FileAccess.Write)))
            {
                keyOutputFileStreamWriter.WriteLine(Convert.ToBase64String(key.D.ToByteArray()));
            }

            Console.Out.WriteLine($"The result file is: {keyFileName}");
        }


        private static void GenerateOutputFileNameIfNotSet(IOutputableOption options)
        {
            if (string.IsNullOrWhiteSpace(options.OutputFilePath))
            {
                string fileExtension = CreateFileExtension(options);
                string filePrefixName = CreateFilePrefixName(options, ref fileExtension);

                options.OutputFilePath = AggregateFileNameConstituentParts(filePrefixName, fileExtension);
            }
        }

        private static string AggregateFileNameConstituentParts(string filePrefixName, string fileExtension)
        {
            DateTime now = DateTime.Now;

            return $"{filePrefixName}_" +
                   $"{now.Year}-{now.Month}-{now.Day}_" +
                   $"{now.Hour}{now.Minute}{now.Second}{now.Millisecond}" +
                   $"{fileExtension}";
        }

        private static string AggregateFileNameConstituentParts(IKeyParams keyParams, KeyType keyType, string timeStamp)
        {
            string extension = keyType.ToString().ToLower();

            var finalFileName = string.IsNullOrWhiteSpace(keyParams.OutputFileNamePrefix)
                ? $"RSA-{keyParams.KeyBitLength}bits_{timeStamp}.{extension}"
                : $"{keyParams.OutputFileNamePrefix}-{keyParams.KeyBitLength}bits.{extension}";

            return finalFileName;
        }


        private static string CreateFileExtension(IOutputableOption options)
        {
            string fileExtension = Path.HasExtension(options.OutputFilePath)
                ? $".{Path.GetExtension(options.OutputFilePath)}"
                : string.Empty;
            return fileExtension;
        }

        private static string CreateFilePrefixName(IOutputableOption options, ref string fileExtension)
        {
            string filePrefixName;

            switch (options)
            {
                case SignDataVerbOptions opts:
                    filePrefixName = "EncryptedData";
                    break;

                case VerifyDigitalSignatureVerbOptions opts:
                    filePrefixName = "DecryptedData";
                    break;

                case GenerateDSAKeyPair opts:
                    filePrefixName = "SK";
                    fileExtension = ".key";
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(options));
            }
            return filePrefixName;
        }
    }

    internal enum KeyType
    {
        Private,
        Public
    }
}
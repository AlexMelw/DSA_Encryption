namespace DSAcli
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using DSAEncDecLib;
    using DSAEncDecLib.Interfaces;

    static partial class Program
    {
        private static void ProcessGenerateRSAKeyPairCommand(GenerateRSAKeyPair options)
        {
            Task.Run(async () =>
            {
                IKeygen keygen = CryptoFactory.CreateKeygen();

                (byte[] modulus, byte[] encryptionExponent, byte[] decryptionExponent) =
                    await keygen.GenerateKeysAsync(options.KeyBitLength).ConfigureAwait(true);

                string timeStamp = CreateTimeStamp();

                string privateKeyFileName = AggregateFileNameConstituentParts(options, KeyType.Private, timeStamp);
                string publicKeyFileName = AggregateFileNameConstituentParts(options, KeyType.Public, timeStamp);

                PersistKeyToFile(publicKeyFileName, encryptionExponent, modulus);
                PersistKeyToFile(privateKeyFileName, decryptionExponent, modulus);
            }).Wait();
        }

        private static void ProcessEncryptCommand(EncryptVerbOptions options)
        {
            byte[] inputByteArray = File.ReadAllBytes(options.InputFilePath);
            byte[] encryptionExponent;
            byte[] modulus;

            using (StreamReader keyStreamReader = File.OpenText(options.KeyPath))
            {
                encryptionExponent = Convert.FromBase64String(keyStreamReader.ReadLine());
                modulus = Convert.FromBase64String(keyStreamReader.ReadLine());
            }

            IEncryptor rsaEncryptor = CryptoFactory.CreateEncryptor();
            rsaEncryptor.ImportPublicKey(encryptionExponent, modulus);
            byte[] encryptedData = rsaEncryptor.EncryptData(inputByteArray);

            GenerateOutputFileNameIfNotSet(options);
            FileStream outputFileStream = File.OpenWrite(options.OutputFilePath);
            outputFileStream.Write(encryptedData, 0, encryptedData.Length);

            Console.Out.WriteLine($"The result file is: {Path.GetFileName(options.OutputFilePath)}");
        }

        private static void ProcessDecryptCommand(DecryptVerbOptions options)
        {
            byte[] inputByteArray = File.ReadAllBytes(options.InputFilePath);
            byte[] decryptionExponent;
            byte[] modulus;

            using (StreamReader keyStreamReader = File.OpenText(options.KeyPath))
            {
                decryptionExponent = Convert.FromBase64String(keyStreamReader.ReadLine());
                modulus = Convert.FromBase64String(keyStreamReader.ReadLine());
            }

            IDecryptor rsaDecryptor = CryptoFactory.CreateDecryptor();
            rsaDecryptor.ImportPrivateKey(decryptionExponent, modulus);
            byte[] decryptedData = rsaDecryptor.DecryptData(inputByteArray);

            GenerateOutputFileNameIfNotSet(options);
            FileStream outputFileStream = File.OpenWrite(options.OutputFilePath);
            outputFileStream.Write(decryptedData, 0, decryptedData.Length);

            Console.Out.WriteLine($"The result file is: {Path.GetFileName(options.OutputFilePath)}");
        }

        private static string CreateTimeStamp()
        {
            DateTime now = DateTime.Now;
            string timeStamp = $"{now.Year}-{now.Month}-{now.Day}_{now.Hour}{now.Minute}{now.Second}{now.Millisecond}";
            return timeStamp;
        }

        private static void PersistKeyToFile(string keyFileName, byte[] exponent, byte[] modulus)
        {
            using (StreamWriter keyOutputFileStreamWriter =
                new StreamWriter(File.Open(keyFileName, FileMode.OpenOrCreate, FileAccess.Write)))
            {
                keyOutputFileStreamWriter.WriteLine(Convert.ToBase64String(exponent));
                keyOutputFileStreamWriter.WriteLine(Convert.ToBase64String(modulus));
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
                case EncryptVerbOptions opts:
                    filePrefixName = "EncryptedData";
                    break;

                case DecryptVerbOptions opts:
                    filePrefixName = "DecryptedData";
                    break;

                case GenerateRSAKeyPair opts:
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
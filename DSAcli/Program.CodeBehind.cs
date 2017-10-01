namespace DSAcli
{
    using System;
    using System.IO;
    using System.Net.Mime;
    using System.Numerics;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Serialization;
    using DSAEncDecLib;
    using DSAEncDecLib.Interfaces;
    using DSAEncDecLib.SpecificTypes;
    using EasySharp.NHelpers.CustomExMethods;
    using Interfaces;

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
                ? $"The signature is valid for the given data."
                : $"The signature is NOT valid for the given data.");
        }

        private static DSASignature LoadDataDigitalSignature(IDigitalSignaturePath option)
        {
            using (StreamReader keyStreamReader = File.OpenText(option.DigitalSignaturePath))
            {
                try
                {
                    BigInteger r = new BigInteger(Convert.FromBase64String(keyStreamReader.ReadLine()));
                    BigInteger s = new BigInteger(Convert.FromBase64String(keyStreamReader.ReadLine()));

                    return new DSASignature(r, s);
                }
                catch (IOException e)
                {
                    Console.Out.WriteLine($"Error on reading file {option.DigitalSignaturePath}");
                    Environment.Exit(1);
                    return default(DSASignature);
                }
            }
        }

        private static DSAPublicKey LoadPublicKey(IPublicKeyOption option)
        {
            using (StreamReader keyStreamReader = File.OpenText(option.PublicKeyPath))
            {
                try
                {
                    BigInteger p = new BigInteger(Convert.FromBase64String(keyStreamReader.ReadLine()));
                    BigInteger q = new BigInteger(Convert.FromBase64String(keyStreamReader.ReadLine()));
                    BigInteger alpha = new BigInteger(Convert.FromBase64String(keyStreamReader.ReadLine()));
                    BigInteger beta = new BigInteger(Convert.FromBase64String(keyStreamReader.ReadLine()));

                    return new DSAPublicKey(p, q, alpha, beta);
                }
                catch (IOException e)
                {
                    Console.Out.WriteLine($"Error on reading file {option.PublicKeyPath}");
                    Environment.Exit(1);
                    return default(DSAPublicKey);
                }
            }
        }

        private static DSAPrivateKey LoadPrivateKey(IPrivateKeyOption option)
        {
            using (StreamReader keyStreamReader = File.OpenText(option.PrivateKeyPath))
            {
                try
                {
                    BigInteger d = new BigInteger(Convert.FromBase64String(keyStreamReader.ReadLine()));

                    return new DSAPrivateKey(d);
                }
                catch (IOException e)
                {
                    Console.Out.WriteLine($"Error on reading file {option.PrivateKeyPath}");
                    Environment.Exit(1);
                    return default(DSAPrivateKey);
                }
            }
        }

        private static byte[] ComputeDataHash(IImputableOption option)
        {
            byte[] dataHash;
            using (var sha512 = SHA512.Create())
            {
                byte[] inputByteArray = File.ReadAllBytes(option.InputFilePath);
                dataHash = sha512.ComputeHash(inputByteArray);
            }
            return dataHash;
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

        private static void PersistDigitalSignatureToFile(string signatureFileName, DSASignature signature)
        {
            using (StreamWriter keyOutputFileStreamWriter =
                new StreamWriter(File.Open(signatureFileName, FileMode.OpenOrCreate, FileAccess.Write)))
            {
                keyOutputFileStreamWriter.WriteLine(Convert.ToBase64String(signature.R.ToByteArray()));
                keyOutputFileStreamWriter.WriteLine(Convert.ToBase64String(signature.S.ToByteArray()));
            }

            Console.Out.WriteLine($"The result file is: {signatureFileName}");
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

        private static string CreateTimeStamp()
        {
            DateTime now = DateTime.Now;
            string timeStamp = $"{now.Year}-{now.Month}-{now.Day}_{now.Hour}{now.Minute}{now.Second}{now.Millisecond}";
            return timeStamp;
        }
    }

    internal enum KeyType
    {
        Private,
        Public
    }
}
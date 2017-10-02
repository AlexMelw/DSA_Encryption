namespace DSAcli.Facade
{
    using System;
    using System.IO;
    using System.Numerics;
    using System.Security.Cryptography;
    using DSAEncDecLib.SpecificTypes;
    using EasySharp.NHelpers.CustomExMethods;
    using Enums;
    using Interfaces;
    using Options;

    static partial class Program
    {
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
                catch (IOException)
                {
                    Console.Out.WriteLine($"Error on reading file {option.DigitalSignaturePath}");
                    Environment.Exit(1);
                    return default;
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
                catch (IOException)
                {
                    Console.Out.WriteLine($"Error on reading file {option.PublicKeyPath}");
                    Environment.Exit(1);
                    return default;
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
                catch (IOException)
                {
                    Console.Out.WriteLine($"Error on reading file {option.PrivateKeyPath}");
                    Environment.Exit(1);
                    return default(DSAPrivateKey);
                }
            }
        }

        private static byte[] ComputeDataHash(IImputableOption option)
        {
            byte[] positiveDataHash;
            using (var sha512 = SHA512.Create())
            {
                byte[] inputByteArray = File.ReadAllBytes(option.InputFilePath);
                byte[] realPosibleNegativeHash = sha512.ComputeHash(inputByteArray);

                var hashSumInteger = new BigInteger(realPosibleNegativeHash);

                positiveDataHash = hashSumInteger.Sign < 0
                    ? hashSumPositiveInteger(hashSumInteger).ToByteArray()
                    : hashSumInteger.ToByteArray();
            }

            return positiveDataHash;

            BigInteger hashSumPositiveInteger(BigInteger integerHashSum) => BigInteger.Abs(integerHashSum) * 2 - 1;
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

        private static string AggregateFileNameConstituentParts(IKeyParams keyParams, KeyType keyType, string timeStamp)
        {
            string extension = keyType.ToString().ToLower();

            var finalFileName = string.IsNullOrWhiteSpace(keyParams.OutputFileNamePrefix)
                ? $"DSA-{keyParams.KeyBitLength}bits_{timeStamp}.{extension}"
                : $"{keyParams.OutputFileNamePrefix}-{keyParams.KeyBitLength}bits.{extension}";

            return finalFileName;
        }

        private static string CreateTimeStamp()
        {
            DateTime now = DateTime.Now;
            string timeStamp = $"{now.Year}-{now.Month}-{now.Day}_{now.Hour}{now.Minute}{now.Second}{now.Millisecond}";
            return timeStamp;
        }

        private static void GenerateOutputFileNameIfNotSet(SignDataVerbOptions option)
        {
            if (string.IsNullOrWhiteSpace(option.OutputFilePath))
            {
                DateTime now = DateTime.Now;

                option.OutputFilePath = $"{Path.GetFileName(option.InputFilePath)}_DigitalSignature_" +
                                        $"{now.Year}-{now.Month}-{now.Day}_" +
                                        $"{now.Hour}{now.Minute}{now.Second}{now.Millisecond}" +
                                        ".sgn";
            }
        }

        private static void CheckKeySizeOption(GenerateDSAKeyPair option)
        {
            if (!option.KeyBitLength.In(new[] { 1024, 2048, 3072 }))
            {
                Console.Out.WriteLine("Wrong key size selected. The only allowed values are: [1024, 2048, 3072].");
                Environment.Exit(1);
            }
        }
    }
}
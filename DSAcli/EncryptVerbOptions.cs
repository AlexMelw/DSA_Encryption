namespace DSAcli
{
    using System.Collections.Generic;
    using CommandLine;
    using CommandLine.Text;

    [Verb("enc", HelpText = "Enforces file encryption with the specified key RSA public.")]
    class EncryptVerbOptions : IOutputableOption
    {
        [Option('i', "input", Required = true,
            HelpText = "Source file containing the data to be encrypted with RAS public key.")]
        public string InputFilePath { get; set; }

        [Option('k', "key", Required = true,
            HelpText = "Path to file containing RSA private key for encryption of data from supplied file.")]
        public string KeyPath { get; set; }

        [Option('o', "output",
            HelpText = "Output File Name. This file will contain the result of the decryption operation.")]
        public string OutputFilePath { get; set; }

        [Usage(ApplicationAlias = "RSAcli")]
        public static IEnumerable<Example> Examples
        {
            get {
                yield return new Example("Encryption", new EncryptVerbOptions
                {
                    KeyPath = "Secret-4096bits.public",
                    InputFilePath = "ToBeEncrypted.ext",
                    OutputFilePath = "Encrypted.ext"
                });
            }
        }
    }
}
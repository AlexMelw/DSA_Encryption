namespace DSAcli.Options
{
    using System.Collections.Generic;
    using CommandLine;
    using CommandLine.Text;
    using Interfaces;

    [Verb("enc", HelpText = "Enforces data digital signing with the specified DSA key-pair.")]
    class SignDataVerbOptions : IOutputableOption, IImputableOption, IPublicKeyOption, IPrivateKeyOption
    {
        [Option('i', "input", Required = true,
            HelpText = "Source file containing the data to be signed using DSA.")]
        public string InputFilePath { get; set; }

        [Option('p', "public-key", Required = true,
            HelpText = "Path to file containing DSA public key for signing data contained in supplied file.")]
        public string PublicKeyPath { get; set; }

        [Option('s', "private-key", Required = true,
            HelpText = "Path to file containing DSA private key for signing data contained in supplied file.")]
        public string PrivateKeyPath { get; set; }

        [Option('o', "output",
            HelpText = "Output Signature FileName. This file will contain the computed data signature.")]
        public string OutputFilePath { get; set; }

        [Usage(ApplicationAlias = "DSAcli")]
        public static IEnumerable<Example> Examples
        {
            get {
                yield return new Example("Signing Data", new SignDataVerbOptions
                {
                    InputFilePath = "FileToBeSigned.ext",
                    PrivateKeyPath = "DSA-2048bits_2017-10-1_143636508.private",
                    PublicKeyPath = "DSA-2048bits_2017-10-1_143636508.public",
                    OutputFilePath = "FileToBeSigned_Signature.sgn"
                });
            }
        }
    }
}
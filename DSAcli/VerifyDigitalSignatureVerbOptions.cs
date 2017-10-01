namespace DSAcli
{
    using System.Collections.Generic;
    using System.Threading;
    using CommandLine;
    using CommandLine.Text;
    using Interfaces;

    [Verb("dec", HelpText = "Enforces verification of data digital signature using DSA public key.")]
    class VerifyDigitalSignatureVerbOptions : IImputableOption, IPublicKeyOption, IDigitalSignaturePath
    {
        [Option('i', "input", Required = true,
            HelpText = "Source file containing the data that was signed using DSA.")]
        public string InputFilePath { get; set; }

        [Option('p', "public-key", Required = true,
            HelpText = "Path to file containing DSA public key for digital signature verification.")]
        public string PublicKeyPath { get; set; }


        [Option('s', "signature", Required = true,
            HelpText = "Path to file containing the digital signature of the given data.")]
        public string DigitalSignaturePath { get; set; }

        [Usage(ApplicationAlias = "DSAcli")]
        public static IEnumerable<Example> Examples
        {
            get {
                yield return new Example("Digital Signature Verification", new VerifyDigitalSignatureVerbOptions
                {
                    InputFilePath = "FileThatWasSigned.ext",
                    PublicKeyPath = "DSA-2048bits_2017-10-1_143636508.private",
                    DigitalSignaturePath = "FileThatWasSigned_Signature.sgn"
                });
            }
        }
    }
}
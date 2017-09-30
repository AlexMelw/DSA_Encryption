namespace DSAcli
{
    using System.Collections.Generic;
    using CommandLine;
    using CommandLine.Text;

    [Verb("keygen", HelpText = "Generates RSA public/private key-pair of the specified bit-length.")]
    class GenerateRSAKeyPair : IKeyParams
    {
        [Option('s', "size",
            HelpText = "Recommended values: 1024, 2048, 4096. " +
                       "Specifies bit-length (integer value) of the RSA public/private keys.",
            Default = 2048)]
        public int KeyBitLength { get; set; }

        [Option('p', "prefix",
            HelpText = "The Prefix of Output Filenames. Example of generated filenames: " +
                       "{Prefix}-{4096}bits.public {Prefix}-{4096}bits.private")]
        public string OutputFileNamePrefix { get; set; }

        [Usage(ApplicationAlias = "RSAcli")]
        public static IEnumerable<Example> Examples
        {
            get {
                yield return new Example("RSA Key-Pair Generation", new GenerateRSAKeyPair
                {
                    OutputFileNamePrefix = @"Your Key Name Prefix"
                });
            }
        }
    }

    internal interface IKeyParams
    {
        int KeyBitLength { get; set; }
        string OutputFileNamePrefix { get; set; }
    }
}
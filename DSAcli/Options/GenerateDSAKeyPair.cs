namespace DSAcli.Options
{
    using System.Collections.Generic;
    using CommandLine;
    using CommandLine.Text;
    using Interfaces;

    [Verb("keygen", HelpText = "Generates DSA public/private key-pair of the specified bit-length.")]
    class GenerateDSAKeyPair : IKeyParams
    {
        [Option('s', "size",
            HelpText =
                "Specifies bit-length (integer value) of the DSA [p] factor " +
                "(the [q] length will be automatically adjusted). " +
                "Both [p] and [q] are part of DSA public key. " +
                "Allowed values: 1024, 2048, 3072. Recommended value: 1024. " +
                "For [p]=1024, [q]=160; " +
                "For [p]=2048, [q]=224; " +
                "For [p]=3072, [q]=256.",
            Default = 1024)]
        public int KeyBitLength { get; set; }

        [Option('p', "prefix",
            HelpText = "The Prefix of Output Filenames. Example of generated filenames: " +
                       "{Prefix}-{1024}bits.public {Prefix}-{1024}bits.private")]
        public string OutputFileNamePrefix { get; set; }

        [Usage(ApplicationAlias = "DSAcli")]
        public static IEnumerable<Example> Examples
        {
            get {
                yield return new Example("DSA Key-Pair Generation", new GenerateDSAKeyPair
                {
                    OutputFileNamePrefix = @"Your Key Name Prefix",
                    KeyBitLength = 1024
                });
            }
        }
    }
}
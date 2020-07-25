# DSA (Digital Signature Algorithm) Utility (CLI)

Implemented in C# (.NET Framework 4.6.1) adhering to the Federal Information Processing Standard for digital signatures algorithm.

### Documentation

To get some help concerning the use of CLI utility, type:

`cmd> DSAcli --help` or just `cmd> DSAcli help`

**The output:**

```
DSA CLI Utility 1.2.0.23755
Copyright (C) 2017. Developed by BARBARII Veaceslav

  keygen     Generates DSA public/private key-pair of the specified bit-length.

  sign       Enforces digital signing of data with the specified DSA key-pair.

  ver        Enforces verification of digital signature of data using DSA public key.

  help       Display more information on a specific command.

  version    Display version information.
```
  
For more information related to each command verb, type:

`cmd> DSAcli help verb`, wherein verb can be either `keygen`, `enc` or `dec`.

**The output:**

```
DSA CLI Utility 1.2.0.23755
Copyright (C) 2017. Developed by BARBARII Veaceslav
USAGE:
DSA Key-Pair Generation:
  DSAcli keygen --prefix "Your Key Name Prefix" --size 1024

  -s, --size      (Default: 1024) Specifies bit-length (integer value) of the DSA [p] factor (the [q] length will be automatically adjusted). Both [p]
                  and [q] are part of DSA public key. Allowed values: 1024, 2048, 3072. Recommended value: 1024. For [p]=1024, [q]=160; For [p]=2048,
                  [q]=224; For [p]=3072, [q]=256.

  -p, --prefix    The Prefix of Output Filenames. Example of generated filenames: {Prefix}-{1024}bits.public {Prefix}-{1024}bits.private

  --help          Display this help screen.

  --version       Display version information.
```

namespace DSAEncDecLib.AlgorithmHelpers
{
    using System;
    using System.Globalization;
    using System.Numerics;
    using EasySharp.NHelpers.Utils.Cryptography;

    public class RsaHelper
    {
        public static void LcmGcdDemo()
        {
            BigInteger a = 12;
            BigInteger b = 30;

            var GCD = BigInteger.GreatestCommonDivisor(a, b);

            BigInteger LCM = a * b / GCD;

            Console.Out.WriteLine("GCD = {0}", GCD);
            Console.Out.WriteLine("LCM = {0}", LCM);
        }

        public static void Demo()
        {
            BigInteger bi_e =
                BigInteger.Parse(
                    "0a932b948feed4fb2b692609bd22164fc9edb59fae7880cc1eaff7b3c9626b7e5b241c27a974833b2622ebe09beb451917663d47232488f23a117fc97720f1e7",
                    NumberStyles.HexNumber);

            BigInteger bi_d = BigInteger.Parse(
                "04adf2f7a89da93248509347d2ae506d683dd3a16357e859a980c4f77a4e2f7a01fae289f13a851df6e9db5adaa60bfd2b162bbbe31f7c8f828261a6839311929d2cef4f864dde65e556ce43c89bbbf9f1ac5511315847ce9cc8dc92470a747b8792d6a83b0092d2e5ebaf852c85cacf34278efa99160f2f8aa7ee7214de07b7",
                NumberStyles.HexNumber);

            BigInteger bi_n = BigInteger.Parse(
                "0e8e77781f36a7b3188d711c2190b560f205a52391b3479cdb99fa010745cbeba5f2adc08e1de6bf38398a0487c4a73610d94ec36f17f3f46ad75e17bc1adfec99839589f45f95ccc94cb2a5c500b477eb3323d8cfab0c8458c96f0147a45d27e45a4d11d54d77684f65d48f15fafcc1ba208e71e921b9bd9017c16a5231af7f",
                NumberStyles.HexNumber);

            Console.Out.WriteLine("bi_e = {0}", bi_e);
            Console.Out.WriteLine("bi_d = {0}", bi_d);
            Console.Out.WriteLine("bi_n = {0}", bi_n);

            var byteArray = bi_e.ToByteArray();
            Console.Out.WriteLine($"SIZE: {byteArray.Length * 8}");


            // data
            BigInteger bi_data = BigInteger.Parse("123456789");

            // encrypt and decrypt data
            BigInteger bi_encrypted = BigInteger.ModPow(bi_data, bi_e, bi_n);
            BigInteger bi_decrypted = BigInteger.ModPow(bi_encrypted, bi_d, bi_n);

            Console.WriteLine("bi_data = " + bi_data);
            Console.WriteLine("\nbi_encrypted =\n" + bi_encrypted);
            Console.WriteLine("\nbi_decrypted = " + bi_decrypted);
        }

        public static BigInteger CustomGCM(BigInteger x, BigInteger y)
        {
            while (x != y)
            {
                if (x > y)
                    x -= y;
                else
                    y -= x;
            }

            return x;
        }

        public static (int p, int q) GetRandomPrimesWithinRange(int start, int end)
        {
            int p = 0;
            int q = 0;

            while (p == q)
            {
                p = GetRandomPrimeWithinRange(start, end);
                q = GetRandomPrimeWithinRange(start, end);
            }

            return (p, q);
        }

        public static int GetRandomPrimeWithinRange(int start, int end)
        {
            if (!ExistsPrimeWithinRange(start, end))
                throw new ArgumentException($"There is no prime numbers for the given range: {start}..{end}");


            int value = RNGUtil.Next(start, end);

            int oscillator = value;
            bool flipFlop = true;
            int seedOffset = 1;

            while (IsNotPrime(oscillator) || !(oscillator >= start && oscillator <= end))
            {
                flipFlop = !flipFlop; // "false" for the first time
                oscillator = value + (flipFlop ? -seedOffset : +seedOffset);
                if (flipFlop) seedOffset += 1;
            }

            return oscillator;
        }

        public static bool ExistsPrimeWithinRange(int start, int end)
        {
            for (int i = start; i < end; i++)
            {
                if (IsPrime(i))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsNotPrime(int number) => !IsPrime(number);

        public static bool IsPrime(int number)
        {
            if (number % 2 == 0)
            {
                return number == 2;
            }

            int limit = (int) Math.Sqrt(number);

            for (int i = 3; i <= limit; i += 2)
            {
                if (number % i == 0)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
namespace DSAEncDecLibTests
{
    using System.Globalization;
    using System.Numerics;
    using System.Text;
    using System.Threading.Tasks;
    using DSAEncDecLib;
    using DSAEncDecLib.Interfaces;
    using NUnit.Framework;

    //[TestFixture]
    //public class DSAEngineTests
    //{
    //    [Test]
    //    [TestCaseSource(nameof(EncryptionDataSourceObjects))]
    //    public void
    //        GivenDataAndEncryptionExpAndModulus_WhenEncryptingWithPreviouslyGeneratedKeys_ThanEqualsToExpectedResult(
    //            BigInteger expected, byte[] m, BigInteger e, BigInteger n)
    //    {
    //        // Arrange
    //        RSAEngine rsaEngine = new RSAEngine();
    //        rsaEngine.ImportPublicKey(e, n);

    //        // Act               
    //        byte[] encryptedRsaParams = rsaEngine.EncryptData(m);
    //        //byte[] encryptedRsaParams = rsaEngine.EncryptData(new BigInteger(2).ToByteArray());

    //        // Assert
    //        Assert.That(new BigInteger(encryptedRsaParams), Is.EqualTo(expected));
    //        //Assert.That(new BigInteger(encryptedRsaParams), Is.EqualTo(new BigInteger(4)));
    //    }

    //    [Test]
    //    [TestCaseSource(nameof(DecryptionDataSourceObjects))]
    //    public void
    //        GivenDataAndDecryptionExpAndModulus_WhenDecryptingWithPreviouslyGeneratedKeys_ThanEqualsToExpectedResult(
    //            BigInteger expected, byte[] m, BigInteger d, BigInteger n)
    //    {
    //        // Arrange
    //        RSAEngine rsaEngine = new RSAEngine();
    //        rsaEngine.ImportPrivateKey(d, n);

    //        // Act               
    //        byte[] decryptedRsaParams = rsaEngine.DecryptData(m);
    //        //byte[] decryptedRsaParams = rsaEngine.DecryptData(new BigInteger(4).ToByteArray());

    //        // Assert
    //        Assert.That(new BigInteger(decryptedRsaParams), Is.EqualTo(expected));
    //        //Assert.That(new BigInteger(decryptedRsaParams), Is.EqualTo(new BigInteger(2)));
    //    }

    //    [TestCase(1024)]
    //    [TestCase(2048)]
    //    [TestCase(4096)]
    //    public void GivenGeneratedKeysWithinCurrentSession_WhenEncryptAndDecryptData_ThenOriginalAndResultAreEqual(
    //        int keyBitsSize)
    //    {
    //        // Arrange
    //        string original = "Sunt din Chișinău. Знаю русский язык. " +
    //                          "Utilizez diacriticele: â,î,ă,ț,ș,Â,Î,Ă,Ț,Ș. " +
    //                          "Test din 22.09.2017";

    //        IKeygen keygen = DSFactory.CreateKeygen();

    //        var result = Task.Run(async () => await keygen.GenerateKeysAsync(keyBitsSize).ConfigureAwait(true)).Result;

    //        (byte[] modulus, byte[] encryptionExponent, byte[] decryptionExponent) =
    //            (result.Item1, result.Item2, result.Item3);

    //        // Act
    //        IEncryptor encryptor = DSFactory.CreateEncryptor();
    //        encryptor.ImportPublicKey(encryptionExponent, modulus);
    //        byte[] encryptedData = encryptor.EncryptData(Encoding.UTF8.GetBytes(original));


    //        IDecryptor decryptor = DSFactory.CreateDecryptor();
    //        decryptor.ImportPrivateKey(decryptionExponent, modulus);
    //        byte[] decryptedData = decryptor.DecryptData(encryptedData);

    //        // Assert
    //        string actual = Encoding.UTF8.GetString(decryptedData);

    //        Assert.That(Encoding.UTF8.GetBytes(actual).Length, Is.EqualTo(Encoding.UTF8.GetBytes(original).Length));
    //        Assert.That(actual, Is.EqualTo(original));
    //    }

    //    private static object[] EncryptionDataSourceObjects =
    //    {
    //        new object[]
    //        {
    //            new BigInteger(4),
    //            new BigInteger(2).ToByteArray(),
    //            new BigInteger(5),
    //            new BigInteger(14)
    //        },

    //        #region Big Numers

    //        new object[]
    //        {
    //            BigInteger.Parse(
    //                "4733996332846476389043559376011325558915104994148704345675630823414143044794894977703174861102469590360418219598653034945753208045098410889709544681323823286242592397229847620474170924648174378438085814015517632300727190751938168799431059241728878026403893663387155417887792550570843768501275146253893503588"),
    //            BigInteger.Parse(
    //                    "123456789")
    //                .ToByteArray(),
    //            BigInteger.Parse(
    //                "0a932b948feed4fb2b692609bd22164fc9edb59fae7880cc1eaff7b3c9626b7e5b241c27a974833b2622ebe09beb451917663d47232488f23a117fc97720f1e7",
    //                NumberStyles.HexNumber), // e
    //            BigInteger.Parse(
    //                "0e8e77781f36a7b3188d711c2190b560f205a52391b3479cdb99fa010745cbeba5f2adc08e1de6bf38398a0487c4a73610d94ec36f17f3f46ad75e17bc1adfec99839589f45f95ccc94cb2a5c500b477eb3323d8cfab0c8458c96f0147a45d27e45a4d11d54d77684f65d48f15fafcc1ba208e71e921b9bd9017c16a5231af7f",
    //                NumberStyles.HexNumber) // n
    //        }

    //        #endregion
    //    };

    //    private static object[] DecryptionDataSourceObjects =
    //    {
    //        new object[]
    //        {
    //            new BigInteger(2),
    //            new BigInteger(4).ToByteArray(),
    //            new BigInteger(11),
    //            new BigInteger(14)
    //        },

    //        #region Big Numbers

    //        new object[]
    //        {
    //            BigInteger.Parse(
    //                "123456789"),
    //            BigInteger.Parse(
    //                    "4733996332846476389043559376011325558915104994148704345675630823414143044794894977703174861102469590360418219598653034945753208045098410889709544681323823286242592397229847620474170924648174378438085814015517632300727190751938168799431059241728878026403893663387155417887792550570843768501275146253893503588")
    //                .ToByteArray(),
    //            BigInteger.Parse(
    //                "04adf2f7a89da93248509347d2ae506d683dd3a16357e859a980c4f77a4e2f7a01fae289f13a851df6e9db5adaa60bfd2b162bbbe31f7c8f828261a6839311929d2cef4f864dde65e556ce43c89bbbf9f1ac5511315847ce9cc8dc92470a747b8792d6a83b0092d2e5ebaf852c85cacf34278efa99160f2f8aa7ee7214de07b7",
    //                NumberStyles.HexNumber), // d
    //            BigInteger.Parse(
    //                "0e8e77781f36a7b3188d711c2190b560f205a52391b3479cdb99fa010745cbeba5f2adc08e1de6bf38398a0487c4a73610d94ec36f17f3f46ad75e17bc1adfec99839589f45f95ccc94cb2a5c500b477eb3323d8cfab0c8458c96f0147a45d27e45a4d11d54d77684f65d48f15fafcc1ba208e71e921b9bd9017c16a5231af7f",
    //                NumberStyles.HexNumber) // n
    //        }

    //        #endregion
    //    };
    //}
}
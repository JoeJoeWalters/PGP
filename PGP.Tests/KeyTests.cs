using AwesomeAssertions;
using PgpCore;
using System.Threading.Tasks;

namespace PGPLibrary.Tests
{
    public class KeyTests
    {
        private const string _fileToEncryptName = "filetoencrypt.txt";
        private const string _encryptedName = "encrypted.pgp";
        private const string _decryptedName = "decrypted.txt";

        [Fact]
        public async Task When_EncryptWithMultipleKeys_Then_DecryptWithBoth()
        {
            // ARRANGE
            if (!File.Exists("FileToEncrypt.txt"))
                throw new FileNotFoundException("FileToEncrypt.txt not found in the test directory.");

            string expectedContent = File.ReadAllText(_fileToEncryptName);

            FileInfo publicKey1 = new FileInfo(@"keys\keypair1_public.asc");
            FileInfo publicKey2 = new FileInfo(@"keys\keypair2_public.asc");
            EncryptionKeys encryptionKeys = new EncryptionKeys(new List<FileInfo>() { publicKey1, publicKey2 });
            FileInfo inputFile = new FileInfo(_fileToEncryptName);
            FileInfo outputFile = new FileInfo(_encryptedName);
            PGP encryptPGP = new PGP(encryptionKeys);
            string encryptedContent = string.Empty;

            FileInfo privateKey1 = new FileInfo(@"keys\keypair1_private.asc");
            EncryptionKeys decryptionKeys1 = new EncryptionKeys(privateKey1, "KeyPair1");
            PGP decrypt1PGP = new PGP(decryptionKeys1);
            string decryptedContent1 = string.Empty;
            FileInfo privateKey2 = new FileInfo(@"keys\keypair1_private.asc");
            EncryptionKeys decryptionKeys2 = new EncryptionKeys(privateKey2, "KeyPair2");
            PGP decrypt2PGP = new PGP(decryptionKeys1);
            string decryptedContent2 = string.Empty;
            FileInfo encryptedFile = new FileInfo(_encryptedName);
            FileInfo decryptedFile = new FileInfo(_decryptedName);

            // ACT
            File.Delete(_encryptedName);
            await encryptPGP.EncryptFileAsync(inputFile, outputFile);
            if (File.Exists(_encryptedName))
                encryptedContent = File.ReadAllText(_encryptedName);

            File.Delete(_decryptedName);
            await decrypt1PGP.DecryptFileAsync(encryptedFile, decryptedFile);
            if (File.Exists(_decryptedName))
                decryptedContent1 = File.ReadAllText(_decryptedName);

            File.Delete(_decryptedName);
            await decrypt2PGP.DecryptFileAsync(encryptedFile, decryptedFile);
            if (File.Exists(_decryptedName))
                decryptedContent2 = File.ReadAllText(_decryptedName);

            // ASSERT
            encryptedContent.Should().StartWith("-----BEGIN PGP MESSAGE-----");
            decryptedContent1.Should().Be(expectedContent);
            decryptedContent2.Should().Be(expectedContent);
        }
    }
}
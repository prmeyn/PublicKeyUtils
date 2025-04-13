using PublicKeyUtils.CryptoKeys;

namespace PublicKeyUtils.Tests.CryptoKeys
{
	public class EncryptDecryptPublicKeyTests
	{
		[Fact]
		public void ModulusAndExponent_ShouldDecodeCorrectly()
		{
			var key = new EncryptDecryptPublicKey
			{
				N = "AQAB", // base64url for 65537
				E = "AQAB"
			};

			var modulus = key.Modulus;
			var exponent = key.Exponent;

			Assert.NotNull(modulus);
			Assert.NotNull(exponent);
			Assert.Equal([1, 0, 1], exponent); // 65537
		}

		[Theory]
		[InlineData("RSA-OAEP")]
		[InlineData("RSA-OAEP-256")]
		[InlineData("RSA-OAEP-384")]
		[InlineData("RSA-OAEP-512")]
		public void Encrypt_ShouldReturnNonEmptyCiphertext_ForValidInput(string algorithm)
		{
			// Public exponent and modulus for 2048-bit key (just a sample — not secure)
			var key = new EncryptDecryptPublicKey
			{
				Algorithm = algorithm,
				E = "AQAB", // 65537
				N = "sXch9W6_K8oZn3PfJPZepKvXYTwc_nPIu9JrYfPRzM8zqVnD3edGldHTebAiN4MbcDkN5q1nbRV69BQ1_lwPt6b92_l6dcH0QJebQqovExY16Y7bQO02NGqjc8tkFPAeqC1cgI2VmojzG3FeAWqxtj5Ez5g0PYYJgxIoEXRopv9N2V-DME4mXwMxf3NVZ9d73Sm1Tb9p_U1OwQuWCh0p4kJHDsh44yBdM37KMLWSLM6pEr7jeWyzX0d1sKdfbORaVq0f1uzjZ_3iM_Oey7GMJKkPGYQlQjWbL2iyHv5PeAxJmZLykB0CZ0oUzOGlYfKJhL1x_j1D_zFckvj7o0K9GQ", // fake RSA 2048 modulus
				KeyOps = ["encrypt"]
			};

			var plaintext = "Hello world!";
			var ciphertext = key.Encrypt(plaintext);

			Assert.NotNull(ciphertext);
			Assert.NotEmpty(ciphertext);
		}

		[Fact]
		public void Encrypt_ShouldReturnEmpty_WhenKeyOpsDoesNotAllowEncryption()
		{
			var key = new EncryptDecryptPublicKey
			{
				Algorithm = "RSA-OAEP",
				E = "AQAB",
				N = "sXch9W6_K8oZn3PfJPZepKvXYTwc_nPIu9JrYfPRzM8zqVnD3edGldHTebAiN4MbcDkN5q1nbRV69BQ1_lwPt6b92_l6dcH0QJebQqovExY16Y7bQO02NGqjc8tkFPAeqC1cgI2VmojzG3FeAWqxtj5Ez5g0PYYJgxIoEXRopv9N2V-DME4mXwMxf3NVZ9d73Sm1Tb9p_U1OwQuWCh0p4kJHDsh44yBdM37KMLWSLM6pEr7jeWyzX0d1sKdfbORaVq0f1uzjZ_3iM_Oey7GMJKkPGYQlQjWbL2iyHv5PeAxJmZLykB0CZ0oUzOGlYfKJhL1x_j1D_zFckvj7o0K9GQ",
				KeyOps = ["verify"] // encryption not allowed
			};

			var result = key.Encrypt("Secret message");
			Assert.Empty(result);
		}

		[Fact]
		public void Encrypt_ShouldThrow_OnUnsupportedAlgorithm()
		{
			var key = new EncryptDecryptPublicKey
			{
				Algorithm = "RSA-PKCS1",
				E = "AQAB",
				N = "AQAB",
				KeyOps = ["encrypt"]
			};

			Assert.Throws<NotSupportedException>(() => key.Encrypt("Test"));
		}
	}
}

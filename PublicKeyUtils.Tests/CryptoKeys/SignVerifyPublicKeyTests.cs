using PublicKeyUtils.CryptoKeys;
using System.Security.Cryptography;
using System.Text;

namespace PublicKeyUtils.Tests.CryptoKeys
{
	public class SignVerifyPublicKeyTests
	{
		[Fact]
		public void Verify_ShouldReturnTrue_WithValidSignature()
		{
			// Arrange: Generate a test EC key and message/signature
			using var ecdsa = ECDsa.Create(ECCurve.NamedCurves.nistP256);
			var parameters = ecdsa.ExportParameters(false);

			string message = "Hello ECDSA!";
			byte[] messageBytes = Encoding.UTF8.GetBytes(message);
			byte[] signature = ecdsa.SignData(messageBytes, HashAlgorithmName.SHA256);

			var key = new SignVerifyPublicKey
			{
				Kty = "EC",
				Crv = "P-256",
				Ext = true,
				KeyOps = ["verify"],
				X = Convert.ToBase64String(parameters.Q.X).Replace("+", "-").Replace("/", "_").TrimEnd('='),
				Y = Convert.ToBase64String(parameters.Q.Y).Replace("+", "-").Replace("/", "_").TrimEnd('='),
			};

			string signatureB64 = Convert.ToBase64String(signature);

			// Act
			var result = key.Verify("SHA-256", message, signatureB64);

			// Assert
			Assert.True(result);
		}

		[Fact]
		public void Verify_ShouldReturnFalse_WhenKeyOpsMissing()
		{
			var key = new SignVerifyPublicKey
			{
				Kty = "EC",
				Crv = "P-256",
				Ext = true,
				KeyOps = null,
				X = "fakeX",
				Y = "fakeY"
			};

			var result = key.Verify("SHA-256", "Test message", "invalidSig==");
			Assert.False(result);
		}

		[Fact]
		public void Verify_ShouldReturnFalse_WhenKtyNotEC()
		{
			var key = new SignVerifyPublicKey
			{
				Kty = "RSA",
				Crv = "P-256",
				Ext = true,
				KeyOps = ["verify"],
				X = "fakeX",
				Y = "fakeY"
			};

			var result = key.Verify("SHA-256", "Test message", "invalidSig==");
			Assert.False(result);
		}

		[Fact]
		public void Verify_ShouldReturnFalse_WhenCurveIsInvalid()
		{
			var key = new SignVerifyPublicKey
			{
				Kty = "EC",
				Crv = "P-999", // Invalid curve
				Ext = true,
				KeyOps = ["verify"],
				X = "fakeX",
				Y = "fakeY"
			};

			var result = key.Verify("SHA-256", "Test message", "invalidSig==");
			Assert.False(result);
		}

		[Fact]
		public void Verify_ShouldReturnFalse_WhenHashAlgorithmIsInvalid()
		{
			var key = new SignVerifyPublicKey
			{
				Kty = "EC",
				Crv = "P-256",
				Ext = true,
				KeyOps = ["verify"],
				X = "fakeX",
				Y = "fakeY"
			};

			var result = key.Verify("MD5", "Test message", "invalidSig==");
			Assert.False(result);
		}
	}
}

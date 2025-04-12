using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;

namespace PublicKeyUtils.CryptoKeys
{
	public sealed class EncryptDecryptPublicKey
	{
		[JsonPropertyName("alg")]
		public string Algorithm { get; set; }

		[JsonPropertyName("e")]
		public string E { get; set; }

		[JsonPropertyName("ext")]
		public bool Ext { get; set; }

		[JsonPropertyName("key_ops")]
		public string[] KeyOps { get; set; }

		[JsonPropertyName("kty")]
		public string Kty { get; set; }

		[JsonPropertyName("n")]
		public string N { get; set; }

		public byte[] Modulus => Convert.FromBase64String(ToBase64Standard(N));
		public byte[] Exponent => Convert.FromBase64String(ToBase64Standard(E));
		private static string ToBase64Standard(string base64) => base64.Replace('-', '+').Replace('_', '/').PadRight(base64.Length + (4 - base64.Length % 4) % 4, '=');

		public RSAEncryptionPadding RSAEncryptionPadding
		{
			get
			{
				return Algorithm switch
				{
					"RSA-OAEP" => RSAEncryptionPadding.OaepSHA1,
					"RSA-OAEP-256" => RSAEncryptionPadding.OaepSHA256,
					"RSA-OAEP-384" => RSAEncryptionPadding.OaepSHA384,
					"RSA-OAEP-512" => RSAEncryptionPadding.OaepSHA512,
					_ => throw new NotSupportedException($"Unsupported algorithm: {Algorithm}"),// Handle other cases or throw an exception if necessary
				};
			}
		}

		public bool IsEncryptionAllowed => KeyOps.Contains("encrypt");

		public byte[] Encrypt(string plainText)
		{
			if (IsEncryptionAllowed)
			{
				RSA rsa = RSA.Create();
				// Extract key components
				rsa.ImportParameters(new()
				{
					Modulus = Modulus,
					Exponent = Exponent
				});
				byte[] messageBytes = Encoding.UTF8.GetBytes(plainText);
				return rsa.Encrypt(messageBytes, RSAEncryptionPadding);
			}
			return [];
		}
	}
}

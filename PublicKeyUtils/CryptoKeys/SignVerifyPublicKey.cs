using System.Security.Cryptography;
using System.Text.Json.Serialization;
using System.Text;

namespace PublicKeyUtils.CryptoKeys
{
	public sealed class SignVerifyPublicKey
	{
		[JsonPropertyName("x")]
		public string X { get; set; }

		[JsonPropertyName("y")]
		public string Y { get; set; }

		[JsonPropertyName("crv")]
		public string Crv { get; set; }

		[JsonPropertyName("ext")]
		public bool Ext { get; set; }

		[JsonPropertyName("key_ops")]
		public string[] KeyOps { get; set; }

		[JsonPropertyName("kty")]
		public string Kty { get; set; }



		public bool Verify(string hashAlgorithm, string message, string signatureAsBase64)
		{
			if (KeyOps == null || !KeyOps.Contains("verify"))
			{
				Console.WriteLine("Key operation missing.");
				return false;
			}
			if (Kty != "EC")
			{
				Console.WriteLine("Kty is not EC");
				return false;
			}
			var hashAlgorithmNames = new Dictionary<string, HashAlgorithmName>
			{
				{ "SHA-1", HashAlgorithmName.SHA1 },
				{ "SHA-256", HashAlgorithmName.SHA256 },
				{ "SHA-384", HashAlgorithmName.SHA384 },
				{ "SHA-512", HashAlgorithmName.SHA512 }
			};
			var namedCurves = new Dictionary<string, ECCurve>
			{
				{ "P-256", ECCurve.NamedCurves.nistP256 },
				{ "P-384", ECCurve.NamedCurves.nistP384 },
				{ "P-521", ECCurve.NamedCurves.nistP521 }
			};


			if (!hashAlgorithmNames.TryGetValue(hashAlgorithm, out HashAlgorithmName hashAlgorithmName))
			{
				Console.WriteLine($"Invalid hashAlgorithmName {hashAlgorithm}");
				return false;
			}
			else
			{
				if (!namedCurves.TryGetValue(Crv, out ECCurve eCCurve))
				{
					Console.WriteLine($"Invalid NamedCurve {Crv}");
					return false;
				}
				else
				{

					var signatureBytes = Convert.FromBase64String(signatureAsBase64);
					// Convert Base64URL-encoded x and y values to standard Base64
					string xBase64 = Base64UrlToBase64(X);
					string yBase64 = Base64UrlToBase64(Y);

					using var ecdsa = ECDsa.Create();
					ecdsa.ImportParameters(new ECParameters
					{
						Curve = eCCurve,
						Q = new ECPoint
						{
							X = Convert.FromBase64String(xBase64),
							Y = Convert.FromBase64String(yBase64)
						}
					});

					var messageBytes = Encoding.UTF8.GetBytes(message);
					return ecdsa.VerifyData(messageBytes, signatureBytes, hashAlgorithmName);

				}
			}
		}

		// Helper method to convert Base64URL to standard Base64
		private static string Base64UrlToBase64(string base64Url)
		{
			string base64 = base64Url.Replace('-', '+').Replace('_', '/');
			switch (base64.Length % 4)
			{
				case 2: base64 += "=="; break;
				case 3: base64 += "="; break;
				case 0: break;
				default: throw new FormatException("Invalid base64url string");
			}
			return base64;
		}

	}
}

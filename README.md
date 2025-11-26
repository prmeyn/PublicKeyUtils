# PublicKeyUtils

[![NuGet Version](https://img.shields.io/nuget/v/PublicKeyUtils.svg)](https://www.nuget.org/packages/PublicKeyUtils)
[![NuGet Downloads](https://img.shields.io/nuget/dt/PublicKeyUtils.svg)](https://www.nuget.org/packages/PublicKeyUtils)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Build Status](https://github.com/prmeyn/PublicKeyUtils/actions/workflows/release.yml/badge.svg)](https://github.com/prmeyn/PublicKeyUtils/actions)

PublicKeyUtils is an open-source .NET class library designed to simplify working with public keys for cryptographic operations. It provides utilities for encryption, decryption, signing, and verification using public keys. The library is built with extensibility and ease of use in mind, making it a great choice for developers working with RSA and ECC cryptography.

## Table of Contents

- [Features](#features)
- [Installation](#installation)
- [Usage Examples](#usage-examples)
  - [RSA Encryption](#rsa-encryption)
  - [ECC Signature Verification](#ecc-signature-verification)
- [Supported Algorithms](#supported-algorithms)
- [Contributing](#contributing)
- [CI/CD](#cicd)
- [License](#license)

## Features

- **RSA Public Key Operations**:
  - Encrypt data using RSA public keys with support for various padding schemes (RSA-OAEP with SHA-1, SHA-256, SHA-384, SHA-512)
  - Decode and parse RSA public key components (modulus and exponent) from Base64URL-encoded strings
  - Key operation validation to ensure encryption is only performed when allowed

- **ECC Public Key Operations**:
  - Verify digital signatures using ECC public keys
  - Support for named curves such as P-256, P-384, and P-521
  - Hash algorithm support for SHA-1, SHA-256, SHA-384, and SHA-512

- **JSON Serialization**:
  - Public key properties are annotated with `System.Text.Json.Serialization` attributes for seamless JSON serialization and deserialization
  - Compatible with JWK (JSON Web Key) format

- **Unit Tests**:
  - Comprehensive test coverage using xUnit to ensure correctness and reliability
  - All cryptographic operations thoroughly tested

## Installation

The library is available as a NuGet package. You can install it using one of the following methods:

### .NET CLI
```bash
dotnet add package PublicKeyUtils
```

### Package Manager Console
```powershell
Install-Package PublicKeyUtils
```

### Package Reference
```xml
<PackageReference Include="PublicKeyUtils" Version="*" />
```

For more details, visit the [NuGet package page](https://www.nuget.org/packages/PublicKeyUtils).

## Usage Examples

### RSA Encryption

Encrypt data using an RSA public key with different padding schemes:

```csharp
using PublicKeyUtils.CryptoKeys;

// Create an RSA public key for encryption
var publicKey = new EncryptDecryptPublicKey
{
    Algorithm = "RSA-OAEP-256",  // Use RSA-OAEP with SHA-256
    E = "AQAB",  // Public exponent (65537)
    N = "sXch9W6_K8oZn3PfJPZepKvXYTwc_nPIu9JrYfPRzM8zqVnD3edGldHTebAiN4MbcDkN5q1nbRV69BQ1_lwPt6b92_l6dcH0QJebQqovExY16Y7bQO02NGqjc8tkFPAeqC1cgI2VmojzG3FeAWqxtj5Ez5g0PYYJgxIoEXRopv9N2V-DME4mXwMxf3NVZ9d73Sm1Tb9p_U1OwQuWCh0p4kJHDsh44yBdM37KMLWSLM6pEr7jeWyzX0d1sKdfbORaVq0f1uzjZ_3iM_Oey7GMJKkPGYQlQjWbL2iyHv5PeAxJmZLykB0CZ0oUzOGlYfKJhL1x_j1D_zFckvj7o0K9GQ",  // Modulus (Base64URL)
    KeyOps = new[] { "encrypt" }  // Allowed operations
};

// Encrypt plaintext
string plaintext = "Hello, World!";
byte[] ciphertext = publicKey.Encrypt(plaintext);

Console.WriteLine($"Encrypted data: {Convert.ToBase64String(ciphertext)}");
```

**Supported RSA Padding Schemes:**
- `RSA-OAEP` - RSA-OAEP with SHA-1
- `RSA-OAEP-256` - RSA-OAEP with SHA-256
- `RSA-OAEP-384` - RSA-OAEP with SHA-384
- `RSA-OAEP-512` - RSA-OAEP with SHA-512

### ECC Signature Verification

Verify digital signatures using an ECC public key:

```csharp
using PublicKeyUtils.CryptoKeys;

// Create an ECC public key for signature verification
var publicKey = new SignVerifyPublicKey
{
    Algorithm = "ES256",  // ECDSA with SHA-256
    Curve = "P-256",      // Named curve
    X = "WKn-ZIGevcwGIyyrzFoZNBdaq9_TsqzGl96oc0CWuis",  // X coordinate (Base64URL)
    Y = "y77t-RvAHRKTsSGdIYUfweuOvwrvDD-Q3Hv5J0fSKbE",  // Y coordinate (Base64URL)
    KeyOps = new[] { "verify" }  // Allowed operations
};

// Data and signature to verify
string data = "Important message";
byte[] signature = Convert.FromBase64String("MEUCIQDKZokqnCjrRtw0...");

// Verify the signature
bool isValid = publicKey.Verify(data, signature);

Console.WriteLine($"Signature valid: {isValid}");
```

**Supported ECC Algorithms:**
- `ES256` - ECDSA with SHA-256
- `ES384` - ECDSA with SHA-384
- `ES512` - ECDSA with SHA-512

## Supported Algorithms

### RSA Encryption Padding Schemes
| Algorithm | Description | Hash Function |
|-----------|-------------|---------------|
| `RSA-OAEP` | RSA-OAEP padding | SHA-1 |
| `RSA-OAEP-256` | RSA-OAEP padding | SHA-256 |
| `RSA-OAEP-384` | RSA-OAEP padding | SHA-384 |
| `RSA-OAEP-512` | RSA-OAEP padding | SHA-512 |

### ECC Curves
| Curve | Key Size | Description |
|-------|----------|-------------|
| `P-256` | 256-bit | NIST P-256 (secp256r1) |
| `P-384` | 384-bit | NIST P-384 (secp384r1) |
| `P-521` | 521-bit | NIST P-521 (secp521r1) |

### Hash Algorithms
- SHA-1 (legacy support)
- SHA-256 (recommended)
- SHA-384
- SHA-512

## Contributing

Contributions are welcome! If you have ideas for new features, bug fixes, or improvements, feel free to:

1. **Open an Issue**: Report bugs or suggest features on the [GitHub Issues page](https://github.com/prmeyn/PublicKeyUtils/issues)
2. **Submit a Pull Request**: Fork the repository, make your changes, and submit a PR
3. **Improve Documentation**: Help enhance the documentation and examples

Please ensure all tests pass before submitting a pull request:
```bash
dotnet test
```

## CI/CD

This project uses GitHub Actions for continuous integration and deployment. On every push to a version tag (e.g., `v2.0.0`), the library is automatically:
- Built and tested
- Packaged as a NuGet package
- Published to [NuGet.org](https://www.nuget.org/packages/PublicKeyUtils)

See the [release workflow](.github/workflows/release.yml) for more details.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Acknowledgments

Special thanks to all contributors and the open-source community for their support and feedback.

---

**Links:**
- [NuGet Package](https://www.nuget.org/packages/PublicKeyUtils)
- [GitHub Repository](https://github.com/prmeyn/PublicKeyUtils)
- [Report Issues](https://github.com/prmeyn/PublicKeyUtils/issues)
- [Release Notes](https://github.com/prmeyn/PublicKeyUtils/releases)
# PublicKeyUtils

PublicKeyUtils is an open-source .NET class library designed to simplify working with public keys for cryptographic operations. It provides utilities for encryption, decryption, signing, and verification using public keys. The library is built with extensibility and ease of use in mind, making it a great choice for developers working with RSA and ECC cryptography.

## Features

- **RSA Public Key Operations**:
  - Encrypt data using RSA public keys with support for various padding schemes (e.g., RSA-OAEP).
  - Decode and parse RSA public key components (modulus and exponent) from Base64URL-encoded strings.

- **ECC Public Key Operations**:
  - Verify digital signatures using ECC public keys.
  - Support for named curves such as P-256, P-384, and P-521.
  - Hash algorithm support for SHA-1, SHA-256, SHA-384, and SHA-512.

- **JSON Serialization**:
  - Public key properties are annotated with `System.Text.Json.Serialization` attributes for seamless JSON serialization and deserialization.

- **Unit Tests**:
  - Comprehensive test coverage using xUnit to ensure correctness and reliability.

## Installation

The library is available as a NuGet package. You can install it using the following command:

```bash
dotnet add package PublicKeyUtils
```

Alternatively, visit the [NuGet package page](https://www.nuget.org/packages/PublicKeyUtils) for more details.

## Contributing
Contributions are welcome! If you have ideas for new features, bug fixes, or improvements, feel free to open an issue or submit a pull request on the [GitHub repository](https://github.com/prmeyn/PublicKeyUtils).

## CI/CD
This project uses GitHub Actions for continuous integration and deployment. On every push to a version tag (e.g., v2.0.0), the library is automatically built, packaged, and published to NuGet. See the release workflow for more details.

## License
This project is licensed under the MIT License. See the LICENSE file for details.

## Acknowledgments
Special thanks to all contributors and the open-source community for their support and feedback.
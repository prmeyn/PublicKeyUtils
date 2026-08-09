# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project

`PublicKeyUtils` is a .NET 10 class library published to NuGet that wraps RSA encryption and ECDSA
signature verification over **public keys supplied in JWK (JSON Web Key) form**. It has no third-party
runtime dependencies — only `System.Security.Cryptography` and `System.Text.Json` from the BCL.

Two projects: `PublicKeyUtils` (the shipped library) and `PublicKeyUtils.Tests` (xUnit.net v3).

## Commands

```bash
dotnet build                  # build both projects
dotnet test                   # run all tests
dotnet pack PublicKeyUtils/PublicKeyUtils.csproj --configuration Release
```

Run a subset of tests — note this does **not** go through `dotnet test`:

```bash
dotnet run --project PublicKeyUtils.Tests -- --filter-method "*.Encrypt_ShouldThrow_OnUnsupportedAlgorithm"
dotnet run --project PublicKeyUtils.Tests -- --filter-class "*.SignVerifyPublicKeyTests"
dotnet run --project PublicKeyUtils.Tests -- --list-tests
```

**`dotnet test --filter "..."` is silently ignored** — it emits `warning MTP0001` and runs the whole suite
anyway, because the VSTest filter properties mean nothing to Microsoft Testing Platform. Use the
`dotnet run -- --filter-*` form above, or invoke the built app directly
(`PublicKeyUtils.Tests/bin/Debug/net10.0/PublicKeyUtils.Tests.exe --filter-method "..."`).

There is no linter or formatter configured. The build is warning-clean; keep it that way.

## Test project runtime (non-obvious)

The test project runs on **xUnit.net v3 under Microsoft Testing Platform, not VSTest**. Consequences:

- `xunit.v3` is the *only* `PackageReference`. There is deliberately no `Microsoft.NET.Test.Sdk`,
  no `xunit.runner.visualstudio`, and no `coverlet.collector` — don't re-add them.
- The test project is `<OutputType>Exe</OutputType>`. xUnit v3 hard-errors at build time without it.
- `<TestingPlatformDotnetTestSupport>true</TestingPlatformDotnetTestSupport>` is what keeps plain
  `dotnet test` (and the CI step) working, by routing the `VSTest` target to `InvokeTestingPlatform`.
- No coverage collector is installed, so `--collect "XPlat Code Coverage"` will not produce anything.
  The MTP equivalent would be `Microsoft.Testing.Extensions.CodeCoverage`.

## Architecture

`PublicKeyUtils/CryptoKeys/` holds two `sealed` classes that are **independent by design** — no shared
base class or helper, and each re-implements its own Base64URL→Base64 padding conversion
(`ToBase64Standard` vs `Base64UrlToBase64`). Don't "unify" them without a reason; they diverge in what
they validate.

- `EncryptDecryptPublicKey` — RSA. JWK fields `alg`/`n`/`e`/`kty`/`key_ops`/`ext`. Exposes `Modulus`
  and `Exponent` (decoded bytes), an `RSAEncryptionPadding` derived from `alg`, and `Encrypt(string)`.
- `SignVerifyPublicKey` — ECDSA. JWK fields `crv`/`x`/`y`/`kty`/`key_ops`/`ext`. Exposes
  `Verify(hashAlgorithm, message, signatureAsBase64)`. Requires `Kty == "EC"`.

**The `[JsonPropertyName]` values are the public contract.** These types are deserialization targets for
JWKs produced elsewhere (e.g. WebCrypto `exportKey`), so the short lowercase names (`n`, `e`, `alg`,
`crv`, `key_ops`) matter more than the C# property names. Renaming a C# property is safe; changing a
JSON name is a breaking change.

**All properties are nullable** (`string?`, `string[]?`) because a JWK legitimately arrives without
optional members. Guard before dereferencing.

### Error-handling convention

The two classes deliberately use different failure modes, and the tests lock this in:

| Situation | Behaviour |
|---|---|
| `key_ops` missing or lacks the needed op | falsy result — `Verify` → `false`, `Encrypt` → `[]` |
| Wrong `kty`, unknown curve, unknown hash name, missing `x`/`y` | `false` + a `Console.WriteLine` diagnostic |
| Unsupported `alg` on RSA | throws `NotSupportedException` |
| `n`/`e` absent when reading `Modulus`/`Exponent` | throws `InvalidOperationException` |

Turning a "return falsy" guard into a throw (or vice versa) will break tests. Recognised hash names are
`SHA-1`/`SHA-256`/`SHA-384`/`SHA-512` and curves `P-256`/`P-384`/`P-521` — both are plain dictionary
lookups inside `Verify`, so adding support means adding a dictionary entry.

## Release process

Releases are **tag-driven**; the version lives nowhere in the repo.

- `PublicKeyUtils.csproj` has no `<Version>`, so a local `dotnet pack` always produces `1.0.0`. CI passes
  `/p:Version=${VERSION}` parsed from the tag. Don't add a `<Version>` to "fix" the local number.
- Pushing a tag matching `v[0-9]+.[0-9]+.[0-9]+` runs `.github/workflows/release.yml`: build → test →
  pack → push to nuget.org.
- The workflow **refuses to publish a tag that isn't an ancestor of `origin/main`** (`git branch --remote
  --contains | grep origin/main`). Tag from main.
- Publishing uses **NuGet trusted publishing (OIDC)**, not a stored API key: the job needs
  `permissions: id-token: write`, and `NuGet/login` exchanges the token for a 1-hour key immediately
  before the push. **No repo secret is involved.** The `user:` input is hardcoded to `globalpay` — the
  nuget.org account that owns the package (not `prmeyn`) and holds its trusted-publishing policy. That
  account name is public, so it's inline on purpose: it was a `NUGET_USER` secret once, and an unset
  secret expands to the empty string rather than failing, which broke a release with the opaque
  `Input required and not supplied: user`. Don't reintroduce the indirection.
- **Actions are pinned to commit SHAs** with a trailing `# vX.Y.Z` comment. Preserve this when touching
  the workflow.

## Conventions

Source files use **tabs** for indentation and CRLF line endings, block-scoped (not file-scoped)
namespaces, and Allman braces. Match the surrounding file.

## README examples

The C# snippets in `README.md` were corrected and compile-verified against the library in August 2026
(the ECC example had drifted badly — it referenced `Algorithm`/`Curve` properties that never existed and
a two-argument `Verify`). They are not covered by the test suite, so they can silently rot again. If you
change a public signature, update the README in the same change, and re-check the snippets by compiling
them against `PublicKeyUtils.csproj` in a scratch project rather than by eye.

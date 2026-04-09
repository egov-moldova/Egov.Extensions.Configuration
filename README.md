# Egov.Extensions.Configuration

[![NuGet](https://img.shields.io/nuget/v/Egov.Extensions.Configuration.svg)](https://www.nuget.org/packages/Egov.Extensions.Configuration)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

A .NET library that provides helpers for certificate loading and configuring `IOptions<SystemCertificateOptions>` in ASP.NET Core 10.0+ applications. It serves as a shared foundation for certificate management in services built on the eGov platform.

---

## Table of Contents

- [Features](#features)
- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Configuration](#configuration)
- [Usage](#usage)
  - [Dependency Injection (Recommended)](#using-dependency-injection-recommended)
  - [CertificateLoader Directly](#using-certificateloader-directly)
  - [CertificateConverter (TypeConverter)](#using-certificateconverter)
- [Supported Certificate Formats](#supported-certificate-formats)
- [Error Handling](#error-handling)
- [Testing](#testing)
- [Contributing](#contributing)
- [Code of Conduct](#code-of-conduct)
- [AI Assistance](#ai-assistance)
- [License](#license)

---

## Features

- Load public certificates from CER/CRT files
- Load private certificates from PFX/PKCS#12 files
- Load certificates from Kubernetes TLS secrets (`tls.crt` / `tls.key`)
- Load full certificate chains including intermediate certificates
- Register certificates via ASP.NET Core Dependency Injection
- `TypeConverter` support for binding certificates directly from configuration strings

---

## Prerequisites

- .NET 10.0 or later
- A valid certificate file (PFX, CER, or PEM format)
- Certificate password if the certificate is password-protected
- For Kubernetes deployments: certificates mounted as [TLS secrets](https://kubernetes.io/docs/concepts/configuration/secret/#tls-secrets) with `tls.crt` and `tls.key` files

---

## Installation

Install the package from [NuGet](https://www.nuget.org/packages/Egov.Extensions.Configuration):

```shell
dotnet add package Egov.Extensions.Configuration
```

Or via the Package Manager Console:

```shell
Install-Package Egov.Extensions.Configuration
```

---

## Configuration

Add the following section to your **appsettings.json**:

```json
{
  "Certificate": {
    "Path": "Files/Certificates/your-certificate.pfx",
    "Password": "your-certificate-password"
  }
}
```

**`Path`** can be:
- A path to a **PFX** file (requires `Password` if encrypted)
- A path to a **CER/CRT** file (public certificate only)
- A path to a **directory** containing `tls.crt` and `tls.key` files (Kubernetes mounted secret)

**`Password`** is optional and only required for encrypted PFX files or encrypted PEM private keys.

---

## Usage

### Using Dependency Injection (Recommended)

Register the certificate in **Program.cs**:

```csharp
builder.Services.AddSystemCertificate(builder.Configuration.GetSection("Certificate"));
```

Or configure it inline:

```csharp
builder.Services.AddSystemCertificate(options =>
{
    options.Path = "Files/Certificates/your-certificate.pfx";
    options.Password = "your-certificate-password";
});
```

Then inject `IOptions<SystemCertificateOptions>` into your services:

```csharp
public class MyService
{
    private readonly SystemCertificateOptions _certificateOptions;

    public MyService(IOptions<SystemCertificateOptions> certificateOptions)
    {
        _certificateOptions = certificateOptions.Value;
    }

    public void UseCertificate()
    {
        var certificate = _certificateOptions.Certificate;
        // Use the certificate...
    }
}
```

The `SystemCertificateOptions` also exposes `IntermediateCertificates` when the loaded certificate chain contains intermediate certificates.

---

### Using CertificateLoader Directly

You can use the `CertificateLoader` static class without dependency injection.

**Load a public certificate (CER/CRT):**

```csharp
var certificate = CertificateLoader.Public("path/to/certificate.cer");
```

**Load a private certificate (PFX):**

```csharp
var certificate = CertificateLoader.Private("path/to/certificate.pfx", "password");
```

**Load from a Kubernetes mounted secret:**

```csharp
var certificate = CertificateLoader.Private("/etc/ssl/certs/my-secret", null);
```

**Load a full chain (certificate + intermediates):**

```csharp
var certificate = CertificateLoader.PrivateChain(
    "path/to/certificate.pfx",
    "password",
    out X509Certificate2Collection? intermediates);
```

---

### Using CertificateConverter

`CertificateConverter` is a `TypeConverter` that allows binding an `X509Certificate2` directly from a configuration string.

Register it once at application startup:

```csharp
CertificateConverter.Register();
```

After registration, configuration strings are automatically converted:
- `"path/to/cert.cer"` → loads a public certificate
- `"path/to/cert.pfx|password"` → loads a private certificate (path and password separated by `|`)

---

## Supported Certificate Formats

| Format | Description |
|--------|-------------|
| **PFX / PKCS#12** | Password-protected certificate bundle with private key |
| **CER / CRT** | Public certificate file (DER or PEM encoded) |
| **PEM** | Certificate and key pair (`tls.crt` + `tls.key`), used with Kubernetes TLS secrets |

---

## Error Handling

| Scenario | Exception |
|----------|-----------|
| Path is null or empty | `ArgumentException` |
| File or directory not found | `ArgumentException` |
| No certificate with private key found in PFX | `ArgumentException` |
| Certificate could not be loaded (missing path and no direct assignment) | `InvalidOperationException` |
| Loaded certificate does not contain a private key | `InvalidOperationException` |

---

## Testing

The solution includes a dedicated test project `Egov.Extensions.Configuration.Tests` built with [xUnit v3](https://xunit.net/).

### Test coverage

| Test class | What is covered |
|---|---|
| `CertificateLoaderTests` | `CertificateLoader.Public`, `Private`, and `PrivateChain` — happy paths, null/empty/missing paths, PEM+key via directory |
| `CertificateConverterTests` | `CertificateConverter` type conversion from config strings (CER path, PFX path with `\|` separator), error cases |
| `SystemCertificateOptionsTests` | Default property values, property assignment, `Dispose` safety (null cert, double-dispose, intermediate certificates) |
| `SystemCertificateOptionsPostConfigureTests` | Post-configure pipeline: loads certificate from path, validates private key presence, propagates intermediates |
| `SystemCertificateExtensionsTests` | `AddSystemCertificate` DI extension — section binding and inline `Action<>` overload |

### Test certificates

The test project ships self-signed test certificates under `TestCertificates/`:

| File | Purpose |
|---|---|
| `egov_library_test.pfx` | Password-protected PKCS#12 bundle (password: `test`) |
| `egov_library_test.cer` | Public certificate (PEM) |
| `egov_library_tes.key` | Private key (PEM) |

These files are automatically copied to the build output directory by the project file and are **not** intended for production use.

### Running the tests

```shell
dotnet test src/Egov.Extensions.Configuration.Tests
```

Or from the solution root:

```shell
dotnet test
```

---

## Contributing

Contributions are welcome! Please read [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines on how to get started.

---

## Code of Conduct

This project adheres to the [Contributor Covenant Code of Conduct](CODE_OF_CONDUCT.md). By participating, you are expected to uphold this code.

---

## AI Assistance

This repository contains an [AGENTS.md](AGENTS.md) file with instructions and context for AI coding agents to assist in development, ensuring consistency in code style and project structure.

---

## License

This project is licensed under the [MIT License](LICENSE).

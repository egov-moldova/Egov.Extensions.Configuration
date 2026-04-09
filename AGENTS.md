# AGENTS.md

This project provides helper classes and extensions for certificate management in .NET applications, primarily focused on the eGov platform.

## Tech Stack
- **Framework:** .NET 10.0+
- **Project Type:** Class Library
- **Dependencies:** ASP.NET Core `Microsoft.Extensions.Configuration.Abstractions`, `Microsoft.Extensions.Options`
- **Testing:** xUnit v3

## Project Structure
- `src/Egov.Extensions.Configuration/`: Main project containing `CertificateLoader`, `SystemCertificateOptions`, and DI extensions.
- `src/Egov.Extensions.Configuration.Tests/`: Unit tests and integration tests.
- `src/Egov.Extensions.Configuration.Tests/TestCertificates/`: Self-signed certificates for testing.

## Build and Test Commands
- **Build Solution:** `dotnet build src/Egov.Extensions.Configuration.sln`
- **Run Tests:** `dotnet test src/Egov.Extensions.Configuration.sln`
- **Clean Artifacts:** `rm -rf artifacts/` (on Windows use `Remove-Item -Recurse -Force artifacts/`)

## Coding Standards
- Use **C# 14.0** features.
- Follow standard .NET naming conventions (PascalCase for classes/methods, camelCase for local variables).
- **XML Documentation:** All public members in `CertificateLoader` and `SystemCertificateOptions` must have `<summary>` and `<param>` tags.
- **Resource Management:** Ensure `X509Certificate2` objects are disposed of correctly unless ownership is transferred (e.g., in `SystemCertificateOptions`).
- **Tests:** Use xUnit v3 features. New features or bug fixes should always include tests in `Egov.Extensions.Configuration.Tests`.

## Maintenance
- Keep dependencies in `.csproj` files up to date.
- Update `README.md` and `AGENTS.md` when adding new major features or changing build workflows.

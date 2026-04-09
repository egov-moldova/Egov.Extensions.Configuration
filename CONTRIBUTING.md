# Contributing to Egov.Extensions.Configuration

Thank you for your interest in contributing! We welcome bug reports, feature requests, documentation improvements, and code contributions.

---

## Table of Contents

- [Code of Conduct](#code-of-conduct)
- [Getting Started](#getting-started)
- [Development Setup](#development-setup)
- [Branching Strategy](#branching-strategy)
- [Making Changes](#making-changes)
- [Commit Messages](#commit-messages)
- [Pull Request Process](#pull-request-process)
- [Reporting Issues](#reporting-issues)

---

## Code of Conduct

Please be respectful and constructive in all interactions. We expect all contributors to follow basic open-source etiquette: be kind, be patient, and focus on the work.

---

## Getting Started

1. **Fork** the repository on GitHub.
2. **Clone** your fork locally:
   ```shell
   git clone https://github.com/your-username/ConfigurationExtensions.git
   cd ConfigurationExtensions
   ```
3. Add the upstream remote:
   ```shell
   git remote add upstream https://github.com/egov-md/ConfigurationExtensions.git
   ```

---

## Development Setup

### Requirements

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download) or later
- Any IDE with C# support (JetBrains Rider, Visual Studio, VS Code)

### Build

```shell
dotnet build src/Egov.Extensions.Configuration.sln
```

### Run Tests

```shell
dotnet test src/Egov.Extensions.Configuration.sln
```

---

## Branching Strategy

| Branch | Purpose |
|--------|---------|
| `main` | Stable, released code |
| `feature/<name>` | New features |
| `fix/<name>` | Bug fixes |
| `docs/<name>` | Documentation-only changes |

Always branch off from `main` and target `main` in your pull request.

---

## Making Changes

1. Create a new branch from `main`:
   ```shell
   git checkout -b feature/my-feature
   ```
2. Make your changes, following the existing code style.
3. Ensure the project builds without errors:
   ```shell
   dotnet build src/Egov.Extensions.Configuration.sln
   ```
4. Add or update XML documentation comments (`/// <summary>`) for any public API changes.
5. Commit your changes (see [Commit Messages](#commit-messages)).
6. Push your branch and open a Pull Request.

---

## Commit Messages

Use clear, concise commit messages in the imperative mood:

```
Add support for PKCS#8 private key format
Fix NullReferenceException when Path is empty
Update README with CertificateConverter examples
```

- Keep the subject line under 72 characters.
- Reference related issues using `Fixes #123` or `Closes #123` in the commit body when applicable.

---

## Pull Request Process

1. Ensure your branch is up to date with `main`:
   ```shell
   git fetch upstream
   git rebase upstream/main
   ```
2. Open a Pull Request against the `main` branch.
3. Fill in the PR description:
   - **What** was changed and **why**
   - Any related issue numbers
   - Any breaking changes
4. A maintainer will review your PR. Please respond to review comments promptly.
5. Once approved, a maintainer will merge your PR.

---

## Reporting Issues

When filing a bug report, please include:

- .NET version (`dotnet --version`)
- Operating system and version
- Package version
- A minimal reproducible example
- The full exception message and stack trace (if applicable)

For feature requests, describe the use case and the expected behavior.

Open an issue at: **https://github.com/egov-md/ConfigurationExtensions/issues**

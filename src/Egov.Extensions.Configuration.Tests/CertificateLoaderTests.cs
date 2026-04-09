using System.Security.Cryptography.X509Certificates;
using Egov.Extensions.Configuration;

namespace Egov.Extensions.Configuration.Tests;

public class CertificateLoaderTests
{
    private static readonly string CertDir = Path.Combine(AppContext.BaseDirectory, "TestCertificates");
    private static readonly string PfxPath = Path.Combine(CertDir, "egov_library_test.pfx");
    private static readonly string CerPath = Path.Combine(CertDir, "egov_library_test.cer");
    private static readonly string KeyPath = Path.Combine(CertDir, "egov_library_tes.key");
    private const string PfxPassword = "test";

    // ── Public ────────────────────────────────────────────────────────────────

    [Fact]
    public void Public_LoadsCerFile_ReturnsCertificate()
    {
        var cert = CertificateLoader.Public(CerPath);
        Assert.NotNull(cert);
        cert.Dispose();
    }

    [Fact]
    public void Public_NullPath_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => CertificateLoader.Public(null!));
    }

    [Fact]
    public void Public_EmptyPath_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => CertificateLoader.Public("   "));
    }

    [Fact]
    public void Public_NonExistentPath_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => CertificateLoader.Public("nonexistent.cer"));
    }

    [Fact]
    public void Public_DirectoryWithoutTlsCrt_ThrowsArgumentException()
    {
        var dir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(dir);
        try
        {
            Assert.Throws<ArgumentException>(() => CertificateLoader.Public(dir));
        }
        finally
        {
            Directory.Delete(dir, true);
        }
    }

    // ── Private ───────────────────────────────────────────────────────────────

    [Fact]
    public void Private_LoadsPfxFile_ReturnsCertificateWithPrivateKey()
    {
        var cert = CertificateLoader.Private(PfxPath, PfxPassword);
        Assert.NotNull(cert);
        Assert.True(cert.HasPrivateKey);
        cert.Dispose();
    }

    [Fact]
    public void Private_NullPath_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => CertificateLoader.Private(null!));
    }

    [Fact]
    public void Private_EmptyPath_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => CertificateLoader.Private("   "));
    }

    [Fact]
    public void Private_NonExistentPath_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => CertificateLoader.Private("nonexistent.pfx"));
    }

    // ── PrivateChain ──────────────────────────────────────────────────────────

    [Fact]
    public void PrivateChain_LoadsPfxFile_ReturnsCertificateWithPrivateKey()
    {
        var cert = CertificateLoader.PrivateChain(PfxPath, PfxPassword, out var intermediates);
        Assert.NotNull(cert);
        Assert.True(cert.HasPrivateKey);
        cert.Dispose();
        if (intermediates != null) foreach (var c in intermediates) c.Dispose();
    }

    [Fact]
    public void PrivateChain_SelfSignedPfx_IntermediatesIsNull()
    {
        // A self-signed cert has no intermediates
        var cert = CertificateLoader.PrivateChain(PfxPath, PfxPassword, out var intermediates);
        Assert.Null(intermediates);
        cert.Dispose();
    }

    [Fact]
    public void PrivateChain_NullPath_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => CertificateLoader.PrivateChain(null!, null, out _));
    }

    [Fact]
    public void PrivateChain_EmptyPath_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => CertificateLoader.PrivateChain("   ", null, out _));
    }

    [Fact]
    public void PrivateChain_NonExistentPath_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => CertificateLoader.PrivateChain("nonexistent.pfx", null, out _));
    }

    // ── PEM + Key ─────────────────────────────────────────────────────────────

    [Fact]
    public void Private_LoadsCerAndKeyFiles_ReturnsCertificateWithPrivateKey()
    {
        var cert = X509Certificate2.CreateFromPemFile(CerPath, KeyPath);
        Assert.NotNull(cert);
        Assert.True(cert.HasPrivateKey);
        cert.Dispose();
    }

    [Fact]
    public void PrivateChain_LoadsCerAndKeyFiles_ReturnsCertificateWithPrivateKey()
    {
        var dir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(dir);
        try
        {
            File.Copy(CerPath, Path.Combine(dir, "tls.crt"));
            File.Copy(KeyPath, Path.Combine(dir, "tls.key"));

            var cert = CertificateLoader.PrivateChain(dir, null, out var intermediates);
            Assert.NotNull(cert);
            Assert.True(cert.HasPrivateKey);
            cert.Dispose();
            if (intermediates != null) foreach (var c in intermediates) c.Dispose();
        }
        finally
        {
            Directory.Delete(dir, true);
        }
    }

    [Fact]
    public void Private_LoadsCerAndKeyViaDirectory_ReturnsCertificateWithPrivateKey()
    {
        var dir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(dir);
        try
        {
            File.Copy(CerPath, Path.Combine(dir, "tls.crt"));
            File.Copy(KeyPath, Path.Combine(dir, "tls.key"));

            var cert = CertificateLoader.Private(dir);
            Assert.NotNull(cert);
            Assert.True(cert.HasPrivateKey);
            cert.Dispose();
        }
        finally
        {
            Directory.Delete(dir, true);
        }
    }
}

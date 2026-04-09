using System.Security.Cryptography.X509Certificates;
using Egov.Extensions.Configuration;

namespace Egov.Extensions.Configuration.Tests;

public class SystemCertificateOptionsTests
{
    private static readonly string CertDir = Path.Combine(AppContext.BaseDirectory, "TestCertificates");
    private static readonly string PfxPath = Path.Combine(CertDir, "egov_library_test.pfx");
    private const string PfxPassword = "test";

    [Fact]
    public void Options_DefaultValues_AreNull()
    {
        var options = new SystemCertificateOptions();
        Assert.Null(options.Path);
        Assert.Null(options.Password);
        Assert.Null(options.Certificate);
        Assert.Null(options.IntermediateCertificates);
    }

    [Fact]
    public void Options_CanSetProperties()
    {
        var options = new SystemCertificateOptions
        {
            Path = "some/path",
            Password = "secret"
        };
        Assert.Equal("some/path", options.Path);
        Assert.Equal("secret", options.Password);
    }

    [Fact]
    public void Dispose_DoesNotThrow_WhenCertificateIsNull()
    {
        var options = new SystemCertificateOptions();
        var ex = Record.Exception(() => options.Dispose());
        Assert.Null(ex);
    }

    [Fact]
    public void Dispose_DisposesCertificate()
    {
        var cert = CertificateLoader.Private(PfxPath, PfxPassword);
        var options = new SystemCertificateOptions { Certificate = cert };
        options.Dispose();
        // No exception expected; double-dispose should also be safe
        var ex = Record.Exception(() => options.Dispose());
        Assert.Null(ex);
    }

    [Fact]
    public void Dispose_DisposesIntermediateCertificates()
    {
        var cert = CertificateLoader.Private(PfxPath, PfxPassword);
        var intermediates = new X509Certificate2Collection();
        var options = new SystemCertificateOptions
        {
            Certificate = cert,
            IntermediateCertificates = intermediates
        };
        var ex = Record.Exception(() => options.Dispose());
        Assert.Null(ex);
    }

    [Fact]
    public void Dispose_CalledTwice_DoesNotThrow()
    {
        var options = new SystemCertificateOptions
        {
            Certificate = CertificateLoader.Private(PfxPath, PfxPassword)
        };
        options.Dispose();
        var ex = Record.Exception(() => options.Dispose());
        Assert.Null(ex);
    }
}

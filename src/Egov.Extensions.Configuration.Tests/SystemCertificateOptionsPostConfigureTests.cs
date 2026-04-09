using Egov.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Egov.Extensions.Configuration.Tests;

public class SystemCertificateOptionsPostConfigureTests
{
    private static readonly string CertDir = Path.Combine(AppContext.BaseDirectory, "TestCertificates");
    private static readonly string PfxPath = Path.Combine(CertDir, "egov_library_test.pfx");
    private static readonly string CerPath = Path.Combine(CertDir, "egov_library_test.cer");
    private const string PfxPassword = "test";

    private static IPostConfigureOptions<SystemCertificateOptions> CreatePostConfigure()
        => new SystemCertificateOptionsPostConfigure();

    [Fact]
    public void PostConfigure_WithValidPfxPath_LoadsCertificate()
    {
        var postConfigure = CreatePostConfigure();
        var options = new SystemCertificateOptions { Path = PfxPath, Password = PfxPassword };

        postConfigure.PostConfigure(null, options);

        Assert.NotNull(options.Certificate);
        Assert.True(options.Certificate.HasPrivateKey);
        options.Dispose();
    }

    [Fact]
    public void PostConfigure_WithPreAssignedCertificate_DoesNotReload()
    {
        var postConfigure = CreatePostConfigure();
        var cert = CertificateLoader.Private(PfxPath, PfxPassword);
        var options = new SystemCertificateOptions { Certificate = cert };

        postConfigure.PostConfigure(null, options);

        Assert.Same(cert, options.Certificate);
        options.Dispose();
    }

    [Fact]
    public void PostConfigure_NoCertificateAndNoPath_ThrowsInvalidOperationException()
    {
        var postConfigure = CreatePostConfigure();
        var options = new SystemCertificateOptions();

        Assert.Throws<InvalidOperationException>(() => postConfigure.PostConfigure(null, options));
    }

    [Fact]
    public void PostConfigure_CertificateWithoutPrivateKey_ThrowsInvalidOperationException()
    {
        var postConfigure = CreatePostConfigure();
        var publicCert = CertificateLoader.Public(CerPath);
        var options = new SystemCertificateOptions { Certificate = publicCert };

        Assert.Throws<InvalidOperationException>(() => postConfigure.PostConfigure(null, options));
        options.Dispose();
    }

    [Fact]
    public void PostConfigure_SetsIntermediateCertificates_FromPfx()
    {
        var postConfigure = CreatePostConfigure();
        var options = new SystemCertificateOptions { Path = PfxPath, Password = PfxPassword };

        postConfigure.PostConfigure(null, options);

        // Self-signed cert has no intermediates — null is expected
        Assert.Null(options.IntermediateCertificates);
        options.Dispose();
    }
}

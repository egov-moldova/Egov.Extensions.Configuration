using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;
using Egov.Extensions.Configuration;

namespace Egov.Extensions.Configuration.Tests;

public class CertificateConverterTests
{
    private static readonly string CertDir = Path.Combine(AppContext.BaseDirectory, "TestCertificates");
    private static readonly string PfxPath = Path.Combine(CertDir, "egov_library_test.pfx");
    private static readonly string CerPath = Path.Combine(CertDir, "egov_library_test.cer");
    private const string PfxPassword = "test";

    [Fact]
    public void CanConvertFrom_String_ReturnsTrue()
    {
        var converter = new CertificateConverter();
        Assert.True(converter.CanConvertFrom(typeof(string)));
    }

    [Fact]
    public void CanConvertFrom_Int_ReturnsFalse()
    {
        var converter = new CertificateConverter();
        Assert.False(converter.CanConvertFrom(typeof(int)));
    }

    [Fact]
    public void ConvertFrom_CerPath_ReturnsPublicCertificate()
    {
        var converter = new CertificateConverter();
        var result = converter.ConvertFrom(CerPath);
        Assert.NotNull(result);
        var cert = Assert.IsType<X509Certificate2>(result);
        cert.Dispose();
    }

    [Fact]
    public void ConvertFrom_PfxPathWithPassword_ReturnsPrivateCertificate()
    {
        var converter = new CertificateConverter();
        var result = converter.ConvertFrom($"{PfxPath}|{PfxPassword}");
        Assert.NotNull(result);
        var cert = Assert.IsType<X509Certificate2>(result);
        Assert.True(cert.HasPrivateKey);
        cert.Dispose();
    }

    [Fact]
    public void ConvertFrom_NullString_ThrowsNotSupportedException()
    {
        var converter = new CertificateConverter();
        Assert.Throws<NotSupportedException>(() => converter.ConvertFrom(null!));
    }

    [Fact]
    public void ConvertFrom_WhitespaceString_ThrowsNotSupportedException()
    {
        var converter = new CertificateConverter();
        Assert.Throws<NotSupportedException>(() => converter.ConvertFrom("   "));
    }

    [Fact]
    public void Register_AddsTypeConverterForX509Certificate2()
    {
        CertificateConverter.Register();
        var converter = TypeDescriptor.GetConverter(typeof(X509Certificate2));
        Assert.IsType<CertificateConverter>(converter);
    }
}
